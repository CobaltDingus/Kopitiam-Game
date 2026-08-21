using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drink
{

    // public Ingredient cupType;
    //OLD CODE
    //public List<Ingredient> ingredients = new();

    // public Color displayColor;
    // public string displayName;
    private bool hasWater;
    public List<Ingredient> ingredients = new List<Ingredient>();
    public Sprite drinkSprite;
    public string drinkName = "";
    public ContainerType containerType = ContainerType.None;
    public bool isStirred;
    public bool isFinished;

    public Drink Clone()
    {
        Drink copy = new Drink();

        copy.ingredients = new List<Ingredient>(ingredients);
        copy.containerType = containerType;
        copy.drinkName = drinkName;
        copy.drinkSprite = drinkSprite;
        copy.isStirred = isStirred;
        copy.isFinished = isFinished;
        copy.hasWater = hasWater;

        return copy;
    }
}