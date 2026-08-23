using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TrayDatabase", menuName = "Tray/TrayDatabase")]
public class TrayDatabase : ScriptableObject
{
    [SerializeField] private List<Drink> savedDrinks = new List<Drink>();
    public List<Drink> SavedDrinks => savedDrinks;
    public int MaxSlots = 3;
    public void SaveDrinks(List<Drink> drinks)
    {
        savedDrinks = new List<Drink>(drinks);
    }
    public void ClearDatabase()
    {
        savedDrinks.Clear();
    }
    public bool AddDrink(Drink drink)
    {
        if (savedDrinks.Count < MaxSlots)
        {
            savedDrinks.Add(drink);
            Debug.Log("Drink added to tray");
            return true;
        }
        else
        {
            Debug.Log("Tray is full");
            return false;
        }
        // for (int i = 0; i < MaxSlots; i++)
        // {
        //     if (savedDrinks[i].drinkName == "")
        //     {
        //         savedDrinks[i] = drink;
        //         return true;
        //     }
        // }
        // Debug.Log("Tray is full");
        // return false;
    }
}
