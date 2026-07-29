using UnityEngine;

public class IngredientPrefab : MonoBehaviour
{
    public Ingredient ingredientAsset;

    [HideInInspector]
    public SpriteRenderer placeholderRenderer;

    private void Awake()
    {
        placeholderRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}