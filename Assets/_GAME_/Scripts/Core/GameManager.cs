using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    public GameSettings gameSettings;
    public RequestedSpinResult requestedSpinResult;

    public ColumnController m_columns;

    [SerializeField] WinCheckController m_winChecker;

    public WinCheckController WinChecker => m_winChecker;

    GameEntries.GameState m_currentGameState;
    Coroutine m_mainStateCoroutine;

    GameEntries.CurrentRowsData currentRowsData;    // Win Check için gelen datayý kontrol!

    void Start()
    {
        Initialize();
        ResetValues();

        m_currentGameState = GameEntries.GameState.Ready;
    }

    private void OnEnable()
    {
        GameEvents.SpinCompleted.AddListener(OnSpinCompleted);
    }

    private void OnDisable()
    {
        GameEvents.SpinCompleted.RemoveListener(OnSpinCompleted);
    }

    void Update()
    {
        switch (m_currentGameState)
        {
            case GameEntries.GameState.Ready:
                OnReadyState();
                break;
            case GameEntries.GameState.Spin:
                OnSpinState();
                break;
            case GameEntries.GameState.Stop:
                OnStopState();
                break;
            case GameEntries.GameState.TurnEnd:
                OnTurnEndState();
                break;
        }
    }

    void Initialize()
    {
        m_currentGameState = GameEntries.GameState.Ready;
    }

    void ResetValues()
    {
        currentRowsData = new GameEntries.CurrentRowsData();
        if (m_mainStateCoroutine != null) StopCoroutine(m_mainStateCoroutine);
        m_mainStateCoroutine = null;
    }

    #region STATE METHODS


    void OnReadyState()
    {
        UIManager.Instance.SetSpinButtonOn(true);
    }
    void OnSpinState()
    {
        //Buttona basýp durdurma yapabilirsin
    }
    void OnStopState()
    {
        //Check Win Controller?
        m_currentGameState = GameEntries.GameState.Ready;
    }

    void OnTurnEndState()
    {
        m_currentGameState = GameEntries.GameState.Ready;
    }


    #endregion

    #region Public Method
    public void StartGame()
    {
        if (m_currentGameState == GameEntries.GameState.Ready) 
        {

            ResetValues();

            UIManager.Instance.SetSpinButtonOn(false);

            m_columns.StartSpin();

            m_currentGameState = GameEntries.GameState.Spin;

        }

    }

    public void StopSpin()
    {
        if (m_currentGameState == GameEntries.GameState.Spin)
        {
            m_columns.StopSpin();
        }
    }


    #endregion Public Method

    #region ACTIONS
    void OnSpinCompleted(GameEntries.CurrentRowsData currentRowsData)
    {
        Debug.Log("SPIN COMPLETED!");
        this.currentRowsData = currentRowsData;
        if (m_currentGameState == GameEntries.GameState.Spin) m_currentGameState = GameEntries.GameState.Stop;
    }
    #endregion
}
