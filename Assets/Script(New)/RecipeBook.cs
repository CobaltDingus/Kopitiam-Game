using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeBook", menuName = "Drinks/RecipeBook")]
public class RecipeBook : ScriptableObject
{
    //OLD CODE
    //[System.Serializable]
    //public class DrinkRecipe
    //{
    //    public string id;
    //    public string drinkName;
    //    public List<Ingredient> ingredients;
    //    public Sprite drinkImage;
    //}

    //[SerializeField] private List<DrinkRecipe> allRecipes = new List<DrinkRecipe>();
    ////public List<DrinkRecipe> AllRecipes => allRecipes;
    //public List<DrinkRecipe> AllRecipes { get { return allRecipes; } }

    public List<DrinkRecipe> allRecipes = new List<DrinkRecipe>();
    public List<DrinkRecipe> AllRecipes => allRecipes;
    public List<DrinkRecipe> _touristUniqueRecipe = new List<DrinkRecipe>();
    public List<DrinkRecipe> TouristUniqueRecipe => _touristUniqueRecipe;

    public List<DrinkRecipe> _everyRecipe = new List<DrinkRecipe>();
    public List<DrinkRecipe> EveryRecipe => _everyRecipe;

}