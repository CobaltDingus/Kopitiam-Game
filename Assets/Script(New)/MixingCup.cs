using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Data.Common;

public class MixingCup : 
// MonoBehaviour,
DraggableObject,
DropIngredientInterface,
// DropInterface,
IPointerDownHandler,
IDragHandler,
IPointerUpHandler

{
    private Drink drink = new Drink();
    [SerializeField] private Ingredient waterAsset;
    [SerializeField] private TMP_Text ingredientText;
    // [SerializeField] private GameObject dragCupPrefab;
    [SerializeField] private GameObject liquidObject;
    private SpriteRenderer liquidSprite;
    // public DragEnum dragType;
    // public bool canDrag;
    // private GameObject draggedObject;
    // private Camera cam;
    private SpriteRenderer sourceRenderer;
    public RecipeBook recipeBook;

    public Sprite testSprite;
    public string waterColorHex = "#98DCFF";

    private Vector3 originalScale;
    private Coroutine bounceCoroutine;

    // private List<Ingredient> ingredients = new List<Ingredient>();

    // private void Awake()
    // {
    //     cam = Camera.main;
    //     sourceRenderer = GetComponentInChildren<SpriteRenderer>();
        
    // }
    
    private void Start()
    {
        originalScale = transform.localScale;
        liquidSprite = liquidObject.GetComponent<SpriteRenderer>();
        ingredientText.text = "Mixing Cup Contents: \n\nEMPTY";
    }

    public void ReceiveIngredient(Ingredient ingredient)
    {
        // ingredients.Add(ingredient);
        drink.ingredients.Add(ingredient);
        dragType = DragEnum.UnfinishedDrink;
        canDrag = true;

        UpdateIngredientText(ingredient);
    }

    private IEnumerator Bounce()
    {
        float duration = 0.15f;
        float elapsed = 0f;

        Vector3 squashed = new Vector3(
            originalScale.x * 1.1f,
            originalScale.y * 0.9f,
            originalScale.z
        );

        // Squash
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);

            transform.localScale = Vector3.Lerp(
                originalScale,
                squashed,
                t
            );

            yield return null;
        }

        elapsed = 0f;

        // Return to normal
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);

            transform.localScale = Vector3.Lerp(
                squashed,
                originalScale,
                t
            );

            yield return null;
        }

        transform.localScale = originalScale;
        bounceCoroutine = null;
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
    private void UpdateIngredientText(Ingredient ingredient)
    {
        if (drink.ingredients.Count == 1)
        {
            ingredientText.text = "Mixing Cup Contents: \n\n";
        }

        // ingredientText.text = string.Join(
        //     "\n",
        //     drink.ingredients.ConvertAll(i => i.Name)
        // );

        ingredientText.text += ingredient.Name + "\n";

        if (bounceCoroutine != null) {
            StopCoroutine(bounceCoroutine);
        }

        bounceCoroutine = StartCoroutine(Bounce());
    }

    // public List<Ingredient> GetIngredients()
    // {
    //     return ingredients;
    // }

    public void ClearCup()
    {
        ingredientText.text = "Mixing Cup Contents: \n\nEMPTY";
        canDrag = false;
        dragType = DragEnum.None;
        drink.ingredients.Clear();
        liquidObject.SetActive(false);
    }

    public void AddWater()
    {
        if (!drink.ingredients.Any(ingredient => ingredient.name == "Hot Water"))
        {
            drink.ingredients.Add(waterAsset);
            UpdateIngredientText(waterAsset);
            dragType = DragEnum.UnfinishedDrink;
            canDrag = true;
            SetSpriteColorFromHex(waterColorHex, liquidSprite);
            liquidObject.SetActive(true);
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
                testSprite = recipe.drinkImage;
                drink.drinkName = recipe.drinkName;
                break;
            }
            
        }
    }

    public override object GetData()
    {
        return drink.Clone();
    }

    public override void AfterDropFunctions()
    {
        ClearCup();
    }

    public void SetSpriteColorFromHex(string hex, SpriteRenderer spriteComponent)
    {
        // TryParseHtmlString returns true if the conversion is successful
        if (ColorUtility.TryParseHtmlString(hex, out Color newColor))
        {
            spriteComponent.color = newColor;
        }
        else
        {
            Debug.LogWarning("Invalid Hexadecimal string provided!");
        }
    }
}