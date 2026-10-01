using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeBookUI : UIPanel
{
    [SerializeField] RecipeViewer recipeViewer;
    [SerializeField] TextMeshProUGUI glossaryText;
    [SerializeField] List<DrinkRecipe> baseDrinks;
    private int unlockIndex = 1;
    [SerializeField] string cText;
    [SerializeField] string kosongText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // UpdateRecipeBook();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        
    }

    public void UpdateRecipeBook()
    {
        if (!ReworkedSaveManager.instance) return;

        // switch (ReworkedSaveManager.instance.Day)
        // {
        //     case 2:
        //         unlockIndex = 3;
        //         break;
        //     case 3:
        //         glossaryText.text += "\n\n" + cText;
        //         break;
        //     case 4:
        //         glossaryText.text += "\n\n" + kosongText;
        //         break;
        //     case 6:
        //         unlockIndex = 4;
        //         break;
        //     case 7:
        //         unlockIndex = 5;
        //         break;
        //     case 8:
        //         unlockIndex = 6;
        //         break;
        // }
        if (ReworkedSaveManager.instance.Day >= 3)
        {
            glossaryText.text += "\n\n" + cText;
        }
        if (ReworkedSaveManager.instance.Day >= 4)
        {
            glossaryText.text += "\n\n" + kosongText;
        }


        // recipeViewer.BuildListCutoff(unlockIndex);
        recipeViewer.BuildListMaster();
    }
}
