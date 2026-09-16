using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReworkedCustomerManager : MonoBehaviour
{
    public static ReworkedCustomerManager instance { get; private set; }

    [Header("Database (Revamp)")]
    [SerializeField] private CustomerDatabase _customerDatabase;
    [SerializeField] private RecipeBook _recipeBook;
    [SerializeField] private TrayDatabase _trayDatabase;

    [Header("UI Display Referencing (Revamp)")]
    [SerializeField] private SpriteRenderer _customerSpriteRenderer;
    [SerializeField] private TMP_Text _dialogueText;

    [Header("Buttons")]
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _okayButton;
    [SerializeField] private Button _retryButton;

    [Header("General Variables (Revamp)")]
    [SerializeField] private int _minDrink = 1;
    [SerializeField] private int _maxDrink = 1;
    private int _chosenSpriteIndex;
    private int _availableRecipeCount;
    private string _currentDialogueText = "";
    private string _lastNonMatchedIngredientText = "";

    public enum GameplayState
    {
        Tutorial,
        GeneralGameplay,
        DaySummary
    }

    public enum CounterState
    {
        ReadDialogue,
        TakingOrder,
        ServingOrder,
        RedoOrder,
    }

    public CounterState CurrentCounterState;
    public GameplayState CurrentGameplayState;

    Scene counterScene;

    private CustomerData _currentCustomer;
    private Sprite _currentSprite;
    private List<DrinkRecipe> orderedRecipes = new();

    [Header("Tutorial Variables (Revamp)")]
    [SerializeField] private Sprite _tutorialSprite;
    [SerializeField] private List<string> _tutorialDialogue;
    private string _tutorialWrongOrderDialogue = "Hmm, that's not right. Try again.";
    [SerializeField] private bool skip;

    public string CurrentDialogue => _currentDialogueText;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        counterScene = SceneManager.GetActiveScene();

        if (counterScene.name == "CounterScene")
        {
            FindNextButton();
            FindOkayButton();
            FindRetryButton();
        }
        if (skip)
        {
            ReworkedSaveManager.instance.Day = 1;
        }

        if (_availableRecipeCount <= 0)
        {
            _availableRecipeCount = 2;
        }

        if (ReworkedSaveManager.instance != null && ReworkedSaveManager.instance.Day == 0)
        {
            ReworkedSaveManager.instance.TutorialPhase = 0;
            CurrentGameplayState = GameplayState.Tutorial;
            CurrentCounterState = CounterState.ReadDialogue;
            LoadTutorialDialogue();
        }
        else
        {
            CurrentGameplayState = GameplayState.GeneralGameplay;
            GenerateCustomer();
        }

        EvaluateAndUpdateGameplayState();
        EvaluateAndUpdateCounterState();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CounterScene")
        {
            FindNextButton();
            FindOkayButton();
            FindRetryButton();

            if (CurrentCounterState == CounterState.TakingOrder)
            {
                CurrentCounterState = CounterState.ServingOrder;
            }

            EvaluateAndUpdateGameplayState();
            EvaluateAndUpdateCounterState();
        }
    }

    void FindNextButton()
    {
        GameObject nextObj = GameObject.Find("Next");
        if (nextObj != null)
        {
            _nextButton = nextObj.GetComponent<Button>();
            _nextButton.onClick.RemoveAllListeners();
            if (CurrentCounterState == CounterState.ServingOrder)
            {
                _nextButton.onClick.AddListener(GenerateCustomer);
            }
            else
            {
                _nextButton.onClick.AddListener(ToKitchen);
            }
        }
    }

    void FindOkayButton()
    {
        GameObject okayObj = GameObject.Find("Okay");
        if (okayObj != null)
        {
            _okayButton = okayObj.GetComponent<Button>();
            _okayButton.onClick.RemoveAllListeners();
            _okayButton.onClick.AddListener(NextDialogue);
        }
    }

    void FindRetryButton()
    {
        GameObject retryObj = GameObject.Find("TryAgain");
        if (retryObj != null)
        {
            _retryButton = retryObj.GetComponent<Button>();
            _retryButton.onClick.RemoveAllListeners();
            _retryButton.onClick.AddListener(PreviousDialogue);
        }
    }

    // ================================ TUTORIAL FUNCTIONS ================================
    public void GenerateTutorialOrder()
    {
        orderedRecipes.Clear();

        if (ReworkedSaveManager.instance.TutorialPhase == 4 || ReworkedSaveManager.instance.TutorialPhase == 5)
        {
            DrinkRecipe kopi = _recipeBook.AllRecipes.FirstOrDefault(r =>
                string.Equals(r.drinkName, "Kopi", StringComparison.OrdinalIgnoreCase));

            if (kopi != null)
                orderedRecipes.Add(kopi);
        }
        else if (ReworkedSaveManager.instance.TutorialPhase == 7)
        {
            DrinkRecipe kopiO = _recipeBook.AllRecipes.FirstOrDefault(r =>
                string.Equals(r.drinkName, "Kopi O", StringComparison.OrdinalIgnoreCase));

            if (kopiO != null)
                orderedRecipes.Add(kopiO);
        }
    }

    public void NextDialogue()
    {
        if ((ReworkedSaveManager.instance.TutorialPhase == 5 || ReworkedSaveManager.instance.TutorialPhase == 7)
            && ReworkedSaveManager.instance.Day == 0 && CurrentGameplayState == GameplayState.Tutorial)
        {
            GenerateTutorialOrder();
            ToKitchen();
            CurrentCounterState = CounterState.ServingOrder;
            EvaluateAndUpdateCounterState();
            return;
        }

        ReworkedSaveManager.instance.TutorialPhase += 1;

        if (ReworkedSaveManager.instance.TutorialPhase == 4 || ReworkedSaveManager.instance.TutorialPhase == 5 || ReworkedSaveManager.instance.TutorialPhase == 7)
        {
            GenerateTutorialOrder();
        }

        if (ReworkedSaveManager.instance.TutorialPhase >= _tutorialDialogue.Count)
        {
            ReworkedSaveManager.instance.Day = 1;
            CurrentGameplayState = GameplayState.GeneralGameplay;
            EvaluateAndUpdateGameplayState();

            if (ReworkedUIManager.instance != null)
            {
                ReworkedUIManager.instance.RestartTimer();
                ReworkedUIManager.instance.UpdateDayDisplayText();
            }

            GenerateCustomer();
            return;
        }
        LoadTutorialDialogue();

        CurrentCounterState = CounterState.ReadDialogue;
        EvaluateAndUpdateCounterState();
    }

    public void LoadTutorialDialogue()
    {
        if (_tutorialDialogue == null || _dialogueText == null) return;

        int phase = ReworkedSaveManager.instance.TutorialPhase;
        if (phase < 0 || phase >= _tutorialDialogue.Count) return;

        _currentDialogueText = _tutorialDialogue[phase];
        _dialogueText.text = _currentDialogueText;
    }

    public void PreviousDialogue()
    {
        LoadTutorialDialogue();
        CurrentCounterState = CounterState.ReadDialogue;
        EvaluateAndUpdateCounterState();
    }

    public void SetTutorialWrong()
    {
        _currentDialogueText = _tutorialWrongOrderDialogue;
        if (_dialogueText != null)
            _dialogueText.text = _currentDialogueText;

        CurrentCounterState = CounterState.RedoOrder;
        EvaluateAndUpdateCounterState();
    }

    public void ToKitchen()
    {
        SceneManager.LoadScene("KitchenScene");
    }

    public void EvaluateTutorialOrder(List<Drink> ServedDrinks)
    {
        ServedDrinks ??= new List<Drink>();
        if (ServedDrinks.Count != orderedRecipes.Count)
        {
            SetTutorialWrong();
            return;
        }
        List<Drink> remainingServed = new List<Drink>(ServedDrinks);

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
                SetTutorialWrong();
                return;
            }
        }

        ReworkedSaveManager.instance.TutorialPhase += 1;

        if (ReworkedSaveManager.instance.TutorialPhase >= _tutorialDialogue.Count)
        {
            ReworkedSaveManager.instance.Day = 1;
            CurrentGameplayState = GameplayState.GeneralGameplay;
            EvaluateAndUpdateGameplayState();

            if (ReworkedUIManager.instance != null)
            {
                ReworkedUIManager.instance.RestartTimer();
                ReworkedUIManager.instance.UpdateDayDisplayText();
            }

            GenerateCustomer();
            return;
        }

        LoadTutorialDialogue();
        CurrentCounterState = CounterState.ReadDialogue;
        EvaluateAndUpdateCounterState();
    }

    // ================================ GENERAL FUNCTIONS ================================
    public void EvaluateAndUpdateCounterState()
    {
        if (_okayButton == null || _nextButton == null || _retryButton == null)
            return;

        switch (CurrentCounterState)
        {
            case CounterState.ReadDialogue:
                _okayButton.gameObject.SetActive(true);
                _nextButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(false);
                break;

            case CounterState.TakingOrder:
                _okayButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(false);

                if (CurrentGameplayState == GameplayState.Tutorial)
                {
                    _okayButton.gameObject.SetActive(true);
                    _nextButton.gameObject.SetActive(false);
                }
                else if (CurrentGameplayState == GameplayState.GeneralGameplay)
                {
                    _nextButton.gameObject.SetActive(true);
                    _nextButton.onClick.RemoveAllListeners();
                    _nextButton.onClick.AddListener(ToKitchen);
                }
                break;

            case CounterState.ServingOrder:
                _okayButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(false);
                break;

            case CounterState.RedoOrder:
                _okayButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(true);
                break;
        }
    }

    public void EvaluateAndUpdateGameplayState()
    {
        if (ReworkedUIManager.instance == null) return;

        switch (CurrentGameplayState)
        {
            case GameplayState.Tutorial:
                ReworkedUIManager.instance.CustomerCountText.gameObject.SetActive(false);
                ReworkedUIManager.instance.TimerText.gameObject.SetActive(false);
                ReworkedUIManager.instance.DayText.gameObject.SetActive(true);
                ReworkedUIManager.instance.FavourText.gameObject.SetActive(true);

                if (ReworkedSaveManager.instance.Day == 0)
                {
                    if (_customerSpriteRenderer != null)
                        _customerSpriteRenderer.sprite = _tutorialSprite;

                    _currentSprite = _tutorialSprite;
                    GenerateTutorialOrder();
                    LoadTutorialDialogue();
                }
                break;

            case GameplayState.GeneralGameplay:
                ReworkedUIManager.instance.CustomerCountText.gameObject.SetActive(true);
                ReworkedUIManager.instance.TimerText.gameObject.SetActive(true);
                ReworkedUIManager.instance.DayText.gameObject.SetActive(true);
                ReworkedUIManager.instance.FavourText.gameObject.SetActive(true);

                ReworkedUIManager.instance.UpdateCustomerDisplayText();
                ReworkedUIManager.instance.UpdateFavourDisplayText();
                ReworkedUIManager.instance.UpdateDayDisplayText();
                break;

            case GameplayState.DaySummary:
                ReworkedUIManager.instance.CustomerCountText.gameObject.SetActive(false);
                ReworkedUIManager.instance.TimerText.gameObject.SetActive(false);
                ReworkedUIManager.instance.DayText.gameObject.SetActive(false);
                ReworkedUIManager.instance.FavourText.gameObject.SetActive(false);
                break;
        }
    }

    public float GetEventSpawnChance()
    {
        if (ReworkedSaveManager.instance == null || ReworkedSaveManager.instance.CurrentEvent <= 0)
            return 0f;

        int remainingCustomers = ReworkedSaveManager.instance.MaxCustomer - ReworkedSaveManager.instance.CurrentCustomer;

        if (remainingCustomers <= 0)
            return 1f;

        float chance = (float)ReworkedSaveManager.instance.CurrentEvent / remainingCustomers;
        return Mathf.Clamp01(chance);
    }

    public void GenerateCustomer()
    {
        //if (ReworkedUIManager.instance != null)
        //{
        //    ReworkedUIManager.instance.IsTimerRunning = true;
        //    if (ReworkedUIManager.instance.ChallengeMode)
        //    {
        //        ReworkedUIManager.instance.RestartTimer();
        //    }
        //}

        if (ReworkedUIManager.instance != null)
        {
            ReworkedUIManager.instance.RestartTimer();
        }

        if (_trayDatabase != null)
            _trayDatabase.ClearDatabase();

        if (ReworkedSaveManager.instance != null && ReworkedSaveManager.instance.CurrentEvent > 0)
        {
            float spawnChance = GetEventSpawnChance();
            int remainingCustomers = ReworkedSaveManager.instance.MaxCustomer - ReworkedSaveManager.instance.CurrentCustomer;

            Debug.Log($"[Event System] Remaining: {remainingCustomers} | Current Event: {ReworkedSaveManager.instance.CurrentEvent} | Spawn Chance: {spawnChance * 100f}%");

            if (UnityEngine.Random.value <= spawnChance)
            {
                int eventNum = UnityEngine.Random.Range(0, 1);
                GenerateSpecialCustomer(eventNum);
            }
            else
            {
                GenerateNormalCustomer();
            }
        }
        else
        {
            GenerateNormalCustomer();
        }

        SetStartDialogue();
        CurrentCounterState = CounterState.TakingOrder;
        EvaluateAndUpdateCounterState();
    }

    public void GenerateSpecialCustomer(int eventNum)
    {
        CustomerData selectedData = null;

        if (eventNum == 0 && _customerDatabase != null && _customerDatabase.AllTouristCustomer != null && _customerDatabase.AllTouristCustomer.Count > 0)
        {
            int chosenIndex = RandomIndex(_customerDatabase.AllTouristCustomer.Count);
            selectedData = _customerDatabase.AllTouristCustomer[chosenIndex];
        }
        else if (eventNum == 1 && _customerDatabase != null && _customerDatabase.AllSleepDeprivedCustomer != null && _customerDatabase.AllSleepDeprivedCustomer.Count > 0)
        {
            int chosenIndex = RandomIndex(_customerDatabase.AllSleepDeprivedCustomer.Count);
            selectedData = _customerDatabase.AllSleepDeprivedCustomer[chosenIndex];
        }

        // Check if selected special customer has valid sprites and dialogues
        bool isValidCustomer = selectedData != null &&
                               selectedData.CustomerSprite != null && selectedData.CustomerSprite.Count > 0 &&
                               selectedData.StartFrontDialogue != null && selectedData.StartFrontDialogue.Count > 0 &&
                               selectedData.StartBackDialogue != null && selectedData.StartBackDialogue.Count > 0;

        if (!isValidCustomer)
        {
            Debug.LogWarning("[ReworkedCustomerManager] Special customer data missing sprites or dialogues. Falling back to normal customer.");
            GenerateNormalCustomer();
            return;
        }

        _currentCustomer = selectedData;
        SetCustomerSprite();

        if (eventNum == 0)
            PopulateTouristCustomerOrders();
        else
            PopulateCustomerOrders();

        if (ReworkedSaveManager.instance != null)
            ReworkedSaveManager.instance.CurrentEvent--;
    }

    public void GenerateNormalCustomer()
    {
        if (_customerDatabase == null || _customerDatabase.AllCustomers == null || _customerDatabase.AllCustomers.Count == 0) return;

        int TotalCustomer = _customerDatabase.AllCustomers.Count;
        int ChosenIndex = RandomIndex(TotalCustomer);
        _currentCustomer = _customerDatabase.AllCustomers[ChosenIndex];
        SetCustomerSprite();

        PopulateCustomerOrders();
    }

    private void PopulateCustomerOrders()
    {
        orderedRecipes.Clear();
        int drinkCount = UnityEngine.Random.Range(_minDrink, _maxDrink + 1);

        int limit = (_availableRecipeCount > 0 && _availableRecipeCount <= _recipeBook.AllRecipes.Count)
            ? _availableRecipeCount
            : _recipeBook.AllRecipes.Count;

        for (int i = 0; i < drinkCount; i++)
        {
            int randomRecipeIndex = RandomIndex(limit);
            orderedRecipes.Add(_recipeBook.AllRecipes[randomRecipeIndex]);
        }

        if (_trayDatabase != null)
            _trayDatabase.MaxSlots = drinkCount;
    }

    private void PopulateTouristCustomerOrders()
    {
        if (_recipeBook == null || _recipeBook.TouristUniqueRecipe == null || _recipeBook.TouristUniqueRecipe.Count == 0)
        {
            Debug.LogWarning("[ReworkedCustomerManager] TouristUniqueRecipe list is empty or unassigned. Falling back to AllRecipes.");
            PopulateCustomerOrders();
            return;
        }

        orderedRecipes.Clear();
        int drinkCount = UnityEngine.Random.Range(_minDrink, _maxDrink + 1);

        for (int i = 0; i < drinkCount; i++)
        {
            int limit = _recipeBook.TouristUniqueRecipe.Count;
            int randomRecipeIndex = RandomIndex(limit);
            orderedRecipes.Add(_recipeBook.TouristUniqueRecipe[randomRecipeIndex]);
        }

        if (_trayDatabase != null)
            _trayDatabase.MaxSlots = drinkCount;
    }

    public int RandomIndex(int maxNum)
    {
        if (maxNum <= 0) return 0;
        return UnityEngine.Random.Range(0, maxNum);
    }

    public void SetCustomerSprite()
    {
        if (_currentCustomer == null || _currentCustomer.CustomerSprite == null) return;

        int TotalSprites = _currentCustomer.CustomerSprite.Count;
        if (TotalSprites <= 0) return;

        int newIndex = RandomIndex(TotalSprites);

        if (TotalSprites > 1 && ReworkedSaveManager.instance != null)
        {
            int guard = 0;
            while (newIndex == ReworkedSaveManager.instance.PreviousIndex && guard < 10)
            {
                newIndex = RandomIndex(TotalSprites);
                guard++;
            }
        }

        if (ReworkedSaveManager.instance != null)
            ReworkedSaveManager.instance.PreviousIndex = newIndex;

        _chosenSpriteIndex = newIndex;
        _currentSprite = _currentCustomer.CustomerSprite[newIndex];

        if (_customerSpriteRenderer != null)
        {
            _customerSpriteRenderer.sprite = _currentSprite;
        }
    }

    public void SetStartDialogue()
    {
        if (_currentCustomer == null) return;

        int index = GetRandomDialogueIndex(_currentCustomer.StartFrontDialogue, _currentCustomer.StartBackDialogue);
        if (index == -1)
        {
            if (_dialogueText != null) _dialogueText.text = "...";
            return;
        }

        string front = _currentCustomer.StartFrontDialogue[index];
        string back = _currentCustomer.StartBackDialogue[index];

        SetDialogueText(front, back);
    }

    public void SetAvailableRecipes(int num)
    {
        _availableRecipeCount = num;
    }

    private void SetDialogueText(string front, string back)
    {
        string drinkListText = string.Join(", ", orderedRecipes.Select(r => r.drinkName));
        _currentDialogueText = $"{front}{drinkListText} {back}";

        if (_dialogueText != null)
            _dialogueText.text = _currentDialogueText;
    }

    public void ProcessOrder()
    {
        if (ReworkedUIManager.instance != null)
            ReworkedUIManager.instance.IsTimerRunning = false;

        if (CurrentGameplayState == GameplayState.Tutorial)
        {
            EvaluateTutorialOrder(_trayDatabase.SavedDrinks);
            _trayDatabase.ClearDatabase();
            return;
        }
        else
        {
            EvaluateGeneralOrder(_trayDatabase.SavedDrinks);
            _trayDatabase.ClearDatabase();

            ReworkedSaveManager.instance.CurrentCustomer += 1;

            if (ReworkedUIManager.instance != null)
                ReworkedUIManager.instance.UpdateCustomerDisplayText();

            CurrentCounterState = CounterState.ReadDialogue;

            if (ReworkedSaveManager.instance.CurrentCustomer >= ReworkedSaveManager.instance.MaxCustomer)
            {
                ReworkedSaveManager.instance.EvaluateAndCalculateDay();
            }
            else
            {
                if (_okayButton != null) _okayButton.gameObject.SetActive(false);
                if (_retryButton != null) _retryButton.gameObject.SetActive(false);
                if (_nextButton != null)
                {
                    _nextButton.gameObject.SetActive(true);
                    _nextButton.onClick.RemoveAllListeners();
                    _nextButton.onClick.AddListener(GenerateCustomer);
                }
            }
            return;
        }
    }

    public void EvaluateGeneralOrder(List<Drink> ServedDrink)
    {
        if (_currentCustomer == null || orderedRecipes.Count == 0) return;

        ServedDrink ??= new List<Drink>();

        int PerfectCount = 0;
        bool BaseIngredientMatched = false;
        List<Drink> RemainingServed = new List<Drink>(ServedDrink);

        foreach (DrinkRecipe recipe in orderedRecipes)
        {
            Drink match = RemainingServed.FirstOrDefault(served => IsDrinkPerfect(served, recipe));

            if (match != null)
            {
                PerfectCount++;
                BaseIngredientMatched = true;
                RemainingServed.Remove(match);
            }
            else
            {
                // Collect all required base ingredients for this recipe
                List<Ingredient> baseIngredientsInRecipe = recipe.ingredients
                    .Where(i => i != null && i.IsBaseIngredient)
                    .ToList();

                if (recipe.baseIngredient != null && recipe.baseIngredient.Count > 0)
                {
                    baseIngredientsInRecipe.AddRange(recipe.baseIngredient.Where(i => i != null));
                }

                // Verify that ALL required base ingredients are matched
                if (baseIngredientsInRecipe.Count > 0)
                {
                    foreach (Drink served in ServedDrink)
                    {
                        if (served == null || served.ingredients == null) continue;

                        List<Ingredient> remainingServedIngredients = new List<Ingredient>(served.ingredients);
                        bool allBasesMatch = true;

                        foreach (Ingredient reqBase in baseIngredientsInRecipe)
                        {
                            Ingredient matchedBase = remainingServedIngredients.FirstOrDefault(s =>
                                s != null && string.Equals(s.Id, reqBase.Id, StringComparison.OrdinalIgnoreCase));

                            if (matchedBase != null)
                            {
                                remainingServedIngredients.Remove(matchedBase); // Consume matched ingredient
                            }
                            else
                            {
                                allBasesMatch = false; // Missing a required base ingredient
                                break;
                            }
                        }

                        if (allBasesMatch)
                        {
                            BaseIngredientMatched = true;
                            break;
                        }
                    }
                }
            }
        }

        List<string> frontList;
        List<string> backList;

        if (PerfectCount == orderedRecipes.Count && ServedDrink.Count == orderedRecipes.Count)
        {
            frontList = _currentCustomer.PerfectFrontDialogue;
            backList = _currentCustomer.PerfectBackDialogue;

            if (_currentCustomer.goodOutComeSprite != null && _currentCustomer.goodOutComeSprite.Count > 0)
            {
                int safeOutcomeIndex = Mathf.Clamp(_chosenSpriteIndex, 0, _currentCustomer.goodOutComeSprite.Count - 1);
                _currentSprite = _currentCustomer.goodOutComeSprite[safeOutcomeIndex];

                if (_customerSpriteRenderer != null)
                {
                    _customerSpriteRenderer.sprite = _currentSprite;
                }
            }
            ReworkedSaveManager.instance.CorrectOrders++;
        }
        else if (BaseIngredientMatched)
        {
            frontList = _currentCustomer.DecentFrontDialogue;
            backList = _currentCustomer.DecentBackDialogue;
            ReworkedSaveManager.instance.Favour += 50;
            ReworkedSaveManager.instance.PartialOrders++;
        }
        else
        {
            frontList = _currentCustomer.WrongFrontDialogue;
            backList = _currentCustomer.WrongBackDialogue;
            ReworkedSaveManager.instance.WrongOrders++;
        }

        for (int i = 0; i < PerfectCount; i++)
        {
            ReworkedSaveManager.instance.Favour += 100;
        }

        if (ReworkedUIManager.instance != null)
            ReworkedUIManager.instance.UpdateFavourDisplayText();

        int maxIndex = Mathf.Min(frontList?.Count ?? 0, backList?.Count ?? 0);
        int index = RandomIndex(maxIndex);

        if (index < 0 || maxIndex == 0)
        {
            if (_dialogueText != null) _dialogueText.text = "...";
            return;
        }

        SetEndDialogueText(frontList[index], backList[index]);
    }

    private bool IsDrinkPerfect(Drink Served, DrinkRecipe recipe)
    {
        if (Served == null || recipe == null) return false;

        bool nameMatch = string.Equals(Served.drinkName, recipe.drinkName, StringComparison.OrdinalIgnoreCase);
        if (!nameMatch) return false;

        return IngredientsMatch(Served.ingredients, recipe.ingredients);
    }

    public void SetEndDialogueText(string Front, string Back)
    {
        _currentDialogueText = $"{Front}{Back}";
        if (_dialogueText != null)
            _dialogueText.text = _currentDialogueText;
    }

    private bool IngredientsMatch(List<Ingredient> served, List<Ingredient> required)
    {
        served ??= new List<Ingredient>();
        required ??= new List<Ingredient>();

        List<Ingredient> remainingServed = new List<Ingredient>(served);
        List<Ingredient> remainingRequired = new List<Ingredient>(required);

        List<Ingredient> nonMatchedIngredients = new List<Ingredient>();

        Ingredient requiredBase = remainingRequired.FirstOrDefault(i => i != null && i.IsBaseIngredient);

        if (requiredBase != null)
        {
            Ingredient servedBaseMatch = remainingServed.FirstOrDefault(s => s != null && s.Id == requiredBase.Id);

            if (servedBaseMatch != null)
            {
                remainingRequired.Remove(requiredBase);
                remainingServed.Remove(servedBaseMatch);
            }
            else
            {
                nonMatchedIngredients.Add(requiredBase);
                remainingRequired.Remove(requiredBase);
            }
        }

        foreach (Ingredient req in remainingRequired.ToList())
        {
            if (req == null) continue;

            Ingredient match = remainingServed.FirstOrDefault(s => s != null && s.Id == req.Id);

            if (match != null)
            {
                remainingServed.Remove(match);
            }
            else
            {
                nonMatchedIngredients.Add(req);
            }
        }

        nonMatchedIngredients.AddRange(remainingServed);

        if (nonMatchedIngredients.Count > 0)
        {
            _lastNonMatchedIngredientText = string.Join(", ", nonMatchedIngredients
                .Where(i => i != null)
                .Select(i => i.name));
        }
        else
        {
            _lastNonMatchedIngredientText = "";
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

    public void RegisterDisplayReferences(SpriteRenderer spriteRenderer, TMP_Text text)
    {
        _customerSpriteRenderer = spriteRenderer;
        _dialogueText = text;

        if (_customerSpriteRenderer != null && _currentSprite != null)
            _customerSpriteRenderer.sprite = _currentSprite;

        if (_dialogueText != null && !string.IsNullOrEmpty(_currentDialogueText))
            _dialogueText.text = _currentDialogueText;
    }
}