using System;
using System.Linq;
using UnityEngine;

public class DrinkManager : MonoBehaviour
{
    public static DrinkManager Instance { get; private set; }

    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private Ingredient waterAsset;

    private Drink currentDrink = new Drink();
    private int currentStirCount = 0;
    private bool isValidDrink = false;
    private bool canAddIngredients = true;

    public int stirsRequired = 4;

    public event Action OnDrinkChanged;
    public event Action OnStirProgress;
    public event Action OnDrinkCleared;

    public Drink CurrentDrink => currentDrink;
    public int CurrentStirCount => currentStirCount;
    public bool IsValidDrink => isValidDrink;
    public bool CanAddIngredients => canAddIngredients;
    public bool IsStirComplete => currentStirCount >= stirsRequired;

   [Serializable]
    public class PouringSlotState
    {
        public bool hasContainer;
        public ContainerType containerType;
        public Sprite containerSprite;
        public Drink storedDrink = new Drink();
    }

    private PouringSlotState pouringSlot = new PouringSlotState();

    public event Action OnPouringSlotChanged;

    public PouringSlotState PouringSlot => pouringSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AddIngredient(Ingredient ingredient)
    {
        if (ingredient == null) return false;

        if (!canAddIngredients)
        {
            return false;
        }

        currentDrink.ingredients.Add(ingredient);
        isValidDrink = CheckDrinkValidity();
        AudioManager.instance.PlaySFX(SFXType.PowderFilling);
        OnDrinkChanged?.Invoke();
        return true;
    }

    public void AddWater()
    {
        if (currentDrink.ingredients.Any(i => i.name == "HotWater"))
            return;

        currentDrink.ingredients.Add(waterAsset);
        isValidDrink = CheckDrinkValidity();

        OnDrinkChanged?.Invoke();
    }

    public void Stir()
    {
        canAddIngredients = false;

        currentStirCount++;

        if (currentStirCount >= stirsRequired)
        {
            currentDrink.isStirred = true;
        }

        OnStirProgress?.Invoke();
    }

    public void ClearDrink()
    {
        currentDrink = new Drink();
        currentStirCount = 0;
        canAddIngredients = true;
        isValidDrink = false;
        AudioManager.instance.PlaySFX(SFXType.Dispose);
        OnDrinkCleared?.Invoke();
    }

    public Drink TakeFinishedDrink()
    {
        Drink finished = currentDrink.Clone();
        ClearDrink();
        return finished;
    }

    private bool CheckDrinkValidity()
    {
        foreach (DrinkRecipe recipe in recipeBook.allRecipes)
        {
            if (RecipeMatches(recipe)) return true;
        }
        foreach (DrinkRecipe recipe in recipeBook.TouristUniqueRecipe)
        {
            if (RecipeMatches(recipe)) return true;
        }
        return false;
    }

    private bool RecipeMatches(DrinkRecipe recipe)
    {
        if (currentDrink.ingredients.Count != recipe.ingredients.Count)
            return false;

        bool matches = currentDrink.ingredients
            .OrderBy(i => i.Id)
            .SequenceEqual(recipe.ingredients.OrderBy(i => i.Id));

        if (matches)
        {
            currentDrink.drinkName = recipe.drinkName;
            currentDrink.colorHex = recipe.drinkColorHex;
        }

        return matches;
    }

    public bool TrySetContainer(ContainerType containerType, Sprite containerSprite)
    {
        if (pouringSlot.hasContainer) return false;

        pouringSlot.hasContainer = true;
        pouringSlot.containerType = containerType;
        pouringSlot.containerSprite = containerSprite;

        OnPouringSlotChanged?.Invoke();
        return true;
    }

    public bool TryPourDrink(Drink drink)
    {
        if (!pouringSlot.hasContainer || drink == null || !drink.isStirred)
            return false;

        pouringSlot.storedDrink = drink.Clone();
        pouringSlot.storedDrink.containerType = pouringSlot.containerType;
        pouringSlot.storedDrink.isFinished = true;

        OnPouringSlotChanged?.Invoke();
        return true;
    }

    public object TakePouringSlotData()
    {
        return pouringSlot.storedDrink.isFinished
            ? pouringSlot.storedDrink.Clone()
            : pouringSlot.containerType;
    }

    public void ClearPouringSlot()
    {
        pouringSlot = new PouringSlotState();
        OnPouringSlotChanged?.Invoke();
    }
}