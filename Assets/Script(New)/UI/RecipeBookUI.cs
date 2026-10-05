using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipeBookUI : UIPanel
{
    [SerializeField] RecipeViewer recipeViewer;
    [SerializeField] TextMeshProUGUI glossaryText;
    [SerializeField] List<DrinkRecipe> baseDrinks;
    [SerializeField] string cText;
    [SerializeField] string kosongText;

    public void UpdateRecipeBook()
    {
        if (!ReworkedSaveManager.instance) return;

        if (ReworkedSaveManager.instance.Day >= 3)
        {
            glossaryText.text += "\n\n" + cText;
        }
        if (ReworkedSaveManager.instance.Day >= 4)
        {
            glossaryText.text += "\n\n" + kosongText;
        }
        recipeViewer.BuildListMaster();
    }
}
