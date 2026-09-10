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
        Debug.Log(ReworkedSaveManager.instance.Day);
        dayCompleteTMP.text = "Day " + ReworkedSaveManager.instance.Day + " Complete!";
        customersServedTMP.text = "Customers Served: " + ReworkedSaveManager.instance.MaxCustomer.ToString();
        // Change customer count to total drink counts across all orders when that is made
        correctServedTMP.text = "Correct Orders: " + ReworkedSaveManager.instance.CorrectOrders + " / " + ReworkedSaveManager.instance.MaxCustomer;
        partialServedTMP.text = "Partially Correct Orders: " + ReworkedSaveManager.instance.PartialOrders + " / " + ReworkedSaveManager.instance.MaxCustomer;
        wrongServedTMP.text = "Wrong Orders: " + ReworkedSaveManager.instance.WrongOrders + " / " + ReworkedSaveManager.instance.MaxCustomer;
        favourEarnedTMP.text = "Favour Earned: " + ReworkedSaveManager.instance.PreviousFavour.ToString();
    }

    public void ProceedNextDay()
    {
        ClosePanel();
        ReworkedSaveManager.instance.StartNextDay();
    }
}
