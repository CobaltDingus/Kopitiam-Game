using UnityEngine;

public class CounterCanvas : MonoBehaviour
{
    [SerializeField] private SummaryPanel summaryPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        SaveManager.OnDayEnd += ShowSummary;
    }

    void OnDisable()
    {
        SaveManager.OnDayEnd -= ShowSummary;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowSummary()
    {
        summaryPanel.ShowSummary();
    }
}
