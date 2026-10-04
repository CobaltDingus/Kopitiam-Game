using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : UIPanel
{
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBGMChanged(float value)
    {
        AudioManager.instance.BGMVolume = value;
    }

    public void OnSFXChanged(float value)
    {
        AudioManager.instance.SFXVolume = value;
    }

    public void TogglePauseWindow()
    {
        TogglePanel();
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

}
