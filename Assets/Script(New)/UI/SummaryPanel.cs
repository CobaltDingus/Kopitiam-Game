using TMPro;
using UnityEngine;

public class SummaryPanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI dayCompleteTMP;
    [SerializeField] private TextMeshProUGUI customersServedTMP;
    [SerializeField] private TextMeshProUGUI correctServedTMP;
    [SerializeField] private TextMeshProUGUI partialServedTMP;
    [SerializeField] private TextMeshProUGUI wrongServedTMP;
    [SerializeField] private TextMeshProUGUI favourEarnedTMP;

    // [SerializeField] private SaveManager saveManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        // SaveManager.OnDayEnd += ShowSummary;
    }

    void OnDisable()
    {
        // SaveManager.OnDayEnd -= ShowSummary;   
    }
    void Start()
    {
        // saveManager = SaveManager.saveManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSummary()
    {
        Debug.Log("show summary");
        UpdateText();
        OpenPanel();
    }

    public void UpdateText()
    {
        int diff = FavourDiff();
        string sign = diff > 0 ? "+" : "";

        Debug.Log(SaveManager.saveManager.DayCount);
        dayCompleteTMP.text = "Day " + SaveManager.saveManager.DayCount + " Complete!";
        customersServedTMP.text = "Customers Served: " + SaveManager.saveManager.MaxCustomerCount.ToString();
        // Change customer count to total drink counts across all orders when that is made
        correctServedTMP.text = "Correct Orders: " + SaveManager.saveManager.CorrectOrders + " / " + SaveManager.saveManager.MaxCustomerCount;
        partialServedTMP.text = "Partially Correct Orders: " + SaveManager.saveManager.PartialOrders + " / " + SaveManager.saveManager.MaxCustomerCount;
        wrongServedTMP.text = "Wrong Orders: " + SaveManager.saveManager.WrongOrders + " / " + SaveManager.saveManager.MaxCustomerCount;
        favourEarnedTMP.text = "Favour Earned: " + SaveManager.saveManager.Favour.ToString() + " (" + sign + diff + ")";
    }

    

    public int FavourDiff()
    {
        return SaveManager.saveManager.Favour - SaveManager.saveManager.LastEarnedFavour;
    }

    public void ProceedNextDay()
    {
        ClosePanel();
        SaveManager.saveManager.StartNextDay();
    }
}
