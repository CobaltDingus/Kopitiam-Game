using System;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private float currentCustomerCount = 0;

    private float maxCustomerCount;
    private int dayCount;

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

    // Progression stage stuff
    public static event Action<int> OnDayChanged;

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
        // dayCount++;
        // OnDayChanged?.Invoke(dayCount);
    }

    private void CheckCustomerCountLimit()
    {
        if (currentCustomerCount > maxCustomerCount)
        {
            currentCustomerCount = 0;
            dayCount += 1;
            resetFavor();
            UiManager.uiManager.UpdateFavor();
            UiManager.uiManager.RestartTimer();
            UiManager.uiManager.UpdateDayCount();
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
    private float _day;
    private float _currentCustomer;
    private float _maxCustomer;
    private float _favour = 0;

    
    // ================================ GETTER & SETTER ================================
    public float Day
    {
        get => _day;
        set => _day = value;
    }
    public float CurrentCustomer
    {
        get => _currentCustomer;
        set => _currentCustomer = value;
    }
    public float MaxCustomer
    {
        get => _maxCustomer;
        set => _maxCustomer = value;
    }
    public float Favour
    {
        get => _favour;
        set => _favour = value;
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
    
}
