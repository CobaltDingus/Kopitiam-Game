using UnityEngine;
using UnityEngine.EventSystems;

public class IngredientObject : 
    DraggableObject
{
    public Ingredient ingredientAsset;

    // Progression stage stuff
    public int unlockDay = 0;

    public override object GetData()
    {
        return ingredientAsset;
    }

    public override void AfterDropFunctions()
    {
        return;
    }
}