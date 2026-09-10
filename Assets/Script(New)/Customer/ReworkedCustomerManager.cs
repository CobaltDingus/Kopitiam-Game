using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReworkedCustomerManager : MonoBehaviour
{
    // ================================ REVAMP CODES ================================
    // [RULES]
    // [NOTE] Do Not Change Any Codes Here Unless Needed
    // - core variables
    // - functions for calculation and modifying said variables
    // - function starts with Capital
    // - variable starts with small
    // ==================================== START ====================================
    public static ReworkedCustomerManager instance { get; private set; }
    [Header("Revamp Scripts and Database")]
    // ================================== GENERAL VARIABLES ==================================
    [Header("Database (Revamp)")]
    [SerializeField] private CustomerDatabase _customerDatabase;
    [SerializeField] private RecipeBook _recipeBook;
    [SerializeField] private TrayDatabase _trayDatabase;

    [Header("UI Display Referencing (Revamp)")]
    [SerializeField] private SpriteRenderer _customerSpriteRenderer;
    [SerializeField] private TMP_Text _dialogueText;

    [Header("Buttons")]
    private Button _nextButton;
    private Button _okayButton;
    private Button _retryButton;

    // testing button
    private Button _perfectButton;

    [Header("General Variables (Revamp)")]

    private int _minDrink = 1;
    private int _maxDrink = 1;
    private int _chosenCustomerSlot;
    private int _chosenSpriteIndex;
    private string _currentDialogueText = "";
    private string _lastNonMatchedIngredientText = "";

    // handles variables
    public enum GameplayState
    {
        Tutorial,
        GeneralGameplay,
        DaySummary
    }

    // handles setActive
    public enum CounterState
    {
        // Tutorial //
        ReadDialogue,
        TakingOrder,
        ServingOrder,
        RedoOrder,

    }

    [SerializeField] public CounterState CurrentCounterState;
    [SerializeField] public GameplayState CurrentGameplayState;

    Scene counterScene;

    private CustomerData _currentCustomer;
    private Sprite _currentSprite;
    private List<DrinkRecipe> orderedRecipes = new();

    // ================================== TUTORIAL VARIABLE ==================================
    [Header("Tutorial Variables (Revamp)")]
    private Sprite _tutorialSprite;
    private List<string> _tutorialDialogue;
    private string _tutorialWrongOrderDialogue = "Hmm, that's not right. Try again.";
    // ================================ GETTER & SETTER ================================
    // TBC

    // ================================ AWAKE START UPDATE ================================
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

        bool Skip = false;
        if (Skip)
        {
            // skip tutorial
            CurrentGameplayState = GameplayState.GeneralGameplay;
            CurrentCounterState = CounterState.TakingOrder;
        }
        else
        {
            // set up Tutorial Start
            CurrentGameplayState = GameplayState.Tutorial;
            CurrentCounterState = CounterState.ReadDialogue;
        }

        EvaluateAndUpdateGameplayState();
        EvaluateAndUpdateCounterState();

    }

    void Update()
    {

    }

    // =============================================================================================
    // ================================ SCENE LOADING & SAVE BUTTON ================================
    // =============================================================================================
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // change name if needed
        if(scene.name == "CounterScene")
        {
            FindNextButton();
            FindOkayButton();
            FindRetryButton();
            FindPerfectButton();
            return;
        }
    }

    void FindNextButton()
    {
        _nextButton = GameObject.Find("Next").GetComponent<Button>();
        _nextButton.onClick.AddListener(GenerateCustomer);
    }

    void FindOkayButton()
    {
        _okayButton = GameObject.Find("Okay").GetComponent<Button>();
        _okayButton.onClick.AddListener(NextDialogue);
    }

    void FindPerfectButton()
    {
        _perfectButton = GameObject.Find("Perfect").GetComponent<Button>();
    }

    void FindRetryButton()
    {
        _retryButton = GameObject.Find("TryAgain").GetComponent<Button>();
        _retryButton.onClick.RemoveAllListeners();
        _retryButton.onClick.AddListener(PreviousDialogue);
    }
    // ====================================================================================
    // ================================ TUTORIAL FUNCTIONS ================================
    // ====================================================================================
    public void GenerateTutorialOrder()
    {
        orderedRecipes.Clear();

        if (ReworkedSaveManager.instance.TutorialPhase == 5)
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
            
            CurrentCounterState = CounterState.ServingOrder;
            EvaluateAndUpdateCounterState();

            return;
        }
        ReworkedSaveManager.instance.TutorialPhase += 1;
        if (ReworkedSaveManager.instance.TutorialPhase == 5 || ReworkedSaveManager.instance.TutorialPhase == 7)
        {
            GenerateTutorialOrder();
        }
        if(ReworkedSaveManager.instance.TutorialPhase >= _tutorialDialogue.Count)
        {
            ReworkedSaveManager.instance.Day = 1;
            // set and update
            CurrentGameplayState = GameplayState.GeneralGameplay;
            EvaluateAndUpdateGameplayState();
        }
        LoadTutorialDialogue();
    }

    public void LoadTutorialDialogue()
    {

        if (ReworkedSaveManager.instance.TutorialPhase == 5 || ReworkedSaveManager.instance.TutorialPhase == 7)
        {
            
        }
        if (_tutorialDialogue != null && ReworkedSaveManager.instance.TutorialPhase < _tutorialDialogue.Count)
        {
            _currentDialogueText = _tutorialDialogue[ReworkedSaveManager.instance.TutorialPhase];
        }

        if (_dialogueText != null)
        {
            _dialogueText.text = _currentDialogueText;
        }
    }

    public void PreviousDialogue()
    {
        ReworkedSaveManager.instance.TutorialPhase -= 1;
        LoadTutorialDialogue();
        return;
    }

    public void SetTutorialWrong()
    {
        _currentDialogueText = _tutorialWrongOrderDialogue;
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
        NextDialogue();
    }


    // ================================ GENERAL FUNCTIONS ================================
    // handles setActive
    public void EvaluateAndUpdateCounterState()
    {
        switch (CurrentCounterState)
        {
            case CounterState.ReadDialogue:
                // set up Button Displays
                _okayButton.gameObject.SetActive(true);
                _nextButton.gameObject.SetActive(false);
                _perfectButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(false);

                break;

            case CounterState.TakingOrder:
                // set up Button Displays
                _okayButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);
                _perfectButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(false);
                if (CurrentGameplayState == GameplayState.Tutorial)
                {
                    _okayButton.gameObject.SetActive(true);
                    // update button function
                    _okayButton.onClick.RemoveAllListeners();
                    _okayButton.onClick.AddListener(ToKitchen);
                }
                else if (CurrentGameplayState == GameplayState.GeneralGameplay)
                {
                    _nextButton.gameObject.SetActive(true);
                    // update button function
                    _nextButton.onClick.RemoveAllListeners();
                    _nextButton.onClick.AddListener(ToKitchen);
                }

                break;

            case CounterState.ServingOrder:
                // gameObject set.Active here
                // set up Button Displays
                _okayButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);
                _perfectButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(false);

                break;

            case CounterState.RedoOrder:
                // set up Button Displays
                _okayButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);
                _perfectButton.gameObject.SetActive(false);
                _retryButton.gameObject.SetActive(true);
                break;

        }
    }

    // handles variables
    public void EvaluateAndUpdateGameplayState()
    {
        switch (CurrentGameplayState)
        {
            case GameplayState.Tutorial:
                // set up Tutorial UI Display
                ReworkedUIManager.instance.CustomerCountText.gameObject.SetActive(false);
                ReworkedUIManager.instance.TimerText.gameObject.SetActive(false);
                ReworkedUIManager.instance.DayText.gameObject.SetActive(true);
                ReworkedUIManager.instance.FavourText.gameObject.SetActive(true);

                if (ReworkedSaveManager.instance.Day == 0)
                {

                }
                break;

            case GameplayState.GeneralGameplay:
                // set up GeneralGameplay UI Display
                ReworkedUIManager.instance.CustomerCountText.gameObject.SetActive(true);
                ReworkedUIManager.instance.TimerText.gameObject.SetActive(true);
                ReworkedUIManager.instance.DayText.gameObject.SetActive(true);
                ReworkedUIManager.instance.FavourText.gameObject.SetActive(true);
                
                break;

            case GameplayState.DaySummary:
                // set up Daysummary UI Display
                ReworkedUIManager.instance.CustomerCountText.gameObject.SetActive(false);
                ReworkedUIManager.instance.TimerText.gameObject.SetActive(false);
                ReworkedUIManager.instance.DayText.gameObject.SetActive(false);
                ReworkedUIManager.instance.FavourText.gameObject.SetActive(false);

                break;
        }
    }

    public void GenerateCustomer()
    {
        //int RandomCustomerIndex;
        int DayRemain = ReworkedSaveManager.instance.CurrentCustomer - ReworkedSaveManager.instance.MaxCustomer;
        // checks to see if there is any available "event"
        if (ReworkedSaveManager.instance.CurrentEvent != 0)
        {
            // gacha machine
            int TempNum = RandomIndex(ReworkedSaveManager.instance.MaxCustomer);
            if(TempNum == 1)
            {
                // load special customer then deduct by 1 and break
                // [total event type -1 ] (0 = tourist, 1 = SleepDeprived)
                int EventNum = RandomIndex(1);
                GenerateSpecialCustomer(EventNum);
            }
            else if (DayRemain == ReworkedSaveManager.instance.CurrentEvent)
            {
                int EventNum = RandomIndex(1);
                GenerateSpecialCustomer(EventNum);
            }
            else
            {
                // load basic customer
                GenerateNormalCustomer();
            }
        }
        else
        {
            GenerateNormalCustomer();
        }
        
    }

    public void GenerateSpecialCustomer(int eventNum)
    {
        int TotalCustomer;
        if (eventNum == 0)
        {
            // load Special customer Tourist
            TotalCustomer = _customerDatabase.AllTouristCustomer.Count;
            int ChosenIndex = RandomIndex(TotalCustomer);

            // can add a condition to keep on reroll here for chosenIndex
            // if want unique customer consecutively
            _currentCustomer = _customerDatabase.AllTouristCustomer[ChosenIndex];
            SetCustomerSprite();


        }
        else if (eventNum == 1)
        {
            // load Special customer SleepDeprived
            TotalCustomer = _customerDatabase.AllSleepDeprivedCustomer.Count;
        }
        // add more "else if" if there is more variation
        
    }

    public void GenerateNormalCustomer()
    {
        // ltr change AllCustomers to NormalCustomer
        int TotalCustomer = _customerDatabase.AllCustomers.Count;
    }

    public int RandomIndex(int maxNum)
    {
        int index = UnityEngine.Random.Range(0, maxNum);
        return index;
    }

    public void SetCustomerSprite()
    {
        int TotalSprites = _currentCustomer.CustomerSprite.Count;


    }

    public void SetStartDialogue()
    {

    }

    public void ProcessOrder()
    {
        if(CurrentGameplayState == GameplayState.Tutorial)
        {
            EvaluateTutorialOrder(_trayDatabase.SavedDrinks);
            _trayDatabase.ClearDatabase();
            // set and update
            CurrentCounterState = CounterState.ReadDialogue;
            EvaluateAndUpdateCounterState();
            return;
        }
        else
        {
            EvaluateGeneralOrder(_trayDatabase.SavedDrinks);
            _trayDatabase.ClearDatabase();
            // set and update
            CurrentCounterState = CounterState.TakingOrder;
            EvaluateAndUpdateCounterState();
            return;
        }
    }
    
    public void EvaluateGeneralOrder(List<Drink> ServedDrink)
    {
        int PerfectCount = 0;
        bool BaseIngredientMatched = false;
        List<Drink> RemainingServed = new List<Drink>(ServedDrink);

        foreach(DrinkRecipe recipe in orderedRecipes)
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
                // Find base ingredients required by the recipe (checks both recipe.ingredients and recipe.baseIngredient)
                List<Ingredient> baseIngredientsInRecipe = recipe.ingredients
                    .Where(i => i != null && i.IsBaseIngredient)
                    .ToList();

                if (recipe.baseIngredient != null && recipe.baseIngredient.Count > 0)
                {
                    baseIngredientsInRecipe.AddRange(recipe.baseIngredient.Where(i => i != null));
                }

                // Check if any served drink contains at least one of the required base ingredients
                foreach (Drink served in ServedDrink)
                {
                    if (served == null || served.ingredients == null) continue;

                    bool hasMatchingBase = served.ingredients.Any(servedIng =>
                        servedIng != null && baseIngredientsInRecipe.Any(reqBase =>
                            string.Equals(servedIng.Id, reqBase.Id, StringComparison.OrdinalIgnoreCase)
                        )
                    );

                    if (hasMatchingBase)
                    {
                        BaseIngredientMatched = true;
                        break;
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
        }
        else if (BaseIngredientMatched)
        {
            frontList = _currentCustomer.DecentFrontDialogue;
            backList = _currentCustomer.DecentBackDialogue;
            // ltr change this to var
            ReworkedSaveManager.instance.Favour += 50;
        }
        else
        {
            frontList = _currentCustomer.WrongFrontDialogue;
            backList = _currentCustomer.WrongBackDialogue;
        }

        for (int i = 0; i < PerfectCount; i++)
        {
            ReworkedSaveManager.instance.Favour += 50;
        }
        ReworkedUIManager.instance.UpdateFavourDisplayText();

        //int index = GetRandomDialogueIndex(frontList, backList);
        int MaxIndex = Mathf.Min(frontList.Count, backList.Count);
        int index = RandomIndex(MaxIndex);
        if (index == -1)
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

        // Step 1: Compare and match Base Ingredient first
        Ingredient requiredBase = remainingRequired.FirstOrDefault(i => i != null && i.IsBaseIngredient);

        if (requiredBase != null)
        {
            Ingredient servedBaseMatch = remainingServed.FirstOrDefault(s => s != null && s.Id == requiredBase.Id);

            if (servedBaseMatch != null)
            {
                Debug.Log($"[Base Match] Matched base ingredient: {requiredBase.Name}");
                remainingRequired.Remove(requiredBase);
                remainingServed.Remove(servedBaseMatch);
            }
            else
            {
                Debug.LogWarning($"[Base Mismatch] Base ingredient mismatch or missing: {requiredBase.Name}");
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
            _lastNonMatchedIngredientText = string.Join(", ", nonMatchedIngredients
                .Where(i => i != null)
                .Select(i => i.name));

            Debug.LogWarning($"[Mismatches Detected] Non-matched ingredients: \"{_lastNonMatchedIngredientText}\"");
        }
        else
        {
            _lastNonMatchedIngredientText = "";
        }

        return nonMatchedIngredients.Count == 0;
    }

    public void RegisterDisplayReferences(SpriteRenderer spriteRenderer, TMP_Text text)
    {
        _customerSpriteRenderer = spriteRenderer;
        _dialogueText = text;

        if (_customerSpriteRenderer != null)
            _customerSpriteRenderer.sprite = _currentSprite;

        if (_dialogueText != null)
            _dialogueText.text = _currentDialogueText;
    }
}
