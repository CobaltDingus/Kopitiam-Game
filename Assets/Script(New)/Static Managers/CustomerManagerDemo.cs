using UnityEngine;
using System.Collections.Generic;

public class CustomerManagerDemo : MonoBehaviour
{
    public static CustomerManagerDemo Instance { get; private set; }

    [SerializeField] private List<DrinkRecipe> availableDrinks;

    private List<DrinkRecipe> drinkOrders = new List<DrinkRecipe>();

    public enum CurrentStage
    {
        IntroKopi,
        IntroKopiO,
        Customers,
        DayEnd
    }

    private CurrentStage currentStage;
    public CurrentStage Stage => currentStage;

    public DrinkRecipe CurrentOrder
    {
        get
        {
            if (drinkOrders.Count == 0)
                return null;

            return drinkOrders[0];
        }
    }

    private void Awake()
    {
        currentStage = CurrentStage.IntroKopi;        


        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Generate only if we don't already have an order
        // if (drinkOrders.Count == 0)
        // {
        //     GenerateNewCustomer();
        // }
    }

    public void OrderKopi()
    {
        drinkOrders.Clear();
        currentStage = CurrentStage.IntroKopi;
        drinkOrders.Add(availableDrinks[0]);
    }

    public void OrderKopiO()
    {
        drinkOrders.Clear();
        currentStage = CurrentStage.IntroKopiO;
        drinkOrders.Add(availableDrinks[1]);
    }

    public void GenerateNewCustomer()
    {
        drinkOrders.Clear();
        int randomIndex = Random.Range(0, availableDrinks.Count);
        drinkOrders.Add(availableDrinks[randomIndex]);

        Debug.Log("New order: " + CurrentOrder.drinkName);
    }
}