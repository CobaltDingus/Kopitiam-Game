using TMPro;
using UnityEngine;

public class ReworkedUIManager : MonoBehaviour
{
    // ================================ REVAMP CODES ================================
    // [RULES]
    // [NOTE] Do Not Change Any Codes Here Unless Needed
    // - core variables
    // - functions for calculation and modifying said variables
    // - function starts with Capital
    // - variable starts with small
    // ================================ START ================================
    public static ReworkedUIManager instance { get; private set; }

    // ================================ VARIABLES ================================
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private TMP_Text _dayText;
    [SerializeField] private TMP_Text _customerCountText;
    [SerializeField] private TMP_Text _favourText;
    private bool _isTimerRunning;
    private bool _challengeMode;
    private float _timeElapsed;
    private float _timeRemaining;
    private float _duration = 120f;

    // ================================ GETTER & SETTER ================================

    public TMP_Text TimerText
    {
        get => _timerText;
    }

    public TMP_Text DayText
    {
        get => _dayText;
    }

    public TMP_Text CustomerCountText
    {
        get => _customerCountText;
    }

    public TMP_Text FavourText
    {
        get => _favourText;
    }

    public bool IsTimerRunning
    {
        get => _isTimerRunning;
        set => _isTimerRunning = value;
    }

    public bool ChallengeMode
    {
        get => _challengeMode;
        set => _challengeMode = value;
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

    void Update()
    {
        if (_challengeMode)
        {
            if (!_isTimerRunning) return;
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                UpdateTimerDisplayText(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
                UpdateTimerDisplayText(_timeRemaining);
                
            }
        }
        else
        {
            _timeElapsed += Time.deltaTime;

        }
    }

    // ================================ GENERAL FUNCTION ================================
    public void RestartTimer()
    {
        if (_challengeMode)
        {
            _timeRemaining = _duration;
            _isTimerRunning = true;
            UpdateTimerDisplayText(_timeRemaining);
        }
        else
        {
            _timeElapsed = 0f;
            _isTimerRunning = true;
            UpdateTimerDisplayText(_timeElapsed);
        }
    }

    private void UpdateTimerDisplayText(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (_timerText != null)
        {
            // true : false
            string text = _challengeMode ? "Time Reamining " : "Time Elapsed ";
            _timerText.text = text + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateDayDisplayText()
    {
        _dayText.text = "Day: " + ReworkedSaveManager.instance.Day;
    }

    public void UpdateCustomerDisplayText()
    {
        _customerCountText.text = "Customer: " + ReworkedSaveManager.instance.CurrentCustomer.ToString() + "/" + ReworkedSaveManager.instance.MaxCustomer.ToString();
    }

    public void UpdateFavourDisplayText()
    {
        _favourText.text = "Favour: " + ReworkedSaveManager.instance.Favour;
    }
}
