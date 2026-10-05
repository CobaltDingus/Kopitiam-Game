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

        RecipeRow row = Instantiate(rowPrefab, contentParent);
        row.Populate(drinkRecipe);
    }

    public void BuildListNoteMultiple(List<DrinkRecipe> drinksToAdd)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (DrinkRecipe recipe in drinksToAdd)
        {
            if (recipe == null) continue;

            RecipeRow row = Instantiate(rowPrefab, contentParent);
            row.Populate(recipe);
        }
    }
}
