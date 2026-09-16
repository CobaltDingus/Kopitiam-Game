using System;
using UnityEngine;

public class ReworkedSaveManager : MonoBehaviour
{
    // ================================ REVAMP CODES ================================
    public static ReworkedSaveManager instance { get; private set; }

    // ================================ VARIABLES ================================
    private int _day;
    private int _currentCustomer;
    private int _maxCustomer = 3;
    private int _customerMin = 3;
    private int _customerCap = 20;
    private int _currentFavourPercentage = 0;
    private int _favour = 0;
    private int _previousFavourPercentage = 0;
    private int _previousFavour;
    private int _maxEvent = 0;
    private int _currentEvent;
    private int _tutorialPhase = 0;
    private int _previousIndex;
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

    public int PreviousFavour
    {
        get => _previousFavour;
        set => _previousFavour = value;
    }

    public int MaxEvent
    {
        get => _maxEvent;
        set => _maxEvent = value;
    }

    public int CurrentEvent
    {
        get => _currentEvent;
        set => _currentEvent = value;
    }

    public int TutorialPhase
    {
        get => _tutorialPhase;
        set => _tutorialPhase = value;
    }

    public int PreviousIndex
    {
        get => _previousIndex;
        set => _previousIndex = value;
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

    // ================================ AWAKE START UPDATE ================================
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _tutorialPhase = 0;
        Day = 0;
        CurrentEvent = MaxEvent;
        SetAvailableRecipes(2);
    }

    // ================================ GENERAL FUNCTION ================================
    public void EvaluateAndCalculateDay()
    {
        if (CurrentCustomer >= MaxCustomer)
        {
            EndCurrentDay();
        }
    }

    public void EvaluateAndCalculateFavour()
    {
        int total = _maxCustomer * 100;
        _currentFavourPercentage = (int)((_favour / total) * 100f);

        if (_currentFavourPercentage > _previousFavourPercentage)
        {
            int diff = _currentFavourPercentage - _previousFavourPercentage;
            if (diff > 50)
            {
                _maxCustomer += 2;
                if (_maxCustomer > _customerCap)
                {
                    _maxCustomer = _customerCap;
                }
                int totale = (int)_maxCustomer / _customerMin;
                _maxEvent = totale;
            }
            else
            {
                _maxCustomer += 1;
                if (_maxCustomer > _customerCap)
                {
                    _maxCustomer = _customerCap;
                }
                int totale = (int)_maxCustomer / _customerMin;
                _maxEvent = totale;
            }
        }
        else if(_currentFavourPercentage == _previousFavourPercentage)
        {
            if(_maxCustomer == _customerCap)
            {
                _maxCustomer = _customerCap;
            }
            // if same but maxCustomer is not capped then remain the same maxCustomer
        }
        else
        {
            int diff = _previousFavourPercentage - _currentFavourPercentage;
            if (diff > 50)
            {
                _maxCustomer -= 2;
                if (_maxCustomer < _customerMin)
                {
                    _maxCustomer = _customerMin;
                }
                _maxEvent = 0;
            }
            else
            {
                _maxCustomer -= 1;
                if (_maxCustomer < _customerMin)
                {
                    _maxCustomer = _customerMin;
                }
                _maxEvent = 0;
            }
        }

        //if (_day == 1)
        //{
        //    _maxCustomer = 5;
        //}
        //else
        //{
        //    // old
        //    if (_favour > _previousFavour)
        //    {
        //        int diff = _favour - _previousFavour;
        //        int remainder = diff / 100;
        //        _maxCustomer += (remainder == 0) ? 1 : remainder;
        //    }
        //    else if (_favour < _previousFavour)
        //    {

        //        int diff = _previousFavour - _favour;
        //        int remainder = diff / 100;
        //        if (remainder == 0)
        //        {
        //            _maxCustomer -= 1;
        //        }
        //        else
        //        {
        //            if (remainder > _maxCustomer || _maxCustomer - remainder < 5)
        //            {
        //                _maxCustomer = 5;
        //            }
        //            else
        //            {
        //                _maxCustomer -= remainder;
        //            }
        //        }
        //    }
        //}
        _previousFavour = _favour;
        _previousFavourPercentage = _currentFavourPercentage;
        _currentFavourPercentage = 0;
        _favour = 0;
    }

    // ================================ JF TEST FUNCTION ================================
    public void SetAvailableRecipes(int num)
    {
        if (ReworkedCustomerManager.instance != null)
        {
            ReworkedCustomerManager.instance.SetAvailableRecipes(num);
        }
    }

    public static event Action OnDayEnd;
    public static event Action<int> OnDayStart;

    public void EndCurrentDay()
    {
        Debug.Log("EndDay");

        if (ReworkedUIManager.instance != null)
        {
            ReworkedUIManager.instance.IsTimerRunning = false;
        }

        // Opens Summary Panel while Day, MaxCustomer, and Favour are still set to the completed day's values
        OnDayEnd?.Invoke();
    }

    public void StartNextDay()
    {
        // Calculate favour & customer count for the NEW day
        EvaluateAndCalculateFavour();

        // Transition to the next day
        Day += 1;
        _currentCustomer = 0;

        // Configure recipes and events based on current Day
        if (Day >= 2)
        {
            SetAvailableRecipes(10);
            if (MaxEvent == 0)
            {
                MaxEvent = 1; // Day 2+ allows 1 event customer
            }
        }
        else
        {
            SetAvailableRecipes(2);
            MaxEvent = 0; // Day 1 allows 0 event customers
        }

        // Reset remaining events for the new day
        CurrentEvent = MaxEvent;

        // Reset daily order trackers
        CorrectOrders = 0;
        PartialOrders = 0;
        WrongOrders = 0;

        if (ReworkedUIManager.instance != null)
        {
            ReworkedUIManager.instance.UpdateFavourDisplayText();
            ReworkedUIManager.instance.UpdateDayDisplayText();
            ReworkedUIManager.instance.UpdateCustomerDisplayText();
            ReworkedUIManager.instance.RestartTimer();
        }

        OnDayStart?.Invoke(Day);

        // Generate the first customer of the new day
        if (ReworkedCustomerManager.instance != null)
        {
            ReworkedCustomerManager.instance.GenerateCustomer();
        }
    }
}