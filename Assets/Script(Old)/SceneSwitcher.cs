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
        SceneManager.LoadScene("KitchenScene");
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

