using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeRow : MonoBehaviour
{
    [SerializeField] private TMP_Text drinkNameText;
    [SerializeField] private Transform ingredientContainer;
    [SerializeField] private Image ingredientIconPrefab;

    public void Populate(DrinkRecipe recipe)
    {
        drinkNameText.text = recipe.drinkName;

        foreach (Transform child in ingredientContainer)
            Destroy(child.gameObject);

        if (recipe.ingredients == null) return;

        foreach (Ingredient ing in recipe.ingredients)
        {
            if (ing == null || ing.Name == "Hot Water") continue;

            Image icon = Instantiate(ingredientIconPrefab, ingredientContainer);
            icon.sprite = ing.Icon;
        }
    }
}