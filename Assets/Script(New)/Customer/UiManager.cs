using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static UiManager uiManager { get; private set; }
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text customerCountText;

    [SerializeField] private TMP_Text favorText;

    // Timer Handling
    [SerializeField] private bool challengeMode;
    private bool isTimerRunning;

    private float timeElapsed;
    private float timeRemaining;
    [SerializeField] private float duration = 20f;

    // base var for saves


    public bool ChallengeMode => challengeMode;

    public bool timerStatus => isTimerRunning;

    public void HideFavour()
    {
        favorText.gameObject.SetActive(false);
    }

    public void ShowFavour()
    {
        favorText.gameObject.SetActive(true);
    }

    public void HideCustomerCount()
    {
        customerCountText.gameObject.SetActive(false);
    }

    public void ShowCustomerCount()
    {
        customerCountText.gameObject.SetActive(true);
    }

    public void HideTimer()
    {
        timerText.gameObject.SetActive(false);
    }

    public void ShowTimer()
    {
        timerText.gameObject.SetActive(true);
    }

    public void TurnOffTimer()
    {
        isTimerRunning = false;
    }

    public void TurnOnTimer()
    {
        isTimerRunning = true;
    }
    void Awake()
    {
        if (uiManager != null && uiManager != this)
        {
            Destroy(gameObject);
            return;
        }

        uiManager = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        RestartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isTimerRunning) return;
        if (challengeMode)
        {
            if (!isTimerRunning) return;
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplayCountDown(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerDisplayCountDown(timeRemaining);
                // loops back the timer, can remove the RestartTimer() here if want to add an outcome if timer ended
                CustomerManager.Instance.GenerateNewCustomer();
                RestartTimer();
            }
        }
        else
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerDisplayCountUp(timeElapsed);
        }
    }

    private void UpdateTimerDisplayCountUp(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timerText != null)
        {
            string word = "Time Elapsed ";
            timerText.text = word + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateTimerDisplayCountDown(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timerText != null)
        {
            string text = "Time Remaining ";
            timerText.text = text + string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }


    public void RestartTimer()
    {
        timeElapsed = 0f;
        isTimerRunning = true;
        UpdateTimerDisplayCountUp(timeElapsed);
        if (challengeMode)
        {
            timeRemaining = duration;
            isTimerRunning = true;
            UpdateTimerDisplayCountDown(timeRemaining);
        }
        else
        {
            timeElapsed = 0f;
            isTimerRunning = true;
            UpdateTimerDisplayCountUp(timeElapsed);
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void UpdateDayCount()
    {
        
        if (timerText != null)
        {
            string text = "DAY: ";
            dayText.text = text + SaveManager.saveManager.DayCount;
        }
    }

    public void UpdateCustomerCount()
    {
        if (customerCountText != null)
        {
            customerCountText.text = "CUSTOMER: " + SaveManager.saveManager.CurrentCustomerCount.ToString() + "/" + SaveManager.saveManager.MaxCustomerCount.ToString();
        }
    }

    public void UpdateFavor()
    {

        if (favorText != null)
        {
            favorText.text = "Favour: " + SaveManager.saveManager.Favour;
        }

    }
}
