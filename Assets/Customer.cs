using UnityEngine;
using TMPro;

public class Customer : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text scoreText;

    private int score = 0;
    public void ReceiveCup(DragCupTest cup)
    {
        string drink = cup.DrinkContents;

        if (drink == "Milo\nWater\nIce")
        {
            score = 75;
            dialogueText.text = "Just what I ordered!";
            Debug.Log("Correct!");
        }
        else if (drink == "Milo\nWater")
        {
            score = 50;
            dialogueText.text = "Forgot the ice but good enough...";
            Debug.Log("Good enough");
        }
        else
        {
            score = 0;
            dialogueText.text = "You got no brain is it?!";
            Debug.Log("Wrong drink!");
        }

        scoreText.text = "SCORE: " + score;

        Destroy(cup.gameObject);
    }
}