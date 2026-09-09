using UnityEngine;

public class CounterCanvas : MonoBehaviour
{
    [SerializeField] private SummaryPanel summaryPanel;
    [SerializeField] private UIPanel noteOne;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnEnable()
    {
        SaveManager.OnDayEnd += ShowSummary;
        SaveManager.OnDayStart += ShowNote;
    }

    void OnDisable()
    {
        SaveManager.OnDayEnd -= ShowSummary;
        SaveManager.OnDayStart -= ShowNote;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowSummary()
    {
        summaryPanel.ShowSummary();
    }

    public void ShowNote(int day)
    {
        if (day == 2)
        {
            noteOne.OpenPanel();
        }
    }
}
