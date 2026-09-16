using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    //public void PlayGame()
    //{
    //    SceneManager.LoadScene("PlayGame");
    //}
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
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}

public class PauseWindow : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

}
