using System.Collections.Generic;
using UnityEngine;

public class RecipeViewer : MonoBehaviour
{
    [SerializeField] private List<DrinkRecipe> drinkRecipes;
    [SerializeField] private RecipeRow rowPrefab;
    [SerializeField] private Transform contentParent;
    private void OnEnable()
    {
        BuildList();
    }

    public void BuildList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (drinkRecipes == null) return;

        foreach (DrinkRecipe recipe in drinkRecipes)
        {
            if (recipe == null) continue;

            RecipeRow row = Instantiate(rowPrefab, contentParent);
            row.Populate(recipe);
        }
    }
}
