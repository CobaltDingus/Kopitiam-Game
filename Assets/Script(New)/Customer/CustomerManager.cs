using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [SerializeField] private List<string> tutorialDialogue;
    //[SerializeField] private string tutorialWrong = "Thats the wrong drink, could you do it again?";



    //[SerializeField] private TMP_Text timerText;

    //private float timeElapsed;
    //private float timeRemaining;
    //[SerializeField] private float duration = 120f;

    //private bool isTimerRunning;

    //[SerializeField] private bool challenge = false;

    private bool hasServed = false;

    [Header("Order Settings")]
    [SerializeField] private int minDrinks = 1;
    [SerializeField] private int maxDrinks = 3; // inclusive

    // swapped out from public to private var testing
    [SerializeField] private Button next;
    [SerializeField] private Button tryAgain;


    Scene currentScene;

    private CustomerData currentCustomer;
    private List<DrinkRecipe> orderedRecipes = new();

    // Cached "last known good" state so a newly loaded scene's UI can be
    // repainted instantly without re-rolling the customer/order/dialogue.

    private Sprite currentSprite;
    private string currentDialogueText = "";

    public bool serveStatus => hasServed;

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
            //if (SaveManager.saveManager.DayCount == 0)
            //{

            //    GenerateTutorialDayZero();

            //}
            //else
            //{
            //    GenerateNewCustomer();
            //}
        }

        //RestartTimer();

        SceneManager.sceneLoaded += OnSceneLoaded;
        currentScene = SceneManager.GetActiveScene();

        next.onClick.AddListener(GenerateNewCustomer);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CounterScene")
        {
            // FindNextButton();
            return;
        }
    }

// void FindNextButton()
// {
//     GameObject nextObj = GameObject.Find("Next");
//     if (nextObj == null)
//     {
//         Debug.LogError("FindNextButton: No GameObject named 'Next' found in scene.");
//         return;
//     }

//     next = nextObj.GetComponent<Button>();
//     if (next == null)
//     {
//         Debug.LogError("FindNextButton: 'Next' object has no Button component.");
//         return;
//     }

//     var bridge = next.GetComponent<CustomerManagerUIBridge>();
//     if (bridge == null)
//     {
//         Debug.LogError("FindNextButton: 'Next' object has no CustomerManagerUIBridge component.");
//         return;
//     }

//     next.onClick.AddListener(GenerateNewCustomer);
//     next.onClick.AddListener(bridge.OnGenerateNewCustomerClicked);
// }

    void FindTryAgainButton()
    {
        
    }

    // ---------------------------------------------------------------
    // Called by CustomerDisplayLink (placed on the sprite/text objects
    // in each scene) once that scene has loaded. Immediately repaints
    // the newly found UI with whatever the current state already is,
    // instead of generating anything new.
    // ---------------------------------------------------------------
    public void RegisterDisplayReferences(SpriteRenderer spriteRenderer, TMP_Text text)
    {
        customerSpriteRenderer = spriteRenderer;
        dialogueText = text;

        if (customerSpriteRenderer != null)
            customerSpriteRenderer.sprite = currentSprite;

        if (dialogueText != null)
            dialogueText.text = currentDialogueText;
    }

    // ---------------------------------------------------------------
    // Generate a brand new customer + order. Hook this to a button
    // (or call it from Start) whenever you want to reset the scene.
    // Kept fully separate from ServeOrder/Evaluate below.
    // ---------------------------------------------------------------
    public void GenerateTutorialDayZero()
    {
        UiManager.uiManager.HideTimer();
        if(SaveManager.saveManager.TutorialPhase == 0)
        {
            //int position;
            
            for (int i =0; i < recipeBook.AllRecipes.Count; i++)
            {
                // add a drink where the drink name is called "Kopi O"
            }
            //orderedRecipes.Add(recipeBook.AllRecipes[]);

        }
        else if (SaveManager.saveManager.TutorialPhase == 1)
        {

        }
        
    }

    public void GenerateNewCustomer()
    {
        UiManager.uiManager.TurnOnTimer();
        UiManager.uiManager.RestartTimer();

        //SaveManager.saveManager.
        hasServed = false;
        //Debug.Log("Generate Customer Button Clicked.");
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
            trayDatabase.ClearDatabase();

        // select random customer
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

    private void SetCustomerSprite()
    {
        if (currentCustomer.CustomerSprite == null || currentCustomer.CustomerSprite.Count == 0)
        {
            Debug.LogWarning($"Customer '{currentCustomer.CustomerName}' has no sprites assigned.");
            return;
        }

        int spriteIndex = UnityEngine.Random.Range(0, currentCustomer.CustomerSprite.Count);
        currentSprite = currentCustomer.CustomerSprite[spriteIndex];

        if (customerSpriteRenderer != null)
            customerSpriteRenderer.sprite = currentSprite;
    }

    public void SetStartDialogue()
    {
        if (currentCustomer == null) return;

        int index = GetRandomDialogueIndex(currentCustomer.StartFrontDialogue, currentCustomer.StartBackDialogue);
        if (index == -1)
        {
            if (dialogueText != null) dialogueText.text = "...";
            return;
        }

        string front = currentCustomer.StartFrontDialogue[index];
        string back = currentCustomer.StartBackDialogue[index];

        SetDialogueText(front, back);
    }

    public void ServePerfectDrinksForTesting()
    {
        if (currentCustomer == null || orderedRecipes.Count == 0)
        {
            Debug.LogWarning("No current customer or order to serve.");
            return;
        }

        if (trayDatabase == null)
        {
            Debug.LogError("TrayDatabase is unassigned!");
            return;
        }

        trayDatabase.ClearDatabase();

        foreach (DrinkRecipe recipe in orderedRecipes)
        {
            Drink perfectDrink = new Drink
            {
                drinkName = recipe.drinkName,
                ingredients = new List<Ingredient>(recipe.ingredients),
                drinkSprite = recipe.drinkImage
            };

            trayDatabase.AddDrink(perfectDrink);
        }

        //ServeOrder();
    }
    public void ServeOrder()
    {
        UiManager.uiManager.TurnOffTimer();
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
        trayDatabase.ClearDatabase();
    }

    public void CustomerServed()
    {
        hasServed = true;
        return;
    }

    // Kept for backwards compatibility if something still calls this with a single Drink.
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
            //SaveManager.saveManager.setTutorialPhase(1);
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
            if (dialogueText != null) dialogueText.text = "...";
            return;
        }

        SetEndDialogueText(frontList[index], backList[index]);
    }

    // First wave: drink name. Second wave: ingredient-by-ingredient comparison.
    private bool IsDrinkPerfect(Drink served, DrinkRecipe recipe)
    {
        if (served == null || recipe == null) return false;

        bool nameMatch = string.Equals(served.drinkName, recipe.drinkName, StringComparison.OrdinalIgnoreCase);
        if (!nameMatch) return false;

        return IngredientsMatch(served.ingredients, recipe.ingredients);
    }

    // Multiset comparison so duplicate ingredients and order don't matter.
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

    private void SetEndDialogueText(string front, string back)
    {
        currentDialogueText = $"{front} {back}";
        if (dialogueText != null)
            dialogueText.text = currentDialogueText;
    }

}
