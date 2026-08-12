using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TrayDatabase", menuName = "Tray/TrayDatabase")]
public class TrayDatabase : ScriptableObject
{
    [SerializeField] private List<Drink> savedDrinks = new List<Drink>();
    public List<Drink> SavedDrinks => savedDrinks;
    public void SaveDrinks(List<Drink> drinks)
    {
        savedDrinks = new List<Drink>(drinks);
    }
    public void ClearDatabase()
    {
        savedDrinks.Clear();
    }
}
