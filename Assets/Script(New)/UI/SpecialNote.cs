using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialNote : 
UIPanel
{
    [SerializeField] private TextMeshProUGUI noteText;
    [SerializeField] private RecipeViewer recipeViewer;
    [TextArea(3, 5)]
    [SerializeField] private string dayTwoText;
    [TextArea(3, 5)]
    [SerializeField] private string dayThreeText;
    [TextArea(3, 5)]
    [SerializeField] private string dayFourText;
    [TextArea(3, 5)]
    [SerializeField] private string dayFiveText;
    [TextArea(3, 5)]
    [SerializeField] private string daySixText;
    [TextArea(3, 5)]
    [SerializeField] private string daySevenText;
    [TextArea(3, 5)]
    [SerializeField] private string dayEightText;
    [SerializeField] private List<DrinkRecipe> tehUiloRecipes;
    [SerializeField] private DrinkRecipe chamRecipe;
    [SerializeField] private DrinkRecipe horKaSaiRecipe;
    [SerializeField] private DrinkRecipe saiKaHorRecipe;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateContent(int day)
    {
        switch (day)
        {
            case 2:
                noteText.text = dayTwoText;
                recipeViewer.BuildListNoteMultiple(tehUiloRecipes);
                break;
            case 3:
                noteText.text = dayThreeText;
                break;
            case 4:
                noteText.text = dayFourText;
                break;
            case 5:
                noteText.text = dayFiveText;
                break;
            case 6:
                noteText.text = daySixText;
                recipeViewer.BuildListNoteSingle(chamRecipe);
                break;
            case 7:
                noteText.text = daySevenText;
                recipeViewer.BuildListNoteSingle(horKaSaiRecipe);
                break;
            case 8:
                noteText.text = dayEightText;
                recipeViewer.BuildListNoteSingle(saiKaHorRecipe);
                break;
            default:
                break;
        }
    }
}
