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
    public List<Ingredient> ingredients = new List<Ingredient>();
    public Sprite drinkSprite;
    public string drinkName = "Custom Mix";
}