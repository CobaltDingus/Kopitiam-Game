using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private float currentCustomerCount;

    private float maxCustomerCount;
    private float dayCount;

    private float baseCustomerCount;

    private float favor;

    private float previousFavor;

    //private float tutorialPhase;

    private bool retry;
    // getters
    public float DayCount => dayCount;
    public float MaxCustomerCount => maxCustomerCount;

    //public float TutorialPhase => tutorialPhase;

    public float CurrentCustomerCount => currentCustomerCount;

    public bool Retry => retry;
    //setters
    public void setDayCount(int num)
    {
        dayCount += num;
        return;
    }
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
    void Update()
    {
        
    }

    private void NextDay()
    {

    }

    private void CalculateFavor()
    {

    }


}
