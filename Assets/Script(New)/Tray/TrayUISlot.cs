using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrayUISlot : MonoBehaviour
{
    public enum SlotNum
    {
        SlotOne,
        SlotTwo,
        SlotThree
    }

    [SerializeField] private TrayDatabase trayDatabase;
    [SerializeField] private SlotNum slotNum;
    [SerializeField] private Image cupImage;
    [SerializeField] private Image liquidImage;
    [SerializeField] private TraySlotDrinkContents traySlotDrinkContents;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Button disposeButton;
    [SerializeField] private TraySlotRowContents rowContentsPrefab;
    [SerializeField] private TrayStorageUI trayStorageUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ResetSlot();
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSlot()
    {
        if (trayDatabase.SavedDrinks.Count > (int)slotNum)
        {
            // ResetSlot();
            contentText.gameObject.SetActive(false);
            disposeButton.image.color = Color.red;
            disposeButton.enabled = true;
            cupImage.gameObject.SetActive(true);
            liquidImage.color = GlobalUtilities.HexToColor(trayDatabase.SavedDrinks[(int)slotNum].colorHex);
            liquidImage.gameObject.SetActive(true);

            foreach (Transform child in traySlotDrinkContents.transform) 
                Destroy(child.gameObject);

            foreach (Ingredient ing in trayDatabase.SavedDrinks[(int)slotNum].ingredients)
            {
                if (ing == null || ing.Name == "Hot Water") continue;

                TraySlotRowContents row = Instantiate(rowContentsPrefab, traySlotDrinkContents.transform);
                row.PopulateIngredient(ing);
            } 
        }
        else
        {
            ResetSlot();
            
        }
    }

    public void ResetSlot()
    {
            disposeButton.image.color = Color.gray;
            disposeButton.enabled = false;
            cupImage.gameObject.SetActive(false);
            liquidImage.color = Color.clear;
            liquidImage.gameObject.SetActive(false);
            contentText.gameObject.SetActive(true);

            foreach (Transform child in traySlotDrinkContents.transform) 
                Destroy(child.gameObject);
    }

    public void DisposeDrink()
    {
        trayDatabase.RemoveDrink((int) slotNum);
        ResetSlot();
        trayStorageUI.UpdateTray();
    }
}
