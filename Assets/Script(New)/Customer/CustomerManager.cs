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
    private string tutorialWrongDialogue = "Thats the wrong drink, could you do it again?";
    [SerializeField] private Sprite TutorialBoss;

    //
    private bool hasServed = false;

    [Header("Order Settings")]
    [SerializeField] private int minDrinks = 1;
    [SerializeField] private int maxDrinks = 3; // inclusive

    // swapped out from public to private var testing
    [SerializeField] private Button next;
    [SerializeField] private Button okay;

    [SerializeField] private Button tryAgain;
    [SerializeField] private Button perfect;

    private bool canEvaluateTutorial = false;

    private bool isNext;
    private bool isOkay;
    private bool isTryAgain;

    public int tutorialPhase;
    private bool tutorialwrong;

    public bool tutorialserve = false;

    public bool tutorialComplete = false;
    //getters

    public bool IsNext => isNext;
    public bool IsOkay => isOkay;
    public bool IsTryAgain => isTryAgain;

    public Button NextButton => next;
    public Button OkayButton => okay;
    public Button TryAgainButton => tryAgain;

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
        UiManager.uiManager.UpdateDayCount();
        if (currentCustomer == null)
        {
            //GenerateNewCustomer();
            tutorialwrong = false;
            tryAgain.gameObject.SetActive(false);
            okay.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            if (SaveManager.saveManager.DayCount == 0)
            {
                okay.gameObject.SetActive(true);
                customerSpriteRenderer.sprite = TutorialBoss;
                currentSprite = TutorialBoss;
                tutorialPhase = 0;
                GenerateTutorialDayZero();
                LoadTutorialDialogue();
            }
            else
            {
                next.gameObject.SetActive(true);
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
            FindPerfectButton();
            return;
        }
    }

    void FindNextButton()
    {
        next = GameObject.Find("Next").GetComponent<Button>();
        next.onClick.AddListener(GenerateNewCustomer);
        //next.onClick.AddListener(next.GetComponent<CustomerManagerUIBridge>().OnGenerateNewCustomerClicked);
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
        // If on tutorial index 5, clicking OKAY transitions to the Kitchen scene
        if (tutorialPhase == 5 && SaveManager.saveManager.DayCount == 0 && !tutorialserve)
        {
            UpdateOkayButton();
            return;
        }

        tutorialPhase += 1;

        // Check if tutorial dialogue sequence is completed
        if (tutorialPhase >= tutorialDialogue.Count)
        {
            tutorialComplete = true;
            okay.gameObject.SetActive(false);
            next.gameObject.SetActive(true);

            // Advance to Day 1 and generate first real customer
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

        DrinkRecipe kopi = recipeBook.AllRecipes.FirstOrDefault(r =>
        string.Equals(r.drinkName, "Kopi", StringComparison.OrdinalIgnoreCase));

        DrinkRecipe kopiO = recipeBook.AllRecipes.FirstOrDefault(r =>
            string.Equals(r.drinkName, "Kopi O", StringComparison.OrdinalIgnoreCase));

        orderedRecipes.Add(kopi);
        orderedRecipes.Add(kopiO);
        return;
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

        // Phase 5 is the ordering / kitchen transition step
        if (tutorialPhase == 5)
        {
            canEvaluateTutorial = true;

            if (!tutorialserve)
            {
                // First time on index 5: OKAY button takes player to kitchen
                okay.gameObject.SetActive(true);
                tryAgain.gameObject.SetActive(false);
            }
            else
            {
                // Returned from kitchen & served correctly: show OKAY button to move to dialogue index 6
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
        tutorialserve = true; // Mark as successfully served

        // Enable OKAY button so player can proceed to next dialogue
        okay.gameObject.SetActive(true);
        tryAgain.gameObject.SetActive(false);

        // Advance to dialogue index 6
        NextDialogue();
    }

    private void SetTutorialWrongState()
    {
        // --- Order Wrong ---
        tutorialwrong = true;
        currentDialogueText = tutorialWrongDialogue;
        if (dialogueText != null)
        {
            dialogueText.text = currentDialogueText;
        }

        tryAgain.gameObject.SetActive(true); // Unlock try again button
        okay.gameObject.SetActive(false);     // Hide okay button
    }

    public void UpdateOkayButton()
    {
        SceneManager.LoadScene("KitchenRearranged");
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
        //UiManager.uiManager.RestartTimer();

        SaveManager.saveManager.setDayCount(1);
        

        UiManager.uiManager.UpdateCustomerCount();
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
                drinkSprite = recipe.drinkImage
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
            if (tutorialPhase == 5)
            {
                if (!canEvaluateTutorial)
                {
                    Debug.Log("Tutorial evaluation locked. Reach Phase 5 dialogue first.");
                    return;
                }
                EvaluateTutorialOrderByName(trayDatabase.SavedDrinks);
            }
            trayDatabase.ClearDatabase();
            return; // Exit here so non-tutorial checks don't trigger
        }

        // --- STANDARD CUSTOMER LOGIC ---
        if (currentCustomer == null)
        {
            Debug.LogWarning("No current customer to serve.");
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

    //origin
    //public void EvaluateAndSetEndDialogue(List<Drink> servedDrinks)
    //{
    //    if (currentCustomer == null || orderedRecipes.Count == 0) return;

    //    servedDrinks ??= new List<Drink>();

    //    int perfectCount = 0;

    //    for (int i = 0; i < orderedRecipes.Count; i++)
    //    {
    //        DrinkRecipe recipe = orderedRecipes[i];
    //        Drink served = i < servedDrinks.Count ? servedDrinks[i] : null;

    //        if (IsDrinkPerfect(served, recipe))
    //            perfectCount++;
    //    }

    //    List<string> frontList;
    //    List<string> backList;

    //    if (perfectCount == orderedRecipes.Count)
    //    {
    //        // all drinks correct
    //        frontList = currentCustomer.PerfectFrontDialogue;
    //        backList = currentCustomer.PerfectBackDialogue;
    //        //SaveManager.saveManager.setTutorialPhase(1);
    //    }
    //    else if (perfectCount == 0)
    //    {
    //        // all drinks wrong
    //        frontList = currentCustomer.WrongFrontDialogue;
    //        backList = currentCustomer.WrongBackDialogue;
    //    }
    //    else
    //    {
    //        // at least one wrong, but not all
    //        frontList = currentCustomer.DecentFrontDialogue;
    //        backList = currentCustomer.DecentBackDialogue;
    //    }

    //    int index = GetRandomDialogueIndex(frontList, backList);
    //    if (index == -1)
    //    {
    //        if (dialogueText != null) dialogueText.text = "...";
    //        return;
    //    }

    //    SetEndDialogueText(frontList[index], backList[index]);
    //}

    //second prototype
    //public void EvaluateAndSetEndDialogue(List<Drink> servedDrinks)
    //{
    //    if (currentCustomer == null || orderedRecipes.Count == 0) return;

    //    servedDrinks ??= new List<Drink>();

    //    int perfectCount = 0;
    //    List<Drink> remainingServed = new List<Drink>(servedDrinks);

    //    // Unordered matching: check each required recipe against remaining tray drinks
    //    foreach (DrinkRecipe recipe in orderedRecipes)
    //    {
    //        Drink match = remainingServed.FirstOrDefault(served => IsDrinkPerfect(served, recipe));

    //        if (match != null)
    //        {
    //            perfectCount++;
    //            remainingServed.Remove(match); // Consume the drink so it isn't matched twice
    //        }
    //    }

    //    List<string> frontList;
    //    List<string> backList;

    //    if (perfectCount == orderedRecipes.Count && servedDrinks.Count == orderedRecipes.Count)
    //    {
    //        // All ordered drinks matched perfectly and no extra/missing drinks
    //        frontList = currentCustomer.PerfectFrontDialogue;
    //        backList = currentCustomer.PerfectBackDialogue;
    //    }
    //    else if (perfectCount == 0)
    //    {
    //        // All drinks wrong
    //        frontList = currentCustomer.WrongFrontDialogue;
    //        backList = currentCustomer.WrongBackDialogue;
    //    }
    //    else
    //    {
    //        // Partially correct order
    //        frontList = currentCustomer.DecentFrontDialogue;
    //        backList = currentCustomer.DecentBackDialogue;
    //    }

    //    int index = GetRandomDialogueIndex(frontList, backList);
    //    if (index == -1)
    //    {
    //        if (dialogueText != null) dialogueText.text = "...";
    //        return;
    //    }

    //    SetEndDialogueText(frontList[index], backList[index]);
    //}

    public void EvaluateAndSetEndDialogue(List<Drink> servedDrinks)
    {
        if (currentCustomer == null || orderedRecipes.Count == 0) return;

        servedDrinks ??= new List<Drink>();

        int perfectCount = 0;
        List<Drink> remainingServed = new List<Drink>(servedDrinks);

        // Unordered matching: check each recipe against any available drink in tray
        foreach (DrinkRecipe recipe in orderedRecipes)
        {
            Drink match = remainingServed.FirstOrDefault(served => IsDrinkPerfect(served, recipe));

            if (match != null)
            {
                perfectCount++;
                remainingServed.Remove(match); // Prevents matching the same drink twice
            }
        }

        List<string> frontList;
        List<string> backList;

        if (perfectCount == orderedRecipes.Count && servedDrinks.Count == orderedRecipes.Count)
        {
            frontList = currentCustomer.PerfectFrontDialogue;
            backList = currentCustomer.PerfectBackDialogue;
        }
        else if (perfectCount == 0)
        {
            frontList = currentCustomer.WrongFrontDialogue;
            backList = currentCustomer.WrongBackDialogue;
        }
        else
        {
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
