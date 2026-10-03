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
    }

    void OnEnable()
    {
        UpdateTray();
    }

    void OnDisable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateTray()
    {
        if (trayDatabase.SavedDrinks.Count > 0)
        {
            for (int i = 0; i < trayDatabase.MaxSlots; i++)
            {
                traySlots[i].UpdateSlot();
            }
        }
    }

    public void DisposeDrink()
    {
        trayDatabase.ClearDatabase();
    }
}
