using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private GameObject _topBar;
    [SerializeField] private GameObject _dayGroup;
    [SerializeField] private GameObject _customerGroup;
    [SerializeField] private GameObject _timerGroup;
    [SerializeField] private GameObject _favourGroup;
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

    public GameObject DayGroup => _dayGroup;
    public GameObject CustomerGroup => _customerGroup;
    public GameObject TimerGroup => _timerGroup;
    public GameObject FavourGroup => _favourGroup;

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
    void Start()
    {
        RestartTimer();
    }

    private Camera _mainCamera;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _mainCamera = Camera.main;

        GetComponent<Canvas>().worldCamera = _mainCamera;
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

                if (ReworkedCustomerManager.instance != null)
                {
                    ReworkedCustomerManager.instance.GenerateCustomer();
                    RestartTimer();
                }
            }
        }
        else
        {
            if (!_isTimerRunning) return;
            _timeElapsed += Time.deltaTime;
            UpdateTimerDisplayText(_timeElapsed);
        }
    }

    // ================================ GENERAL FUNCTION ================================
    public void RestartTimer()
    {
        _isTimerRunning = true;
        if (_challengeMode)
        {
            _timeRemaining = _duration;
            UpdateTimerDisplayText(_timeRemaining);
        }
        else
        {
            _timeElapsed = 0f;
            UpdateTimerDisplayText(_timeElapsed);
        }
    }

    private void UpdateTimerDisplayText(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (_timerText != null)
        {
            string text = _challengeMode ? "Time Remaining " : "Time Spent ";
            _timerText.text = text + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateDayDisplayText()
    {
        if (_dayText != null && ReworkedSaveManager.instance != null)
            _dayText.text = "Day: " + ReworkedSaveManager.instance.Day;
    }

    public void UpdateCustomerDisplayText()
    {
        if (_customerCountText != null && ReworkedSaveManager.instance != null)
            _customerCountText.text = "Customer: " + ReworkedSaveManager.instance.CurrentCustomer.ToString() + "/" + ReworkedSaveManager.instance.MaxCustomer.ToString();
    }

    public void UpdateFavourDisplayText()
    {
        if (_favourText != null && ReworkedSaveManager.instance != null)
            _favourText.text = "Favour: " + ReworkedSaveManager.instance.Favour;
    }
}