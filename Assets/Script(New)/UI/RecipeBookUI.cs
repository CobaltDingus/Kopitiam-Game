using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeBookUI : UIPanel
{
    [SerializeField] RecipeViewer recipeViewer;
    [SerializeField] TextMeshProUGUI glossaryText;
    [SerializeField] List<DrinkRecipe> baseDrinks;
    [SerializeField] string cText;
    [SerializeField] string kosongText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        UpdateRecipeBook();
    }

    public void UpdateRecipeBook()
    {
        if (!ReworkedSaveManager.instance) return;
        
        switch (ReworkedSaveManager.instance.Day)
        {
            case 2:
                recipeViewer.BuildListCutoff(3);
                break;
            case 3:
                glossaryText.text += "\n\n" + cText;
                break;
            case 4:
                glossaryText.text += "\n\n" + kosongText;
                break;
            case 6:
                recipeViewer.BuildListCutoff(4);
                break;
            case 7:
                recipeViewer.BuildListCutoff(5);
                break;
            case 8:
                recipeViewer.BuildListCutoff(6);
                break;
        }
    }
}
