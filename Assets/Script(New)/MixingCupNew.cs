using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MixingCupNew : MonoBehaviour, DropIngredientInterface
{
    //put MasterRecipe.asset here
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private Image targetDisplayImage;
    [SerializeField] private GameObject imageContainer;

    [SerializeField] private int maxIngredients = 5;

    [SerializeField] private TMP_Text cupText;

    private List<Ingredient> currentIngredients = new List<Ingredient>();

    public bool AddIngredient(Ingredient newIngredient)
    {
        if(currentIngredients.Count >= maxIngredients)
        {
            Debug.Log("Cup is Full!");
            return false;
        }
        currentIngredients.Add(newIngredient);
        Debug.Log($"Added {newIngredient.Name}. Total: {currentIngredients.Count}");
        return true;
    }

    public void ReceiveIngredient(Ingredient newIngredient)
    {
        currentIngredients.Add(newIngredient);

        // cupText.text = string.Join("\n", ingredients);
    }

    public void FinaliseDrink()
    {
        if(currentIngredients.Count < 1)
        {

        }
        Sprite matchedSprite = null;
        string matchedName = "idk";
        foreach(var recipe in recipeBook.AllRecipes)
        {
            if (compareIngredients(currentIngredients, recipe.ingredients)){
                matchedName = recipe.drinkName;
                // matchedSprite = recipe.drinkImage;
                break;
            }
        }

        if(matchedSprite != null)
        {
            targetDisplayImage.sprite = matchedSprite;
            if(imageContainer != null)
            {
                imageContainer.SetActive(true);
            }
            Debug.Log($"Completed: {matchedName}");

            //add somethign here if planning to save different in global pool
        }
        else
        {
            targetDisplayImage.sprite = null;
            if(imageContainer != null)
            {
                imageContainer.SetActive(false);
            }
            Debug.Log("drink does not exist");
        }
    }

    private bool compareIngredients(List<Ingredient> listA, List<Ingredient> listB)
    {
        //compare ingredient number
        if (listA.Count != listB.Count)
        {
            return false;
        }
        List<string> idsA = new List<string>();
        List<string> idsB = new List<string>();

        foreach(var ingredient in listA)
        {
            idsA.Add(ingredient.Id);
        }
        foreach(var ingredient in listB)
        {
            idsB.Add(ingredient.Id);
        }

        //organise the shit
        idsA.Sort();
        idsB.Sort();

        //
        for (int i = 0; i < idsA.Count; i++)
        {
            if (idsA[i] != idsB[i])
            {
                return false;
            }
        }
        return true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
