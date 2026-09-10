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
        ReworkedSaveManager.OnDayEnd += ShowSummary;
        ReworkedSaveManager.OnDayStart += ShowNote;
    }

    void OnDisable()
    {
        ReworkedSaveManager.OnDayEnd -= ShowSummary;
        ReworkedSaveManager.OnDayStart -= ShowNote;    
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
