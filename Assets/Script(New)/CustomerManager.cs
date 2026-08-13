using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Databases")]
    [SerializeField] private CustomerDatabase customerDatabase;
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private TrayDatabase trayDatabase;

    [Header("UI & Display References")]
    [SerializeField] private SpriteRenderer customerSpriteRenderer;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Order Settings")]
    [SerializeField] private int minDrinks = 1;
    [SerializeField] private int maxDrinks = 3;

    private CustomerData currentCustomer;
    private List<DrinkRecipe> orderedRecipes = new();

    private Sprite currentSprite;
    private string currentDialogueText = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (currentCustomer == null)
        {
            GenerateNewCustomer();
        }
            
    }

    public void GenerateNewCustomer()
    {
        //temp debug
        if (customerDatabase == null || customerDatabase.AllCustomers.Count == 0)
        {
            Debug.LogError("CustomerDatabase is empty or unassigned!");
            return;
        }

        if (recipeBook == null || recipeBook.AllRecipes.Count == 0)
        {
            Debug.LogError("RecipeBook is empty or unassigned!");
            return;
        }

        // clear whatever was left in the tray from the previous customer
        if (trayDatabase != null)
        {
            trayDatabase.ClearDatabase();
        }

        //select random customerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
        int randomCustomerIndex = UnityEngine.Random.Range(0, customerDatabase.AllCustomers.Count);
        currentCustomer = customerDatabase.AllCustomers[randomCustomerIndex];

        SetCustomerSprite();

        // choose 1 to 3 drinks randomly (duplicates allowed)
        orderedRecipes.Clear();
        int drinkCount = UnityEngine.Random.Range(minDrinks, maxDrinks + 1);

        for (int i = 0; i < drinkCount; i++)
        {
            int randomRecipeIndex = UnityEngine.Random.Range(0, recipeBook.AllRecipes.Count);
            orderedRecipes.Add(recipeBook.AllRecipes[randomRecipeIndex]);
        }

        // let the tray only accept as many drinks as were ordered
        if (trayDatabase != null)
            trayDatabase.MaxSlots = drinkCount;

        SetStartDialogue();
    }

    public void SetCustomerSprite()
    {
        if (currentCustomer.CustomerSprite == null || currentCustomer.CustomerSprite.Count == 0)
        {
            Debug.LogWarning($"Customer '{currentCustomer.CustomerName}' has no sprites assigned.");
            return;
        }

        int spriteIndex = UnityEngine.Random.Range(0, currentCustomer.CustomerSprite.Count);
        currentSprite = currentCustomer.CustomerSprite[spriteIndex];

        if (customerSpriteRenderer != null)
        {
            customerSpriteRenderer.sprite = currentSprite;
        }
    }

    public void SetStartDialogue()
    {
        if (currentCustomer == null) return;

        int index = GetRandomDialogueIndex(currentCustomer.StartFrontDialogue, currentCustomer.StartBackDialogue);
        if (index == -1)
        {
            if (dialogueText != null) dialogueText.text = "ded";
            return;
        }

        string front = currentCustomer.StartFrontDialogue[index];
        string back = currentCustomer.StartBackDialogue[index];

        SetDialogueText(front, back);
    }

    public void ServeOrder()
    {
        if (currentCustomer == null)
        {
            Debug.LogWarning("No current customer to serve.");
            return;
        }

        if (trayDatabase == null)
        {
            Debug.LogError("TrayDatabase is unassigned!");
            return;
        }

        EvaluateAndSetEndDialogue(trayDatabase.SavedDrinks);
    }

    public void ReceiveDrink(Drink servedDrink)
    {
        if (currentCustomer == null || servedDrink == null) return;
        EvaluateAndSetEndDialogue(new List<Drink> { servedDrink });
    }

    public void EvaluateAndSetEndDialogue(List<Drink> servedDrinks)
    {
        if (currentCustomer == null || orderedRecipes.Count == 0) return;

        servedDrinks ??= new List<Drink>();

        int perfectCount = 0;

        for (int i = 0; i < orderedRecipes.Count; i++)
        {
            DrinkRecipe recipe = orderedRecipes[i];
            Drink served = i < servedDrinks.Count ? servedDrinks[i] : null;

            if (IsDrinkPerfect(served, recipe))
                perfectCount++;
        }

        List<string> frontList;
        List<string> backList;

        if (perfectCount == orderedRecipes.Count)
        {
            // all drinks correct
            frontList = currentCustomer.PerfectFrontDialogue;
            backList = currentCustomer.PerfectBackDialogue;
        }
        else if (perfectCount == 0)
        {
            // all drinks wrong
            frontList = currentCustomer.WrongFrontDialogue;
            backList = currentCustomer.WrongBackDialogue;
        }
        else
        {
            // at least one wrong, but not all
            frontList = currentCustomer.DecentFrontDialogue;
            backList = currentCustomer.DecentBackDialogue;
        }

        int index = GetRandomDialogueIndex(frontList, backList);
        if (index == -1)
        {
            if (dialogueText != null) dialogueText.text = "ded";
            return;
        }

        SetDialogueText(frontList[index], backList[index]);
    }

    private bool IsDrinkPerfect(Drink served, DrinkRecipe recipe)
    {
        if (served == null || recipe == null) return false;

        bool nameMatch = string.Equals(served.drinkName, recipe.drinkName, StringComparison.OrdinalIgnoreCase);
        if (!nameMatch) return false;

        return IngredientsMatch(served.ingredients, recipe.ingredients);
    }

    private bool IngredientsMatch(List<Ingredient> served, List<Ingredient> required)
    {
        served ??= new List<Ingredient>();
        required ??= new List<Ingredient>();

        if (served.Count != required.Count) return false;

        List<Ingredient> remaining = new List<Ingredient>(served);

        foreach (Ingredient requiredIngredient in required)
        {
            Ingredient match = remaining.FirstOrDefault(i =>
                i != null && requiredIngredient != null && i.Id == requiredIngredient.Id);

            if (match == null) return false;

            remaining.Remove(match);
        }

        return true;
    }

    private int GetRandomDialogueIndex(List<string> frontList, List<string> backList)
    {
        if (frontList == null || backList == null || frontList.Count == 0 || backList.Count == 0)
            return -1;

        int maxIndex = Mathf.Min(frontList.Count, backList.Count);
        return UnityEngine.Random.Range(0, maxIndex);
    }

    private void SetDialogueText(string front, string back)
    {
        string drinkListText = string.Join(", ", orderedRecipes.Select(r => r.drinkName));
        currentDialogueText = $"{front} {drinkListText}. {back}";

        if (dialogueText != null)
            dialogueText.text = currentDialogueText;
    }

    //private string GetRandomStringFromList(List<string> list)
    //{
    //    if (list == null || list.Count == 0) return "...";
    //    return list[UnityEngine.Random.Range(0, list.Count)];
    //}


    // Update is called once per frame
    void Update()
    {
        
    }
}
