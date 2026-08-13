using UnityEngine;

using TMPro;
using System.Collections.Generic;

public class Customer : MonoBehaviour, DropTrayInterface
{
    [SerializeField] private TMP_Text customerCounter;
    [SerializeField] private TMP_Text dialogueText;

    private SpriteRenderer customerSprite;

    private void Start()
    {
        customerSprite = GetComponentInChildren<SpriteRenderer>();

        RunCurrentCustomer();
    }

    private void RunCurrentCustomer()
    {
        switch (CustomerManagerDemo.Instance.Stage)
        {
            case CustomerManagerDemo.CurrentStage.IntroKopi:
                bossIntro();
                break;

            case CustomerManagerDemo.CurrentStage.IntroKopiO:
                bossIntro2();
                break;

            case CustomerManagerDemo.CurrentStage.Customers:
                // CustomerRoutine();
                break;

            case CustomerManagerDemo.CurrentStage.DayEnd:
                // DayEndRoutine();
                break;
        }
    }

    private void bossIntro()
    {
        dialogueText.text = "You know how to make Kopi? Just add Kopi, Sugar, and Condensed Milk. Then put water.";
        CustomerManagerDemo.Instance.OrderKopi();
    }

    private void bossIntro2()
    {
        dialogueText.text = "Now you learn to make Kopi O. 'O' means no Condensed Milk. Can do, right?";
        CustomerManagerDemo.Instance.OrderKopiO();
        // Debug.Log(CustomerManagerDemo.Instance.CurrentOrder.drinkName);
    }


    public void proceed()
    {
        switch (CustomerManagerDemo.Instance.Stage)
        {
            case CustomerManagerDemo.CurrentStage.IntroKopi:
                bossIntro2();
                break;

            case CustomerManagerDemo.CurrentStage.IntroKopiO:
                CustomerManagerDemo.Instance.GenerateNewCustomer();
                UpdateCustomerUI();
                break;

            case CustomerManagerDemo.CurrentStage.Customers:
                // Customer stage logic
                break;

            case CustomerManagerDemo.CurrentStage.DayEnd:
                dialogueText.text = "Well done.";
                break;
        }
    }
    private void UpdateCustomerUI()
    {
        DrinkRecipe order = CustomerManagerDemo.Instance.CurrentOrder;

        if (order == null)
        {
            Debug.LogWarning("Customer has no order!");
            return;
        }

        dialogueText.text = "Could I have a " + order.drinkName + "?";

        Debug.Log("Customer UI updated: " + order.drinkName);
    }

    public void ReceiveTray(List<Drink> drinks)
    {
            // foreach (DrinkRecipe recipe in CustomerManagerDemo.)
            // {
            //     if (drink.ingredients.Count == recipe.ingredients.Count &&
            //     drink.ingredients
            //     .OrderBy(i => i.Id)
            //     .SequenceEqual(recipe.ingredients.OrderBy(i => i.Id)))
            //     {
            //         drink.drinkSprite = recipe.drinkImage;
            //         drink.drinkName = recipe.drinkName;
            //         break;
            //     }
                
            // }
        if (drinks[0].drinkName == CustomerManagerDemo.Instance.CurrentOrder.drinkName)
        {
            switch (CustomerManagerDemo.Instance.Stage)
            {
                case CustomerManagerDemo.CurrentStage.IntroKopi:
                    dialogueText.text = "Good, you know how to make Kopi.";
                    break;
                case CustomerManagerDemo.CurrentStage.IntroKopiO:
                    dialogueText.text = "Good, now you know what 'O' means. You serve customers now.";
                    break;
                case CustomerManagerDemo.CurrentStage.Customers:
                    dialogueText.text = "Thank you!";
                    // customerSprite = 
                    break;
                default:
                    dialogueText.text = "Well done.";
                    break;
            }
        }
        else
        {
            switch (CustomerManagerDemo.Instance.Stage)
            {
                case CustomerManagerDemo.CurrentStage.IntroKopi:
                    dialogueText.text = "Wrong, do again.";
                    break;
                case CustomerManagerDemo.CurrentStage.IntroKopiO:
                    dialogueText.text = "Wrong, do again.";
                    break;
                case CustomerManagerDemo.CurrentStage.Customers:
                    dialogueText.text = "I didn't order this...";
                    break;
                default:
                    dialogueText.text = "Wrong.";
                    break;
            }
        }
    }
}