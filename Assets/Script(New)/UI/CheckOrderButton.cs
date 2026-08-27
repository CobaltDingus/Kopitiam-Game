using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckOrderButton : MonoBehaviour
{
    [SerializeField] private UIPanel UIPanel;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private string normalText;
    [SerializeField] private string closeText;
    private TextMeshProUGUI UIText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIText = UIPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCurrentOrder()
    {
        UIPanel.TogglePanel();
        buttonText.text = UIPanel.gameObject.activeSelf ? closeText : normalText;
        if (CustomerManager.Instance != null)
        {
            UIText.text = CustomerManager.Instance.CurrentDialogueText;
            Debug.Log(CustomerManager.Instance.CurrentDialogueText);   
        }
        else
        {
            Debug.Log("No CustomerManager found");
        }
    }
}
