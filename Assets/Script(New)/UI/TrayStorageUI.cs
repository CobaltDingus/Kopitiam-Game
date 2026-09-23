using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrayStorageUI : UIPanel
{
    [SerializeField] private TextMeshProUGUI contentsText;
    [SerializeField] private TrayDatabase trayDatabase;
    [SerializeField] private Button disposeButton;
    [SerializeField] private Image cupImage;
    [SerializeField] private Image liquidImage;
    [SerializeField] private List<TrayUISlot> traySlots;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateTray();
    }

    void OnEnable()
    {
        trayDatabase.OnDrinkAdded += UpdateTray;
    }

    void OnDisable()
    {
        trayDatabase.OnDrinkAdded -= UpdateTray;
    }

    // Update is called once per frame
    void Update()
    {
        // if (trayDatabase.SavedDrinks.Count > 0)
        // {
        //     for (int i = 0; i < trayDatabase.MaxSlots; i++)
        //     {
                
        //     }
        //     // contentsText.text = "Currently Stored Drink:\n\n" + string.Join(
        //     //     "\n",
        //     //     trayDatabase.SavedDrinks[0].ingredients.ConvertAll(i => "- " + i.Name)
        //     // );
        //     contentsText.text = string.Join(
        //         "\n",
        //         trayDatabase.SavedDrinks[0].ingredients.ConvertAll(i => "- " + i.Name)
        //     );            
        //     disposeButton.image.color = Color.red;
        //     disposeButton.enabled = true;
        //     cupImage.gameObject.SetActive(true);
        //     liquidImage.color = HexToColor(trayDatabase.SavedDrinks[0].colorHex);
        //     liquidImage.gameObject.SetActive(true);
        // }
        // else
        // {
        //     // contentsText.text = "Currently Stored Drink: \n\nEMPTY";
        //     contentsText.text = "EMPTY";
        //     disposeButton.image.color = Color.gray;
        //     disposeButton.enabled = false;
        //     cupImage.gameObject.SetActive(false);
        //     liquidImage.color = Color.clear;
        //     liquidImage.gameObject.SetActive(false);
        // }



    }

    public void UpdateTray()
    {
        if (trayDatabase.SavedDrinks.Count > 0)
        {
            for (int i = 0; i < trayDatabase.MaxSlots; i++)
            {
                
            }
            // contentsText.text = "Currently Stored Drink:\n\n" + string.Join(
            //     "\n",
            //     trayDatabase.SavedDrinks[0].ingredients.ConvertAll(i => "- " + i.Name)
            // );
            contentsText.text = string.Join(
                "\n",
                trayDatabase.SavedDrinks[0].ingredients.ConvertAll(i => "- " + i.Name)
            );            
            disposeButton.image.color = Color.red;
            disposeButton.enabled = true;
            cupImage.gameObject.SetActive(true);
            liquidImage.color = HexToColor(trayDatabase.SavedDrinks[0].colorHex);
            liquidImage.gameObject.SetActive(true);
        }
        else
        {
            // contentsText.text = "Currently Stored Drink: \n\nEMPTY";
            contentsText.text = "EMPTY";
            disposeButton.image.color = Color.gray;
            disposeButton.enabled = false;
            cupImage.gameObject.SetActive(false);
            liquidImage.color = Color.clear;
            liquidImage.gameObject.SetActive(false);
        }


    }

    public void DisposeDrink()
    {
        trayDatabase.ClearDatabase();
        UpdateTray();
    }

    public Color HexToColor(string hexCode)
    {
        if (ColorUtility.TryParseHtmlString(hexCode, out Color newColor))
        {
            return newColor;
        }
        else
        {
            Debug.LogWarning("Invalid Hexadecimal string provided!");
            return Color.clear;
        }
    }
}
