using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DrinkRecipe", menuName = "Scriptable Objects/DrinkRecipe")]
public class DrinkRecipe : ScriptableObject
{
    public string id;
    public string drinkName;
    public List<Ingredient> ingredients;

    public List<Ingredient> baseIngredient;
    // public Sprite drinkImage;
    public string drinkColorHex;
}
