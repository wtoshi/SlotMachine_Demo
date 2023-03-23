using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Threading.Tasks;

public class Column : MonoBehaviour
{
    [SerializeField] Transform m_slotHolder;
    [SerializeField] int m_columnIndex;

    public AudioClip m_columnStopSound;

    public bool IsReady { get; private set; }
    public GameEntries.ColumnState m_columnState { get; private set; }

    AudioSource m_audiosource;
    Coroutine m_spinStopCoroutine;

    List<Slot> m_slots = new();

    float m_currentMoveSpeed;
    float m_spinStartTime;
    float m_spinTime;
    float m_columnSlotDistance = 200f;
    int m_spinCount = 0;

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        //if (m_columnIndex == 0)
        //{
        //    Debug.Log("currentState: " + m_columnState);
        //}

        switch (m_columnState)
        {
            case GameEntries.ColumnState.Idle:
                break;

            case GameEntries.ColumnState.SpinStart:
                UpdateOnStartState();
                break;

            case GameEntries.ColumnState.EaseIn:
                UpdateOnEaseInState();
                break;

            case GameEntries.ColumnState.Spin:
                UpdateOnSpinState();
                break;

            case GameEntries.ColumnState.SpinSpeedUp:
                UpdateOnSpinAccelState();
                break;

            case GameEntries.ColumnState.SpinToTargetReady:
                UpdateOnSpinToTargetReady();
                break;

            case GameEntries.ColumnState.SpinToTarget:
                UpdateOnSpinOnTargetState();
                break;

            case GameEntries.ColumnState.SpinEnd:
                UpdateOnSpinEndState();
                break;

            case GameEntries.ColumnState.EaseOut:
                UpdateOnEaseOutState();
                break;

            case GameEntries.ColumnState.Stop:
                UpdateOnStopState();
                break;
        }
    }

    #region STATE FUNCTIONS

    float currentTime = 0;

    void UpdateOnStartState()
    {
        ResetSpeed();

        if (GameManager.Instance.gameSettings.useEaseIn)
        {
            m_columnState = GameEntries.ColumnState.EaseIn;
        }
        else
        {
            m_columnState = GameEntries.ColumnState.Spin;
            m_spinStopCoroutine = StartCoroutine(StopColumnAfterDuration(m_spinTime));
        }

    }

    void UpdateOnSpinState()
    {
        MoveColumn();
    }

    void UpdateOnSpinAccelState()
    {
        m_currentMoveSpeed += (20 * Time.deltaTime);

        if (m_currentMoveSpeed >= 1.5f * m_currentMoveSpeed)
        {
            m_currentMoveSpeed = 1.5f * m_currentMoveSpeed;
        }

        MoveColumn();
    }

    void UpdateOnSpinToTargetReady()
    {
        MoveToSpinToTargetReady();
    }

    void UpdateOnSpinOnTargetState()
    {
        MoveColumnOTarget();
    }

    void UpdateOnSpinEndState()
    {

        if (GameManager.Instance.gameSettings.useEaseOut)
        {
            m_columnState = GameEntries.ColumnState.EaseOut;
        }
        else
        {
            m_columnState = GameEntries.ColumnState.Stop;
        }


        //m_audiosource.PlayOneShot(m_columnStopSound);     Son column ise 
    }

    float easedIn = 0;
    void UpdateOnEaseInState()
    {

        if (easedIn >= 50 || m_spinCount == 0)
        {
            m_columnState = GameEntries.ColumnState.Spin;
            m_spinStopCoroutine = StartCoroutine(StopColumnAfterDuration(m_spinTime));

            easedIn = 0;
            return;
        }

        float speedIn = GameManager.Instance.gameSettings.speedIn;
        var easeIn = new Vector2(0, Time.deltaTime * speedIn * 100);

        easedIn += easeIn.y;

        Vector2 pos = m_slotHolder.GetComponent<RectTransform>().anchoredPosition + easeIn;
        m_slotHolder.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    float easedOut = 0;
    void UpdateOnEaseOutState()
    {
        if (easedOut >= 50)
        {
            var desiredY = -(desiredIndexToStop * m_columnSlotDistance);
            Vector2 lastPos = m_slotHolder.GetComponent<RectTransform>().anchoredPosition;
            lastPos.y = desiredY;
            m_slotHolder.GetComponent<RectTransform>().anchoredPosition = lastPos;

            easedOut = 0f;
            m_columnState = GameEntries.ColumnState.Stop;
            return;
        }

        float speedOut = GameManager.Instance.gameSettings.speedOut;
        var easeOut = new Vector2(0, Time.deltaTime * speedOut * 100);

        easedOut += easeOut.y;

        Vector2 pos = m_slotHolder.GetComponent<RectTransform>().anchoredPosition + easeOut;
        m_slotHolder.GetComponent<RectTransform>().anchoredPosition = pos;

    }

    void UpdateOnStopState()
    {

        if (m_columnState != GameEntries.ColumnState.Stop)
            return;

        var columnData = GetColumnData();

        m_columnState = GameEntries.ColumnState.Idle;

        GameEvents.ColumnStopped.Invoke(m_columnIndex, columnData);
    }

    #endregion

    #region Public Methods

    public void PrepareForNextSpin()
    {
        ResetValues();

        if (m_spinCount > 0 && lastColumnData != null && lastColumnData.currentSlots.Count > 0)
        {
            m_slots.Clear();

            for (int i = 0; i < m_slotHolder.childCount; i++)
            {
                Destroy(m_slotHolder.GetChild(i).gameObject);
            }

            for (int i = lastColumnData.currentSlots.Count - 1; i >= 0; i--)
            {
                AddSlot(lastColumnData.currentSlots[i].slotData.slotResource);
            }
        }

        Vector2 pos = m_slotHolder.GetComponent<RectTransform>().anchoredPosition;
        pos.y = 0;
        m_slotHolder.GetComponent<RectTransform>().anchoredPosition = pos;


    }

    public void AddSlot(GameEntries.SlotResource slotResource)
    {
        var slot = Instantiate(ColumnController.Instance.SlotPF, m_slotHolder).GetComponent<Slot>();

        int slotIndex = m_slots.Count;
        slot.transform.SetAsFirstSibling();
        slot.Initialize(new GameEntries.SlotData(slotResource, slotIndex));

        m_slots.Add(slot);
    }

    public void ClearSlots()
    {
        for (int i = 0; i < m_slotHolder.childCount; i++)
        {
            Destroy(m_slotHolder.GetChild(i).gameObject);
        }
    }

    public void StartSpin(float spinTime)
    {
        if (m_columnState != GameEntries.ColumnState.Idle) return;

        //ResetValues();



        lastColumnData = null;

        t = 0;
        m_spinStartTime = Time.time;
        m_spinTime = spinTime;

        m_columnState = GameEntries.ColumnState.SpinStart;
    }

    public void StopSpin()
    {
        if (m_columnState == GameEntries.ColumnState.Spin)
        {
            StopCoroutine(m_spinStopCoroutine);
            m_audiosource.Stop();
            m_columnState = GameEntries.ColumnState.SpinToTargetReady;
        }
    }

    public void SpeedUpSlot()
    {
        Debug.Log("m_columnState: " + m_columnState);
        if (m_columnState != GameEntries.ColumnState.Spin && m_columnState != GameEntries.ColumnState.SpinToTargetReady && m_columnState != GameEntries.ColumnState.SpinToTarget) return;

        Debug.Log("active: " + m_columnState);
        var restTime = m_spinTime - (Time.time - m_spinStartTime);

        float extraTime = 2f;
        var delayedSpinTime = (restTime > 0 ? restTime : 0) + extraTime;

        if (m_spinStopCoroutine != null)
        {
            StopCoroutine(m_spinStopCoroutine);
            m_spinStopCoroutine = null;
        }

        m_columnState = GameEntries.ColumnState.SpinSpeedUp;

        m_spinStopCoroutine = StartCoroutine(StopColumnAfterDuration(delayedSpinTime));
    }

    #endregion 

    void Initialize()
    {        
        m_audiosource = GetComponent<AudioSource>();

        ClearSlots();

        ResetSpeed();

        m_columnState = GameEntries.ColumnState.Idle;
        IsReady = true;
    }

    void ResetSpeed()
    {
        m_currentMoveSpeed = GameManager.Instance.gameSettings.spinSpeed;
        //firstAdjustSlots = false;
    }

    void ResetValues()
    {
        hasClonedFirstSlots = false;
    }

    List<GameEntries.SlotResource> targetResources;
    public void SetTargetResources(List<GameEntries.SlotResource> resources)
    {
        targetResources = resources;
        SetDesiredStopIndex(0);
    }

    void SwitchToTargetSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            m_slots[i].SwitchResource(targetResources[i]);
        }

        targetResources = null;
    }

    void CloneFirstSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            AddSlot(m_slots[i].slotData.slotResource);
        }
    }

    float speedFactor = 100;
    float currentSpeedFactor = 0;
    float t = 0;
    bool hasClonedFirstSlots;
    void MoveColumn()
    {
        t += 3f * Time.deltaTime;
        currentSpeedFactor = Mathf.Lerp(0, speedFactor, t);

        Vector2 pos = m_slotHolder.GetComponent<RectTransform>().anchoredPosition - new Vector2(0, Time.deltaTime * m_currentMoveSpeed * currentSpeedFactor);

        if (pos.y <= -600)
        {
            if (targetResources != null)
            {
                SwitchToTargetSlots();

                targetResources = null;
            }

            if (!hasClonedFirstSlots)
            {
                CloneFirstSlots();
                hasClonedFirstSlots = true;
            }


        }

        if (pos.y <= -m_columnSlotDistance * (m_slots.Count - 3))
        {
            m_spinCount++;

            pos.y = 0;
        }

        m_slotHolder.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    void MoveToSpinToTargetReady()
    {
        Vector2 pos = m_slotHolder.GetComponent<RectTransform>().anchoredPosition - new Vector2(0, Time.deltaTime * m_currentMoveSpeed * speedFactor);

        if (pos.y <= -m_columnSlotDistance * (m_slots.Count - 3))
        {
            m_spinCount++;

            pos.y = 0;
        }

        m_slotHolder.GetComponent<RectTransform>().anchoredPosition = pos;

        if (ColumnController.Instance.CanColumnEnterLastTurn(m_columnIndex))
        {
            if (pos.y == 0)
            {
                t = 0f;
                m_columnState = GameEntries.ColumnState.SpinToTarget;
            }
        }
    }

    void MoveColumnOTarget()
    {
        Vector2 pos = m_slotHolder.GetComponent<RectTransform>().anchoredPosition - new Vector2(0, Time.deltaTime * m_currentMoveSpeed * speedFactor);

        m_slotHolder.GetComponent<RectTransform>().anchoredPosition = pos;

        bool usingEaseOut = GameManager.Instance.gameSettings.useEaseOut;
        var desiredY = -(desiredIndexToStop * m_columnSlotDistance) + (usingEaseOut ? -50f : 0);

        if (pos.y <= desiredY)
        {

            m_columnState = GameEntries.ColumnState.SpinEnd;
        }
    }

    int desiredIndexToStop = 5;
    void SetDesiredStopIndex(int index = -1)
    {
        if (index == -1)
            desiredIndexToStop = Random.Range(0, m_slots.Count - 2);
        else
            desiredIndexToStop = index;

    }

    IEnumerator StopColumnAfterDuration(float spinTime)
    {
        yield return new WaitForSeconds(spinTime);

        m_columnState = GameEntries.ColumnState.SpinToTargetReady;
    }

    GameEntries.CurrentColumnData lastColumnData = null;
    GameEntries.CurrentColumnData GetColumnData()
    {
        lastColumnData = new GameEntries.CurrentColumnData(m_slots[desiredIndexToStop + 2], m_slots[desiredIndexToStop + 1], m_slots[desiredIndexToStop]);

        return lastColumnData;
    }
}
