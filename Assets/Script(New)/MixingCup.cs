using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MixingCup : MonoBehaviour, DropInterface
{
    [SerializeField] private TMP_Text ingredientText;

    private List<Ingredient> ingredients = new List<Ingredient>();

    public void ReceiveIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient);

        UpdateIngredientText();
    }

    private void UpdateIngredientText()
    {
        ingredientText.text = string.Join(
            "\n",
            ingredients.ConvertAll(i => i.Name)
        );
    }

    public List<Ingredient> GetIngredients()
    {
        return ingredients;
    }

    public void ClearCup()
    {
        ingredients.Clear();
        ingredientText.text = "";
    }
}