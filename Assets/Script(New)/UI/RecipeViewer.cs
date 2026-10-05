using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeViewer : MonoBehaviour
{
    [SerializeField] private List<DrinkRecipe> drinkRecipes;
    [SerializeField] private List<DrinkRecipe> baseDrinkRecipes;
    [SerializeField] private RecipeBook masterRecipeBook;
    [SerializeField] private RecipeRow rowPrefab;
    [SerializeField] private Transform contentParent;
    
    private void OnEnable()
    {
        // BuildListMaster();
    }

    public void BuildListMaster()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (drinkRecipes == null) return;

        foreach (DrinkRecipe recipe in masterRecipeBook.allRecipes)
        {
            if (recipe == null || !recipe.isBaseDrink) continue;

            RecipeRow row = Instantiate(rowPrefab, contentParent);
            row.Populate(recipe);
        }
    }

    public void BuildListNoteSingle(DrinkRecipe drinkRecipe)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // if (drinkRecipes == null) return;

        // foreach (DrinkRecipe recipe in drinkRecipes)
        // {
            // if (recipe == null) continue;

        RecipeRow row = Instantiate(rowPrefab, contentParent);
        row.Populate(drinkRecipe);
        // }
    }

    public void BuildListNoteMultiple(List<DrinkRecipe> drinksToAdd)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // if (drinkRecipes == null) return;

        foreach (DrinkRecipe recipe in drinksToAdd)
        {
            if (recipe == null) continue;

            RecipeRow row = Instantiate(rowPrefab, contentParent);
            row.Populate(recipe);
        }
    }

    public void BuildListCutoff(int cutOff)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // if (drinkRecipes == null) return;

        // foreach (DrinkRecipe recipe in baseDrinkRecipes)
        // {
        //     if (recipe == null) continue;

        //     RecipeRow row = Instantiate(rowPrefab, contentParent);
        //     row.Populate(recipe);
        // }

        for (int i = 0; i < cutOff; i++)
        {
            RecipeRow row = Instantiate(rowPrefab, contentParent);
            row.Populate(baseDrinkRecipes[i]); 
        }
    }
}
