using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void SettingsScene()
    {
        SceneManager.LoadScene("SettingsScene");
    }
 public void QuitGame()
    {
        Application.Quit();
    }
    public void CreditsScene()
    {
        SceneManager.LoadScene("CreditsScene");
    }
    public void LoadKitchen()
    {
        if(ReworkedCustomerManager.instance.CurrentCounterState == ReworkedCustomerManager.CounterState.ServingOrder)
        {
            SceneManager.LoadScene("KitchenScene");
        }
    }

    public void LoadCounter()
    {
        SceneManager.LoadScene("CounterScene");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("CounterScene");
    }
}
