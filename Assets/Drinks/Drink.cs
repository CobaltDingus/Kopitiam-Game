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

    public Drink Clone()
    {
        Drink copy = new Drink();

        copy.ingredients = new List<Ingredient>(ingredients);

        return copy;
    }
}