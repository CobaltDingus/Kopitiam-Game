using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

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

    // Tutorial stuff
    [SerializeField] private List<string> tutorialDialogue;
    private string tutorialWrongDialogue = "Hmm, that's not right. Try again.";
    [SerializeField] private Sprite TutorialBoss;

    //
    private bool hasServed = false;

    [Header("Order Settings")]
    [SerializeField] private int minDrinks = 1;
    [SerializeField] private int maxDrinks = 1; // inclusive

    // swapped out from public to private var testing
    [SerializeField] private Button next;
    [SerializeField] private Button okay;

    [SerializeField] private Button tryAgain;
    [SerializeField] private Button perfect;

    private bool canEvaluateTutorial = false;

    private bool isNext;
    private bool isOkay;
    private bool isTryAgain;
    private bool isCustomerCount;

    private bool isFavour;
    

    public int tutorialPhase;
    private bool tutorialwrong;

    public bool tutorialserve = false;

    public bool tutorialComplete = false;
    //getters
    public bool IsFavour => isFavour;
    public bool IsNext => isNext;
    public bool IsOkay => isOkay;
    public bool IsTryAgain => isTryAgain;
    public bool IsCustomerCount => isCustomerCount;

    public Button NextButton => next;
    public Button OkayButton => okay;
    public Button TryAgainButton => tryAgain;

    public int chosenCustomerSlot;

    //setters

    public void setIsNext(bool status)
    {
        isNext = status;
    }

    public void setIsOkay(bool status)
    {
        isOkay = status;
    }

    public void setIsTryAgain(bool status)
    {
        isTryAgain = status;
    }

    Scene currentScene;

    private CustomerData currentCustomer;
    private List<DrinkRecipe> orderedRecipes = new();

    // Cached "last known good" state so a newly loaded scene's UI can be
    // repainted instantly without re-rolling the customer/order/dialogue.

    private Sprite currentSprite;
    private int chosenSpriteIndex;
    private string currentDialogueText = "";
    private string lastNonMatchedIngredientsText = "";

    public string CurrentDialogueText => currentDialogueText;

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
        trayDatabase.ClearDatabase();
        //SaveManager.saveManager
        UiManager.uiManager.UpdateDayCount();
        if (currentCustomer == null)
        {
            //GenerateNewCustomer();
            tutorialwrong = false;
            tryAgain.gameObject.SetActive(false);
            okay.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            isNext = false;
            if (SaveManager.saveManager.DayCount == 0)
            {
                isFavour = false;
                isCustomerCount = false;
                okay.gameObject.SetActive(true);
                customerSpriteRenderer.sprite = TutorialBoss;
                currentSprite = TutorialBoss;
                tutorialPhase = 0;
                GenerateTutorialDayZero();
                LoadTutorialDialogue();
            }
            else
            {
                isCustomerCount = true;
                isFavour = true;
                UiManager.uiManager.ShowCustomerCount();
                UiManager.uiManager.ShowFavour();
                next.gameObject.SetActive(true);
                isNext = true;
                GenerateNewCustomer();
            }
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
            FindNextButton();
            FindOkayButton();
            FindTryAgainButton();
            // FindPerfectButton();
            return;
        }
    }

    void FindNextButton()
    {
        next = GameObject.Find("Next").GetComponent<Button>();

        // Bind depending on serving state
        if (hasServed)
        {
            SetNextButtonToNewCustomer();
        }
        else
        {
            SetNextButtonToKitchen();
        }
    }

    void FindOkayButton()
    {
        okay = GameObject.Find("Okay").GetComponent<Button>();
        okay.onClick.AddListener(NextDialogue);
    }

    void FindTryAgainButton()
    {
        tryAgain = GameObject.Find("TryAgain").GetComponent<Button>();
        tryAgain.onClick.AddListener(GoBackPreviousDialogue);
    }

    void FindPerfectButton()
    {
        perfect = GameObject.Find("CompletePerfect").GetComponent<Button>();
        perfect.onClick.AddListener(ServePerfectDrinksForTesting);
    }

    public void GoBackPreviousDialogue()
    {
        tutorialwrong = false;
        hasServed = false;
        tutorialPhase -= 1;
        tryAgain.gameObject.SetActive(false);
        okay.gameObject.SetActive(true);
        LoadTutorialDialogue();
        return;
    }

    public void NextDialogue()
    {
        if ((tutorialPhase == 5 || tutorialPhase == 7) && SaveManager.saveManager.DayCount == 0 && !tutorialserve)
        {
            UpdateOkayButton();
            return;
        }

        tutorialPhase += 1;
        hasServed = false;

        if (tutorialPhase != 5 && tutorialPhase != 7)
        {
            tutorialserve = false;
        }

        if (tutorialPhase == 4 || tutorialPhase == 5 || tutorialPhase == 7)
        {
            GenerateTutorialOrderOne();
        }

        // Check if tutorial dialogue sequence is completed
        if (tutorialPhase >= tutorialDialogue.Count)
        {
            tutorialComplete = true;
            isCustomerCount = true;
            isFavour = true;
            UiManager.uiManager.ShowFavour();
            UiManager.uiManager.UpdateFavor();
            UiManager.uiManager.ShowCustomerCount();
            // Force disable tutorial buttons once tutorial completes
            if (okay != null) okay.gameObject.SetActive(false);
            if (tryAgain != null) tryAgain.gameObject.SetActive(false);

            if (next != null) next.gameObject.SetActive(true);

            SaveManager.saveManager.setDayCount(1);
            GenerateNewCustomer();

            UiManager.uiManager.UpdateDayCount();
            UiManager.uiManager.ShowTimer();
            UiManager.uiManager.RestartTimer();
            return;
        }

        LoadTutorialDialogue();
    }

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
        isNext = false;
        isTryAgain = false;
        orderedRecipes.Clear();

        //DrinkRecipe kopi = recipeBook.AllRecipes.FirstOrDefault(r =>
        //string.Equals(r.drinkName, "Kopi", StringComparison.OrdinalIgnoreCase));

        //DrinkRecipe kopiO = recipeBook.AllRecipes.FirstOrDefault(r =>
        //string.Equals(r.drinkName, "Kopi O", StringComparison.OrdinalIgnoreCase));

        //orderedRecipes.Add(kopi);
        //orderedRecipes.Add(kopiO);
        //return;
    }
    public void GenerateTutorialOrderOne()
    {
        orderedRecipes.Clear();

        if (tutorialPhase == 4 || tutorialPhase == 5)
        {
            DrinkRecipe kopi = recipeBook.AllRecipes.FirstOrDefault(r =>
                string.Equals(r.drinkName, "Kopi", StringComparison.OrdinalIgnoreCase));

            if (kopi != null)
                orderedRecipes.Add(kopi);
        }
        else if (tutorialPhase == 7)
        {
            DrinkRecipe kopiO = recipeBook.AllRecipes.FirstOrDefault(r =>
                string.Equals(r.drinkName, "Kopi O", StringComparison.OrdinalIgnoreCase));

            if (kopiO != null)
                orderedRecipes.Add(kopiO);
        }
    }


    public void LoadTutorialDialogue()
    {
        canEvaluateTutorial = false;
        Debug.Log(tutorialPhase);

        if (tutorialwrong)
        {
            currentDialogueText = tutorialWrongDialogue;
            if (dialogueText != null)
                dialogueText.text = currentDialogueText;
            return;
        }

        // Explicitly allow both Phase 5 and Phase 7 to enable tutorial serving
        if (tutorialPhase == 5 || tutorialPhase == 7)
        {
            canEvaluateTutorial = true; // Fix: unlocks ServeOrder() execution

            if (!tutorialserve)
            {
                okay.gameObject.SetActive(true);
                tryAgain.gameObject.SetActive(false);
            }
            else
            {
                okay.gameObject.SetActive(true);
                tryAgain.gameObject.SetActive(false);
            }
        }

        if (tutorialDialogue != null && tutorialPhase < tutorialDialogue.Count)
        {
            currentDialogueText = tutorialDialogue[tutorialPhase];
        }

        if (dialogueText != null)
        {
            dialogueText.text = currentDialogueText;
        }
    }

    public void EvaluateTutorialOrderByName(List<Drink> servedDrinks)
    {
        servedDrinks ??= new List<Drink>();

        if (servedDrinks.Count != orderedRecipes.Count)
        {
            SetTutorialWrongState();
            return;
        }

        List<Drink> remainingServed = new List<Drink>(servedDrinks);

        foreach (DrinkRecipe requiredRecipe in orderedRecipes)
        {
            Drink match = remainingServed.FirstOrDefault(d =>
                d != null && string.Equals(d.drinkName, requiredRecipe.drinkName, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                remainingServed.Remove(match);
            }
            else
            {
                SetTutorialWrongState();
                return;
            }
        }

        // --- Order Correct ---
        tutorialwrong = false;
        tutorialserve = true;

        okay.gameObject.SetActive(true);
        tryAgain.gameObject.SetActive(false);

        // Advance to next dialogue step
        NextDialogue();
    }

    // Switches Next button behavior to enter Kitchen
    public void SetNextButtonToKitchen()
    {
        if (next != null)
        {
            next.onClick.RemoveAllListeners();
            next.onClick.AddListener(LoadKitchenScene);
            next.gameObject.SetActive(true);
        }
    }

    // Switches Next button behavior back to Generate New Customer
    public void SetNextButtonToNewCustomer()
    {
        if (next != null)
        {
            next.onClick.RemoveAllListeners();
            next.onClick.AddListener(GenerateNewCustomer);
            next.gameObject.SetActive(true);
        }
    }

    public void LoadKitchenScene()
    {
        SceneManager.LoadScene("KitchenScene");
    }


    private void SetTutorialWrongState()
{
    // --- Order Wrong ---
    tutorialwrong = true;
    hasServed = false; // Fix: Reset serve status so player can drag and try serving again
    currentDialogueText = tutorialWrongDialogue;
    
    if (dialogueText != null)
    {
        dialogueText.text = currentDialogueText;
    }

    tryAgain.gameObject.SetActive(true);
    okay.gameObject.SetActive(false);
}

    public void UpdateOkayButton()
    {
        isOkay = false;
        isTryAgain = false;
        SceneManager.LoadScene("KitchenScene");
    }

    public void RevertOkayButton()
    {

    }

    public void GenerateNewCustomer()
    {
        UiManager.uiManager.TurnOnTimer();
        if (UiManager.uiManager.ChallengeMode)
        {
            UiManager.uiManager.RestartTimer();
        }

        // Always ensure tutorial buttons are inactive during main gameplay loop
        if (okay != null) okay.gameObject.SetActive(false);
        if (tryAgain != null) tryAgain.gameObject.SetActive(false);

        SaveManager.saveManager.IncrementCustomerCount();
        UiManager.uiManager.UpdateCustomerCount();

        hasServed = false;

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

        if (trayDatabase != null)
            trayDatabase.ClearDatabase();

        int randomCustomerIndex;

        int totalCustomers = customerDatabase.AllCustomers.Count;

        // Standard cases
        if (totalCustomers == 1)
        {
            randomCustomerIndex = 0;
        }
        else if (totalCustomers == 2)
        {
            // For exactly 2 customers (indices 0 and 1), flip to the alternate index guaranteed
            randomCustomerIndex = (chosenCustomerSlot == 0) ? 1 : 0;
        }
        else
        {
            // For 3 or more customers, keep rolling until a different index is selected
            do
            {
                randomCustomerIndex = UnityEngine.Random.Range(0, totalCustomers);
            }
            while (randomCustomerIndex == chosenCustomerSlot);
        }

        // Store the newly selected index for the next run
        chosenCustomerSlot = randomCustomerIndex;
        currentCustomer = customerDatabase.AllCustomers[chosenCustomerSlot];

        SetCustomerSprite();

        orderedRecipes.Clear();
        int drinkCount = UnityEngine.Random.Range(minDrinks, maxDrinks + 1);

        for (int i = 0; i < drinkCount; i++)
        {
            int randomRecipeIndex = UnityEngine.Random.Range(0, recipeBook.AllRecipes.Count);
            orderedRecipes.Add(recipeBook.AllRecipes[randomRecipeIndex]);
        }

        if (trayDatabase != null)
            trayDatabase.MaxSlots = drinkCount;

        SetStartDialogue();

        SetNextButtonToKitchen();
    }

    private void SetCustomerSprite()
    {
        if (currentCustomer.CustomerSprite == null || currentCustomer.CustomerSprite.Count == 0)
        {
            Debug.LogWarning($"Customer '{currentCustomer.CustomerName}' has no sprites assigned.");
            return;
        }

        int totalSprites = currentCustomer.CustomerSprite.Count;

        if (totalSprites == 1)
        {
            chosenSpriteIndex = 0;
        }
        else if (totalSprites == 2)
        {
            // Toggle directly between index 0 and 1
            chosenSpriteIndex = (chosenSpriteIndex == 0) ? 1 : 0;
        }
        else
        {
            // For 3 or more variations, keep rolling until a different sprite index is selected
            int previousIndex = chosenSpriteIndex;
            do
            {
                chosenSpriteIndex = UnityEngine.Random.Range(0, totalSprites);
            }
            while (chosenSpriteIndex == previousIndex);
        }

        currentSprite = currentCustomer.CustomerSprite[chosenSpriteIndex];

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
        // Check if we are in Day 0 / Tutorial
        bool isTutorial = SaveManager.saveManager != null && SaveManager.saveManager.DayCount == 0;

        // Validate state: standard customers require currentCustomer, tutorial only requires orderedRecipes
        if (!isTutorial && currentCustomer == null)
        {
            Debug.LogWarning("No current customer to serve.");
            return;
        }

        if (orderedRecipes == null || orderedRecipes.Count == 0)
        {
            Debug.LogWarning("No ordered recipes found to populate testing drinks.");
            return;
        }

        if (trayDatabase == null)
        {
            Debug.LogError("TrayDatabase is unassigned!");
            return;
        }

        // Clear existing drinks in tray before populating test drinks
        trayDatabase.ClearDatabase();

        // Populate the tray database with perfect drinks matching orderedRecipes
        foreach (DrinkRecipe recipe in orderedRecipes)
        {
            if (recipe == null) continue;

            Drink perfectDrink = new Drink
            {
                drinkName = recipe.drinkName,
                ingredients = new List<Ingredient>(recipe.ingredients),
                // drinkSprite = recipe.drinkImage
            };

            trayDatabase.AddDrink(perfectDrink);
        }

        Debug.Log($"[Testing] Added {orderedRecipes.Count} perfect drink(s) to TrayDatabase for {(isTutorial ? "Tutorial" : currentCustomer.CustomerName)}.");
    }
    public void ServeOrder()
    {
        UiManager.uiManager.TurnOffTimer();

        if (trayDatabase == null)
        {
            Debug.LogError("TrayDatabase is unassigned!");
            return;
        }

        // --- TUTORIAL / DAY 0 BYPASS ---
        if (SaveManager.saveManager.DayCount == 0)
        {
            if (tutorialPhase == 5 || tutorialPhase == 7)
            {
                if (!canEvaluateTutorial) return;
                EvaluateTutorialOrderByName(trayDatabase.SavedDrinks);
            }
            trayDatabase.ClearDatabase();
            return;
        }

        // --- STANDARD CUSTOMER LOGIC ---
        if (currentCustomer == null) return;

        EvaluateAndSetEndDialogue(trayDatabase.SavedDrinks);
        trayDatabase.ClearDatabase();

        // CUSTOMER SERVED: Re-enable Next button with GenerateNewCustomer functionality
        SetNextButtonToNewCustomer();
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
        bool baseIngredientMatched = false;
        List<Drink> remainingServed = new List<Drink>(servedDrinks);

        foreach (DrinkRecipe recipe in orderedRecipes)
        {
            Drink match = remainingServed.FirstOrDefault(served => IsDrinkPerfect(served, recipe));

            if (match != null)
            {
                perfectCount++;
                baseIngredientMatched = true;
                remainingServed.Remove(match);
            }
            else
            {
                // Find base ingredients required by the recipe (checks both recipe.ingredients and recipe.baseIngredient)
                List<Ingredient> baseIngredientsInRecipe = recipe.ingredients
                    .Where(i => i != null && i.IsBaseIngredient)
                    .ToList();

                if (recipe.baseIngredient != null && recipe.baseIngredient.Count > 0)
                {
                    baseIngredientsInRecipe.AddRange(recipe.baseIngredient.Where(i => i != null));
                }

                // Check if any served drink contains at least one of the required base ingredients
                foreach (Drink served in servedDrinks)
                {
                    if (served == null || served.ingredients == null) continue;

                    bool hasMatchingBase = served.ingredients.Any(servedIng =>
                        servedIng != null && baseIngredientsInRecipe.Any(reqBase =>
                            string.Equals(servedIng.Id, reqBase.Id, StringComparison.OrdinalIgnoreCase)
                        )
                    );

                    if (hasMatchingBase)
                    {
                        baseIngredientMatched = true;
                        break;
                    }
                }
            }
        }

        List<string> frontList;
        List<string> backList;

        if (perfectCount == orderedRecipes.Count && servedDrinks.Count == orderedRecipes.Count)
        {
            frontList = currentCustomer.PerfectFrontDialogue;
            backList = currentCustomer.PerfectBackDialogue;

            if (currentCustomer.goodOutComeSprite != null && currentCustomer.goodOutComeSprite.Count > 0)
            {
                int safeOutcomeIndex = Mathf.Clamp(chosenSpriteIndex, 0, currentCustomer.goodOutComeSprite.Count - 1);
                currentSprite = currentCustomer.goodOutComeSprite[safeOutcomeIndex];

                if (customerSpriteRenderer != null)
                {
                    customerSpriteRenderer.sprite = currentSprite;
                }
            }
        }
        else if (baseIngredientMatched)
        {
            frontList = currentCustomer.DecentFrontDialogue;
            backList = currentCustomer.DecentBackDialogue;
            SaveManager.saveManager.setFavour(50);
        }
        else
        {
            frontList = currentCustomer.WrongFrontDialogue;
            backList = currentCustomer.WrongBackDialogue;
        }

        for (int i = 0; i < perfectCount; i++)
        {
            SaveManager.saveManager.setFavour(100);
        }
        UiManager.uiManager.UpdateFavor();

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

        List<Ingredient> remainingServed = new List<Ingredient>(served);
        List<Ingredient> remainingRequired = new List<Ingredient>(required);

        List<Ingredient> nonMatchedIngredients = new List<Ingredient>();

        // Step 1: Compare and match Base Ingredient first
        Ingredient requiredBase = remainingRequired.FirstOrDefault(i => i != null && i.IsBaseIngredient);

        if (requiredBase != null)
        {
            Ingredient servedBaseMatch = remainingServed.FirstOrDefault(s => s != null && s.Id == requiredBase.Id);

            if (servedBaseMatch != null)
            {
                Debug.Log($"[Base Match] Matched base ingredient: {requiredBase.name}");
                remainingRequired.Remove(requiredBase);
                remainingServed.Remove(servedBaseMatch);
            }
            else
            {
                Debug.LogWarning($"[Base Mismatch] Base ingredient mismatch or missing: {requiredBase.name}");
                nonMatchedIngredients.Add(requiredBase);
                remainingRequired.Remove(requiredBase);
            }
        }

        // Step 2: Compare and pair remaining non-base ingredients
        foreach (Ingredient req in remainingRequired.ToList())
        {
            if (req == null) continue;

            Ingredient match = remainingServed.FirstOrDefault(s => s != null && s.Id == req.Id);

            if (match != null)
            {
                Debug.Log($"[Ingredient Match] Matched correct ingredient: {req.name}");
                remainingServed.Remove(match);
            }
            else
            {
                nonMatchedIngredients.Add(req);
            }
        }

        // Step 3: Add leftover served ingredients (extra/wrong additions)
        nonMatchedIngredients.AddRange(remainingServed);

        // Step 4: Store non-matched ingredients into a formatted string
        if (nonMatchedIngredients.Count > 0)
        {
            lastNonMatchedIngredientsText = string.Join(", ", nonMatchedIngredients
                .Where(i => i != null)
                .Select(i => i.name));

            Debug.LogWarning($"[Mismatches Detected] Non-matched ingredients: \"{lastNonMatchedIngredientsText}\"");
        }
        else
        {
            lastNonMatchedIngredientsText = "";
        }

        return nonMatchedIngredients.Count == 0;
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


    // ================================ REVAMP CODES ================================
    // [RULES]
    // [NOTE] Do Not Change Any Codes Here Unless Needed
    // - core variables
    // - functions for calculation and modifying said variables
    // - function starts with Capital
    // - variable starts with small
    // ================================ START ================================

}
