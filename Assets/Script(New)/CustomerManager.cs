using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    [Header("Databases")]
    [SerializeField] private CustomerDatabase customerDatabase;
    [SerializeField] private RecipeBook recipeBook;

    [Header("UI & Display References")]
    [SerializeField] private SpriteRenderer customerSpriteRenderer;
    [SerializeField] private TMP_Text dialogueText;

    private CustomerData currentCustomer;
    private List<DrinkRecipe> orderedRecipes = new();
    void Start()
    {
        GenerateNewCustomer();
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

        //select random customerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
        int randomCustomerIndex = Random.Range(0, customerDatabase.AllCustomers.Count);
        currentCustomer = customerDatabase.AllCustomers[randomCustomerIndex];

        if (customerSpriteRenderer != null)
            // customerSpriteRenderer.sprite = currentCustomer.CustomerSprite;

        //Choose 1 to 3 drinks randomly
        orderedRecipes.Clear();
        int drinkCount = Random.Range(1, 4);

        for (int i = 0; i < drinkCount; i++)
        {
            int randomRecipeIndex = Random.Range(0, recipeBook.AllRecipes.Count);
            orderedRecipes.Add(recipeBook.AllRecipes[randomRecipeIndex]);
        }

        // 3. Display Order Dialogue
        SetStartDialogue();
    }

    public void SetStartDialogue()
    {
        //if (currentCustomer == null || currentCustomer.StartDialogue.Count == 0) return;

        //string baseText = GetRandomStringFromList(currentCustomer.StartDialogue);

        // Format drink names list
        //string drinkListText = string.Join(", ", orderedRecipes.Select(r => r.drinkName));

        // dialog testing
        //dialogueText.text = $"{baseText}. I would like: {drinkListText}";
    }

    public void ReceiveDrink(Drink servedDrink)
    {
        if (currentCustomer == null || servedDrink == null) return;

        EvaluateAndSetEndDialogue(servedDrink);
    }

    public void EvaluateAndSetEndDialogue(Drink servedDrink)
    {

    }

    private string GetRandomStringFromList(List<string> list)
    {
        if (list == null || list.Count == 0) return "...";
        return list[Random.Range(0, list.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
