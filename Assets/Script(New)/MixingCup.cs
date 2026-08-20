using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.VisualScripting;

public class MixingCup : 
MonoBehaviour,
// DraggableObject,
DropIngredientInterface,
// DropInterface,
IPointerDownHandler,
IDragHandler,
IPointerUpHandler

{
    private Drink drink = new Drink();
    [SerializeField] private Ingredient waterAsset;
    [SerializeField] private TMP_Text ingredientText;
    [SerializeField] private GameObject dragCupPrefab;
    public DragEnum dragType;
    public bool canDrag;
    private GameObject draggedObject;
    private Camera cam;
    private SpriteRenderer sourceRenderer;
    public RecipeBook recipeBook;

    // private List<Ingredient> ingredients = new List<Ingredient>();

    private void Awake()
    {
        cam = Camera.main;
        sourceRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ReceiveIngredient(Ingredient ingredient)
    {
        // ingredients.Add(ingredient);
        drink.ingredients.Add(ingredient);
        dragType = DragEnum.UnfinishedDrink;
        canDrag = true;

        UpdateIngredientText();
    }

    // public override DraggedData GetData()
    // {
    //     return drink;
    // }
    // public void ReceiveDraggable( draggableObject)
    // {
    //     if (draggableObject.GetData() is Ingredient ingredient)
    //     {
    //         drink.ingredients.Add(ingredient);

    //         UpdateIngredientText();
    //     }

    // }
    private void UpdateIngredientText()
    {
        ingredientText.text = string.Join(
            "\n",
            drink.ingredients.ConvertAll(i => i.Name)
        );
    }

    // public List<Ingredient> GetIngredients()
    // {
    //     return ingredients;
    // }

    public void ClearCup()
    {
        ingredientText.text = "Mixing Cup (Empty)";
        canDrag = false;
        dragType = DragEnum.None;
        drink.ingredients.Clear();
    }

    public void AddWater()
    {
        if (!drink.ingredients.Any(ingredient => ingredient.name == "Hot Water"))
        {
            drink.ingredients.Add(waterAsset);
            UpdateIngredientText();
            canDrag = true;
            StirDrink();
            // dragType = DragEnum.FinishedDrink;
        } else
        {
            return;
        }
    }

    public void StirDrink()
    {
        drink.isStirred = true;
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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (canDrag)
        {
            Vector3 worldPos =
                cam.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            draggedObject = Instantiate(dragCupPrefab, worldPos, Quaternion.identity);

            // SpriteRenderer sourceRenderer = GetComponent<SpriteRenderer>();
            // SpriteRenderer dragRenderer = draggedObject.GetComponent<SpriteRenderer>();
            SpriteRenderer dragRenderer = draggedObject.GetComponentInChildren<SpriteRenderer>();

            // dragRenderer.sprite = sourceRenderer.sprite;
            dragRenderer.color = sourceRenderer.color;
            Debug.Log("Pointer Down"); 
            DragManager.BeginDrag(dragType);       
        }
        

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag)
        {
            if (draggedObject == null)
                return;

            Vector3 worldPos =
                cam.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            draggedObject.transform.position = worldPos;          
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canDrag)
        {
            if (draggedObject == null)
                return;

            Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null)
            {
                DropInterface dropTarget = hit.GetComponent<DropInterface>();
                if (dropTarget != null)
                {
                    Debug.Log("Tray received drink!");
                    if (dropTarget.ReceiveDraggable(drink.Clone()))
                    {
                        ClearCup();
                    }
                }
            }

            Destroy(draggedObject);
            draggedObject = null;
            DragManager.EndDrag();       
        }

    }
}