using UnityEngine;
using System.Linq;
public class Tray :  
ExpandableUI,
DropInterface
// DropDrinkInterface
{
    public TrayDatabase trayDatabase;
    public RecipeBook recipeBook;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool ReceiveDraggable<T>(T data)
    {
        if (validDragTypes.Contains(DragManager.CurrentlyDragging))
        {
            if (data is not Drink drink)
            {
                return false;
            }

            foreach (DrinkRecipe recipe in recipeBook.allRecipes)
            {
                if (drink.ingredients.Count == recipe.ingredients.Count &&
                drink.ingredients
                .OrderBy(i => i.Id)
                .SequenceEqual(recipe.ingredients.OrderBy(i => i.Id)))
                {
                    drink.drinkSprite = recipe.drinkImage;
                    drink.drinkName = recipe.drinkName;
                    break;
                }
                
            }
            if (trayDatabase.AddDrink(drink) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
