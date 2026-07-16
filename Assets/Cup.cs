using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [SerializeField] private TMP_Text cupText;

    public List<string> ingredients = new();

    public void AddIngredient(string ingredient)
    {
        ingredients.Add(ingredient);

        cupText.text = string.Join("\n", ingredients);
    }

    public void ResetCup()
    {
        ingredients.Clear();
        cupText.text = "EMPTY";
    }
}