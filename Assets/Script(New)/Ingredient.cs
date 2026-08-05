using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients/Ingredient")]
public class Ingredient : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string ingredientName;
    [SerializeField] private Sprite icon;

    public string Id => id;
    public string Name => ingredientName;
    public Sprite Icon => icon;

    //public string Id { get { return id; } }
    //public string Name { get { return ingredientName; } }
    //public Sprite Icon {  get { return icon; } }
}
