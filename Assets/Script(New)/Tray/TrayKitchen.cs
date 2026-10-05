using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
public class TrayKitchen :  
ExpandableUI,
DropInterface,
IPointerDownHandler
// IPointerUpHandler

// DropDrinkInterface
{
    public TrayDatabase trayDatabase;
    public RecipeBook recipeBook;
    public UIPanel panel;
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
                    // drink.drinkSprite = recipe.drinkImage;
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

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        panel.OpenPanel();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
