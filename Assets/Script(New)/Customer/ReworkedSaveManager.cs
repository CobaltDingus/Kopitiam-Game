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
    private int _favour = 0;
    private int _previousFavour;
    private int _maxEvent = 1;
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
        if (_previousFavour == 0)
        {
            _maxCustomer = 5;
        }
        else
        {
            if (_favour > _previousFavour)
            {
                float diff = _favour - _previousFavour;
                int remainder = (int)diff / 100;
                _maxCustomer += (remainder == 0) ? 1 : remainder;
            }
            else if (_favour < _previousFavour)
            {
                int diff = _previousFavour - _favour;
                int remainder = diff / 100;
                if (remainder == 0)
                {
                    _maxCustomer -= 1;
                }
                else
                {
                    if (remainder > _maxCustomer || _maxCustomer - remainder < 5)
                    {
                        _maxCustomer = 5;
                    }
                    else
                    {
                        _maxCustomer -= remainder;
                    }
                }
            }
        }
        _previousFavour = _favour;
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

        // Calculate favour scaling before clearing variables
        EvaluateAndCalculateFavour();

        _currentCustomer = 0;
        Day += 1;

        if (Day >= 2)
        {
            SetAvailableRecipes(6);
        }
        else
        {
            SetAvailableRecipes(2);
        }

        if (ReworkedUIManager.instance != null)
        {
            ReworkedUIManager.instance.UpdateFavourDisplayText();
            ReworkedUIManager.instance.RestartTimer();
            ReworkedUIManager.instance.UpdateDayDisplayText();
            ReworkedUIManager.instance.UpdateCustomerDisplayText();
            ReworkedUIManager.instance.IsTimerRunning = false;
        }

        // Triggers Day Summary Panel / events
        OnDayEnd?.Invoke();
    }

    public void StartNextDay()
    {
        // Reset order trackers for the new day
        CorrectOrders = 0;
        PartialOrders = 0;
        WrongOrders = 0;

        if (ReworkedUIManager.instance != null)
        {
            ReworkedUIManager.instance.IsTimerRunning = true;
        }

        OnDayStart?.Invoke(Day);

        // Generate the first customer of the new day
        if (ReworkedCustomerManager.instance != null)
        {
            ReworkedCustomerManager.instance.GenerateCustomer();
        }
    }
}