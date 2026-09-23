using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TraySlotRowContents : MonoBehaviour
{
    [SerializeField] private Image ingredientImage;
    [SerializeField] private TextMeshProUGUI ingredientTMP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PopulateIngredient(Ingredient ingredient)
    {
        ingredientTMP.text = ingredient.Name;
        ingredientImage.sprite = ingredient.Icon;

        // foreach (Transform child in ingredientContainer)
        //     Destroy(child.gameObject);

        // if (recipe.ingredients == null) return;

        // foreach (Ingredient ing in recipe.ingredients)
        // {
        //     if (ing == null || ing.Name == "Hot Water") continue;

        //     Image icon = Instantiate(ingredientIconPrefab, ingredientContainer);
        //     icon.sprite = ing.Icon;
        // }
    }
}
