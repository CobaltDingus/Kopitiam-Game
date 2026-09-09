using UnityEngine;

public class ReworkedSaveManager : MonoBehaviour
{
    // ================================ REVAMP CODES ================================
    // [RULES]
    // [NOTE] Do Not Change Any Codes Here Unless Needed
    // - core variables
    // - functions for calculation and modifying said variables
    // - function starts with Capital
    // - variable starts with small
    // ================================ START ================================
    public static ReworkedSaveManager instance { get; private set; }
    // ================================ VARIABLES ================================
    private int _day;
    private int _currentCustomer;
    private int _maxCustomer;
    private int _favour = 0;
    private int _previousFavour;
    private int _maxEvent = 1;
    private int _currentEvent;
    private int _tutorialPhase;
    private int _previousIndex;

    //private float lastEarnedFavour = 0;

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

    // ================================ GENERAL FUNCTION ================================
    public void EvaluateAndCalculateDay()
    {
        if (CurrentCustomer == MaxCustomer)
        {
            CurrentCustomer = 0;
            Day += 1;

            EvaluateAndCalculateFavour();

            ReworkedUIManager.instance.UpdateFavourDisplayText();
            ReworkedUIManager.instance.RestartTimer();
            ReworkedUIManager.instance.UpdateDayDisplayText();

            if (Day == 2)
            {
                SetAvailableRecipes(6);
                MaxCustomer = 5;
            }
            //return;
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
                if (remainder == 0)
                {
                    _maxCustomer += 1;
                }
                else
                {
                    _maxCustomer += remainder;
                }
            }
            else if (_favour < _previousFavour)
            {
                // simple version
                _maxCustomer = 5;

                //complex (start)
                int diff = (int)_previousFavour - (int)_favour;
                int remainder = diff / 100;
                if (remainder == 0)
                {
                    _maxCustomer -= 1;
                }
                else
                {
                    if (remainder > _maxCustomer)
                    {
                        _maxCustomer = 5;
                    }
                    else if (_maxCustomer - remainder < 5)
                    {
                        _maxCustomer = 5;
                    }
                    else
                    {
                        _maxCustomer -= remainder;
                    }
                }
                //complex (end)
            }
        }
        _previousFavour = _favour;
        _favour = 0;

    }

    // public static event Action<int> OnDayTwo; 
    // ================================ JF TEST FUNCTION ================================
    public void SetAvailableRecipes(int num)
    {
        CustomerManager.Instance.SetAvailableRecipes(num);
    }
}
