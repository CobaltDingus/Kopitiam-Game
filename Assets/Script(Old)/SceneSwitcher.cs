using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
<<<<<<< Updated upstream
    // void Start()
    // {
    //     SceneManager.LoadScene("KitchenScene", LoadSceneMode.Additive);
    // }
=======
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
>>>>>>> Stashed changes
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

