using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private float currentCustomerCount = 0;

    private float maxCustomerCount = 1;
    private float dayCount;

    //private float baseCustomerCount = 5;

    private float favor = 0;

    //private float previousFavor;

    //private float tutorialPhase;

    private bool retry;
    // getters
    public float Favor => favor;
    public float DayCount => dayCount;
    public float MaxCustomerCount => maxCustomerCount;

    //public float TutorialPhase => tutorialPhase;

    public float CurrentCustomerCount => currentCustomerCount;

    public bool Retry => retry;
    //setters
    //public void setTutorialPhase(int num)
    //{
    //    tutorialPhase += num;
    //}

    public void setCurrentCustomerCount(int num)
    {

    }


    public static SaveManager saveManager { get; private set; }


    void Start()
    {
        if (saveManager != null && saveManager != this)
        {
            Destroy(gameObject);
            return;
        }

        saveManager = this;
        DontDestroyOnLoad(gameObject);
        dayCount = 0;
        SetAvailableRecipes(2);

        // SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void setDayCount(int num)
    {
        dayCount += num;
    }

    // Increments customer count and checks for day completion
    public void IncrementCustomerCount()
    {
        currentCustomerCount += 1;
        CheckCustomerCountLimit();
    }

    private void CheckCustomerCountLimit()
    {
        if (currentCustomerCount > maxCustomerCount)
        {
            Debug.Log("check customer count limit");
            EndCurrentDay();
            // currentCustomerCount = 0;
            // dayCount += 1;
            // // if (favor > lastEarnedFavour)
            // // {
            // //     maxCustomerCount++;
            // // }
            // // lastEarnedFavour = _favour;
            // resetFavor();
            // UiManager.uiManager.UpdateFavor();
            // UiManager.uiManager.RestartTimer();
            // UiManager.uiManager.UpdateDayCount();

            // if (dayCount == 2)
            // {
            //     SetAvailableRecipes(6);
            //     maxCustomerCount = 5;
            // }
        }
    }
    public void setFavour (int num)
    {
        Favour += num;
    }

    public void resetFavor()
    {
        _favour = 0;
    }

    // ================================ REVAMP CODES ================================
    // [RULES]
    // [NOTE] Do Not Change Any Codes Here Unless Needed
    // - core variables
    // - functions for calculation and modifying said variables
    // - function starts with Capital
    // - variable starts with small
    // ================================ START ================================

    // ================================ VARIABLES ================================
    private int _day;
    private int _currentCustomer;
    private int _maxCustomer;
    private int _favour = 0;
    private int _lastEarnedFavour = 0;
    private int _correctOrders;
    private int _partialOrders;
    private int _wrongOrders;

    
    // ================================ GETTER & SETTER ================================
    public int Day
    {
        get => _day;
        set => _day = value;
    }
    public int CurrentCustomer
    {
        get => _currentCustomer;
        set => _currentCustomer = value;
    }
    public int MaxCustomer
    {
        get => _maxCustomer;
        set => _maxCustomer = value;
    }
    public int Favour
    {
        get => _favour;
        set => _favour = value;
    }

    public int CorrectOrders
    {
        get => _correctOrders;
        set => _correctOrders = value;
    }

    public int PartialOrders
    {
        get => _partialOrders;
        set => _partialOrders = value;
    }

    public int WrongOrders
    {
        get => _wrongOrders;
        set => _wrongOrders = value;
    }

    public int LastEarnedFavour
    {
        get => _lastEarnedFavour;
        set => _lastEarnedFavour = value;
    }

    // ================================ GENERAL FUNCTION ================================
    public void EvaluateAndCalculateDay()
    {
        if (CurrentCustomer == MaxCustomer)
        {
            CurrentCustomer = 0;
            Day += 1;
            return;
        }

    }
    
    // ================================ JF TEST FUNCTION ================================
    public void SetAvailableRecipes(int num)
    {
        CustomerManager.Instance.SetAvailableRecipes(num);
    }
    public static event Action OnDayEnd;
    // [SerializeField] private SummaryPanel summaryPanel;

    public void EndCurrentDay()
    {
        Debug.Log("EndDay");
        LastEarnedFavour = Favour;
        OnDayEnd?.Invoke();
        // summaryPanel.ShowSummary();
        currentCustomerCount = 0;
        dayCount += 1;
        // if (favor > lastEarnedFavour)
        // {
        //     maxCustomerCount++;
        // }
        // lastEarnedFavour = _favour;
        resetFavor();
        UiManager.uiManager.UpdateFavor();
        UiManager.uiManager.RestartTimer();
        UiManager.uiManager.UpdateDayCount();

        if (dayCount == 2)
        {
            SetAvailableRecipes(6);
            maxCustomerCount = 5;
        }

        UiManager.uiManager.IsTimerRunning = false;
    }

    public void StartNextDay()
    {
        UiManager.uiManager.IsTimerRunning = true;
    }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if (scene.name == "CounterScene")
    //     {
    //         FindSummaryPanel();
    //     }
    // }

    // void FindSummaryPanel()
    // {
    //     summaryPanel = GameObject.Find("NormalDaySummary").GetComponent<SummaryPanel>();
    // }
}
