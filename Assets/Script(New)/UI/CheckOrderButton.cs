using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CheckOrderButton : MonoBehaviour
{
    [SerializeField] private UIPanel UIPanel;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private string normalText;
    [SerializeField] private string closeText;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite greenSprite;
    [SerializeField] private Sprite redSprite;
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
        if (ReworkedCustomerManager.instance != null)
        {
            UIText.text = string.Join(
                "\n",
                ReworkedCustomerManager.instance.OrderedRecipes
                    .Select((recipe, index) => $"{index + 1}. {recipe.drinkName}")
            );
        }
        else
        {
            Debug.Log("No CustomerManager found");
        }
    }

    public void TogglePanel()
    {
        UIPanel.TogglePanel();
        GetCurrentOrder();
        buttonText.text = UIPanel.gameObject.activeSelf ? closeText : normalText;
        buttonImage.sprite = UIPanel.gameObject.activeSelf ? redSprite : greenSprite;
    }
}
