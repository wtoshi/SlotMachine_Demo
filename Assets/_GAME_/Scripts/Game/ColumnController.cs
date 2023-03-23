using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ColumnController : Singleton<ColumnController>
{
    public GameObject SlotPF;
     
    public Column[] m_columns = new Column[5];
    public AudioClip m_slotStartSound;
    
    AudioSource m_audiosource;

    float delayAmongSlots;
    float spinTime;

    private void OnEnable()
    {
        GameEvents.ColumnStopped.AddListener(OnColumnStopped);
    }

    private void OnDisable()
    {
        GameEvents.ColumnStopped.AddListener(OnColumnStopped);
    }

    void Start()
    {
        m_audiosource = GetComponent<AudioSource>();

        Initialize();
    }

    void Initialize() 
    {
        delayAmongSlots = GameManager.Instance.gameSettings.delayAmongSlots;

        StartCoroutine(InitColumns());
    }

    IEnumerator InitColumns()
    {
        yield return new WaitUntil(()=> m_columns[0].IsReady && m_columns[1].IsReady && m_columns[2].IsReady && m_columns[3].IsReady && m_columns[4].IsReady);

        var initialSlot = GameManager.Instance.gameSettings.resources.Find(x => x.type == GameEntries.SlotType.Temp);

        foreach (var column in m_columns)
        {
            column.AddSlot(initialSlot);
            column.AddSlot(initialSlot);
            column.AddSlot(initialSlot);
        }
    }

    void SetColumnSlots()
    {
        for (int i = 0; i < m_columns.Length; i++)
        {
            var currentColumn = m_columns[i];

            currentColumn.PrepareForNextSpin();

            int otherSlotsCount = 7;

            for (int x = 0; x < otherSlotsCount; x++)
            {
                var randomSlot = GameManager.Instance.gameSettings.GetRandomResource();

                currentColumn.AddSlot(randomSlot);
            }

            List<GameEntries.SlotResource> targetResources = new();

            if (!GameManager.Instance.requestedSpinResult.randomSlots && GameManager.Instance.requestedSpinResult.requestedRows != null)
            {

                var requestedRows = GameManager.Instance.requestedSpinResult.requestedRows;
                for (int x = requestedRows.desiredRows.Length - 1; x >= 0; x--)
                {
                    var resourceType = requestedRows.desiredRows[x].row[i];
                    var requestedResource = GameManager.Instance.gameSettings.resources.Find(x => x.type == resourceType);

                    targetResources.Add(requestedResource);
                }
            }
            else
            {
                for (int x = 0; x < 3; x++)
                {
                    var randomResource = GameManager.Instance.gameSettings.GetRandomResource();

                    targetResources.Add(randomResource);
                }
            }

            m_columns[i].SetTargetResources(targetResources);
        }      

    }

    public void StartSpin()
    {

        m_allColumnsResultData.Clear();
        m_audiosource.PlayOneShot(m_slotStartSound);


        SetColumnSlots();

        spinTime = Random.Range(GameManager.Instance.gameSettings.spinDuration.x, GameManager.Instance.gameSettings.spinDuration.y);

        for (int i = 0; i < m_columns.Length; i++)
        {
            float totalSpinTime = spinTime + delayAmongSlots * i;
            m_columns[i].StartSpin(totalSpinTime);
        }
    }

    public void StopSpin()
    {
        for (int i = 0; i < m_columns.Length; i++)
        {
            m_columns[i].StopSpin();
        }
    }

    public bool CanColumnEnterLastTurn(int columnIndex)
    {
        if (columnIndex == 0 || m_columns[columnIndex - 1].m_columnState == GameEntries.ColumnState.Idle)
            return true;

        return false;
    }

    void SpeedUpSlot(int columnIndex)
    {
        var slot = m_columns[columnIndex];

        if (slot == null)
            return;

        slot.SpeedUpSlot();
    }

    void OnColumnStopped(int columnIndex, GameEntries.CurrentColumnData columnData)
    {
        Debug.Log("Column Stopped: "+columnIndex);
        if (m_allColumnsResultData.ContainsKey(columnIndex))
        {
            Debug.Log("m_allColumnsResultData Contains Same Key!: "+ columnIndex);

            return;
        }

        m_allColumnsResultData.Add(columnIndex, columnData);

        // Should Complete Spin?
        bool shouldComplete = true;
        foreach (var column in m_columns)
        {
            if (column.m_columnState == GameEntries.ColumnState.Idle)
                continue;

            shouldComplete = false;
        }

        if (shouldComplete)
        {
            var spinResult = GenerateSpinResultData(m_allColumnsResultData);
            GameEvents.SpinCompleted.Invoke(spinResult);

            return;
        }

        // Has SpeedUP?
        if (GameManager.Instance.requestedSpinResult.hasSpeedUp)
        {
            if (GameManager.Instance.requestedSpinResult.speedUpStartFrom == columnIndex)
            {
                for (int i = columnIndex + 1; i < m_columns.Length; i++)
                {
                    SpeedUpSlot(i);
                }
            }
        }

    }

    Dictionary<int, GameEntries.CurrentColumnData> m_allColumnsResultData = new Dictionary<int, GameEntries.CurrentColumnData>();
    GameEntries.CurrentRowsData GenerateSpinResultData(Dictionary<int, GameEntries.CurrentColumnData> allColumnsResultData)
    {
        GameEntries.CurrentRowsData rowsData = new GameEntries.CurrentRowsData();

        foreach (var columnData in m_allColumnsResultData)
        {
            rowsData.SetColumnSlots(columnData.Key, columnData.Value);
        }

        return rowsData;
    }
}
