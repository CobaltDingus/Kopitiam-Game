using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MixingCup : 
DraggableObject,
DropIngredientInterface,
// DropInterface,
IPointerDownHandler,
IDragHandler,
IPointerUpHandler

{
    private Drink drink = new Drink();
    [SerializeField] private TMP_Text ingredientText;
    // [SerializeField] private GameObject dragCupPrefab;
    
    // private GameObject draggedObject;
    // private Camera cam;
    // private SpriteRenderer sourceRenderer;

    // // private List<Ingredient> ingredients = new List<Ingredient>();



    // private void Awake()
    // {
    //     cam = Camera.main;
    //     sourceRenderer = GetComponentInChildren<SpriteRenderer>();
    // }

    public void ReceiveIngredient(Ingredient ingredient)
    {
        // ingredients.Add(ingredient);
        drink.ingredients.Add(ingredient);

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

    // // public List<Ingredient> GetIngredients()
    // // {
    // //     return ingredients;
    // // }

    // public void ClearCup()
    // {
    //     drink.ingredients.Clear();
    //     ingredientText.text = "";
    // }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     Vector3 worldPos =
    //         cam.ScreenToWorldPoint(eventData.position);
    //     worldPos.z = 0;

    //     draggedObject = Instantiate(dragCupPrefab, worldPos, Quaternion.identity);

    //     // SpriteRenderer sourceRenderer = GetComponent<SpriteRenderer>();
    //     // SpriteRenderer dragRenderer = draggedObject.GetComponent<SpriteRenderer>();
    //     SpriteRenderer dragRenderer = draggedObject.GetComponentInChildren<SpriteRenderer>();

    //     // dragRenderer.sprite = sourceRenderer.sprite;
    //     dragRenderer.color = sourceRenderer.color;
    //     Debug.Log("Pointer Down");
    // }

    // public void OnDrag(PointerEventData eventData)
    // {
    //     if (draggedObject == null)
    //         return;

    //     Vector3 worldPos =
    //         cam.ScreenToWorldPoint(eventData.position);
    //     worldPos.z = 0;

    //     draggedObject.transform.position = worldPos;
    // }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     if (draggedObject == null)
    //         return;

    //     Vector3 worldPos = cam.ScreenToWorldPoint(eventData.position);
    //     worldPos.z = 0;

    //     Collider2D hit = Physics2D.OverlapPoint(worldPos);

    //     if (hit != null)
    //     {
    //         DropDrinkInterface dropTarget = hit.GetComponent<DropDrinkInterface>();
    //         if (dropTarget != null)
    //         {
    //             Debug.Log("Tray received drink!");
    //             dropTarget.ReceiveDrink(drink);
    //             ClearCup();
    //         }
    //     }

    //     Destroy(draggedObject);
    //     draggedObject = null;
    // }
}