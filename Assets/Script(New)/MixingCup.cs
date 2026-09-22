using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class MixingCup :
DraggableObject,
DropIngredientInterface,
IPointerDownHandler,
IDragHandler,
IPointerUpHandler
{
    [SerializeField] private GameObject liquidObject;
    private SpriteRenderer liquidSprite;
    public string waterColorHex = "#98DCFF";
    Color currentLiquidColor;

    private Vector3 originalScale;
    private Coroutine bounceCoroutine;

    [SerializeField] private TMP_Text ingredientText;
    [SerializeField] private TMP_Text stirCounterText;
    [SerializeField] private Button stirButton;
    private TMP_Text stirButtonText;
    [SerializeField] private GesturePanel gesturePanel;

    private Image gesturePanelDrawArea;
    [SerializeField] Image gesturePanelDrawAreaOverlay;
    [SerializeField] GameObject _popUpPrefab;

    private void OnEnable()
    {
        DrinkManager.Instance.OnDrinkChanged += RefreshFromDrink;
        DrinkManager.Instance.OnStirProgress += RefreshStirUI;
        DrinkManager.Instance.OnDrinkCleared += HandleCleared;
    }

    private void OnDisable()
    {
        DrinkManager.Instance.OnDrinkChanged -= RefreshFromDrink;
        DrinkManager.Instance.OnStirProgress -= RefreshStirUI;
        DrinkManager.Instance.OnDrinkCleared -= HandleCleared;
    }

    private void Start()
    {
        MakeEmptyCup();
        RefreshFromDrink();
        RefreshStirUI();
    }

    public void SetCamera(Camera newCamera)
    {
        cam = newCamera;
    }

    public void MakeEmptyCup()
    {
        originalScale = transform.localScale;
        liquidSprite = liquidObject.GetComponent<SpriteRenderer>();
        stirButtonText = stirButton.GetComponentInChildren<TMP_Text>();
        gesturePanelDrawArea = gesturePanel.GetComponent<Image>();
        gesturePanelDrawArea.color = Color.white;

        if (DrinkManager.Instance.CurrentDrink.ingredients.Count == 0)
        {
            ingredientText.text = "Mixing Cup Contents: \n\nEMPTY";
        }
    }
    public void ReceiveIngredient(Ingredient ingredient)
    {
        DrinkManager.Instance.AddIngredient(ingredient);
        GameObject popUp = Instantiate(_popUpPrefab);
        popUp.GetComponentInChildren<TMP_Text>().fontSize = 30;
        popUp.GetComponentInChildren<TMP_Text>().text = "+" + ingredient.name;

        dragType = DragEnum.UnfinishedDrink;
        canDrag = true;
    }

    public void AddWater()
    {
        DrinkManager.Instance.AddWater();
        dragType = DragEnum.UnfinishedDrink;
        canDrag = true;
    }

    public void StirDrink()
    {
        DrinkManager.Instance.Stir();
    }

    // Change to drink manager

    private void RefreshFromDrink()
    {
        var drink = DrinkManager.Instance.CurrentDrink;

        RebuildIngredientText(drink);
        AnimateBounce();

        bool hasContents = drink.ingredients.Count > 0;
        canDrag = hasContents;
        dragType = hasContents ? DragEnum.UnfinishedDrink : DragEnum.None;

        if (drink.ingredients.Any(i => i.name == "HotWater"))
        {
            currentLiquidColor = HexToColor(waterColorHex);
            liquidSprite.color = currentLiquidColor;
            liquidObject.SetActive(true);
            gesturePanelDrawArea.color = currentLiquidColor;
        }

        if (DrinkManager.Instance.IsValidDrink)
        {
            stirButtonText.text = "Start stirring";
            stirButton.enabled = true;
            EnableStirring();
        }
    }

    private void RebuildIngredientText(Drink drink)
    {
        ingredientText.text = "Mixing Cup Contents: \n\n";
        foreach (var ing in drink.ingredients)
        {
            ingredientText.text += ing.Name + "\n";
        }
    }

    private void RefreshStirUI()
    {
        int count = DrinkManager.Instance.CurrentStirCount;
        int required = DrinkManager.Instance.stirsRequired;

        stirCounterText.text = "Stirs Left (" + (required - count) + ")";

        currentLiquidColor = Color.Lerp(
            HexToColor(waterColorHex),
            HexToColor(DrinkManager.Instance.CurrentDrink.colorHex),
            count / (float)required
        );

        gesturePanelDrawArea.color = currentLiquidColor;
        liquidSprite.color = currentLiquidColor;

        if (DrinkManager.Instance.IsStirComplete)
        {
            stirButtonText.text = "Ready to serve!";
            stirButton.image.color = Color.green;
            stirButton.interactable = true;
        }
    }

    private void HandleCleared()
    {
        ingredientText.text = "Mixing Cup Contents: \n\nEMPTY";
        canDrag = false;
        dragType = DragEnum.None;
        liquidObject.SetActive(false);
        ResetUI();
    }

    public void ClearCup()
    {
        DrinkManager.Instance.ClearDrink(); // triggers HandleCleared via event
    }

    public void ResetUI()
    {
        stirButton.image.color = Color.grey;
        stirButton.enabled = false;
        stirButtonText.text = "Incomplete Drink";
        stirCounterText.text = "Stirs Left " + DrinkManager.Instance.stirsRequired;

        gesturePanel.DisableDetector();
        gesturePanelDrawArea.color = Color.white;
        currentLiquidColor = Color.white;
        gesturePanelDrawAreaOverlay.gameObject.SetActive(true);
        stirButton.interactable = false;
    }

    public void EnableStirring()
    {
        stirButton.image.color = Color.grey;
        stirButton.interactable = false;
        stirButtonText.text = "Start stirring!";
        gesturePanelDrawAreaOverlay.gameObject.SetActive(false);
        gesturePanel.UnlockAndEnableDetector();
    }

    private IEnumerator Bounce()
    {
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 squashed = new Vector3(originalScale.x * 1.1f, originalScale.y * 0.9f, originalScale.z);

        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, squashed, elapsed / (duration / 2f));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(squashed, originalScale, elapsed / (duration / 2f));
            yield return null;
        }

        transform.localScale = originalScale;
        bounceCoroutine = null;
    }

    private void AnimateBounce()
    {
        if (bounceCoroutine != null) StopCoroutine(bounceCoroutine);
        bounceCoroutine = StartCoroutine(Bounce());
    }

    public override object GetData()
    {
        return DrinkManager.Instance.CurrentDrink.Clone();
    }

    public override void AfterDropFunctions()
    {
        ClearCup();
    }

    public Color HexToColor(string hexCode)
    {
        if (ColorUtility.TryParseHtmlString(hexCode, out Color newColor))
            return newColor;

        Debug.LogWarning("Invalid Hexadecimal string provided!");
        return Color.clear;
    }
}