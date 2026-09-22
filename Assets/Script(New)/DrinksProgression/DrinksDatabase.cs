using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DrinksDatabase", menuName = "DrinksProgression/DrinksDatabase")]
public class DrinksDatabase : ScriptableObject
{
    [SerializeField] private RecipeBook recipeBook;
    // Kopi, Kopi O
    [SerializeField] private List<DrinkRecipe> startingDrinks;
    // Teh, Uilo, and O
    [SerializeField] private List<DrinkRecipe> dayTwoDrinks;
    // C
    [SerializeField] private List<DrinkRecipe> dayThreeDrinks;
    // Kosong
    [SerializeField] private List<DrinkRecipe> dayFourDrinks;
    // O Kosong, C Kosong
    [SerializeField] private List<DrinkRecipe> dayFiveDrinks;
    // Cham
    [SerializeField] private List<DrinkRecipe> daySixDrinks;
    // Hor Ka Sai
    [SerializeField] private List<DrinkRecipe> daySevenDrinks;
    // Sai Ka Hor
    [SerializeField] private List<DrinkRecipe> dayEightDrinks;

    private List<Drink> drinksToAdd;
    
    public List<Drink> DrinksToAdd => drinksToAdd;
    
    public void UnlockDrinks(int day)
    {
        switch (day)
        {
            case 2:
                AddDrinksToMaster(dayTwoDrinks);
                break;
            case 3:
                AddDrinksToMaster(dayThreeDrinks);
                break;
            case 4:
                AddDrinksToMaster(dayFourDrinks);
                break;
            case 5:
                AddDrinksToMaster(dayFiveDrinks);
                break;
            case 6:
                AddDrinksToMaster(daySixDrinks);
                break;
            case 7:
                AddDrinksToMaster(daySevenDrinks);
                break;
            case 8:
                AddDrinksToMaster(dayEightDrinks);
                break;
            default:
                break;
        }
    }

    public void AddDrinksToMaster(List<DrinkRecipe> drinks)
    {

        foreach (DrinkRecipe drink in drinks)
        {
            recipeBook.AllRecipes.Add(drink);
        }
        // RemoveFromTourist(drinks);
    }

    public void RemoveFromTourist(List<DrinkRecipe> drinks)
    {
        foreach (DrinkRecipe drink in drinks)
        {
            recipeBook.TouristUniqueRecipe.Remove(drink);
        }
    }
}
