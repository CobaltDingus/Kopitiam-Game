using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/Ingredient")]
public class Ingredient : ScriptableObject
{
    public enum IngredientType
    {
        Powder,
        Liquid
    }
    [SerializeField] private string id;
    [SerializeField] private string ingredientName;
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isBaseIngredient;
    [SerializeField] private IngredientType ingType;
    

    public string Id => id;
    public string Name => ingredientName;
    public Sprite Icon => icon;
    public bool IsBaseIngredient => isBaseIngredient;
    public IngredientType IngType => ingType;

    //public string Id { get { return id; } }
    //public string Name { get { return ingredientName; } }
    //public Sprite Icon {  get { return icon; } }
}
