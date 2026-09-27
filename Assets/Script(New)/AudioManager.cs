using UnityEngine;
using UnityEngine.UI;
public enum SFXType
{
    Test = 0,
    Mixing = 1,
    PowderFilling = 2,
    WaterFilling = 3,
    CupPlacing = 4,
    Dispose = 5
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("BGM Audio Sources")]
    [SerializeField] private AudioSource[] bgmSources;

    [Header("SFX Audio Sources")]
    [SerializeField] private AudioSource[] sfxSources;

    [Header("UI Sliders")]
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    private float _bgmVolume = 1.0f;
    private float _sfxVolume = 1.0f;

    public float BGMVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = Mathf.Clamp01(value);
            ApplyBGMVolume();
            PlayerPrefs.SetFloat(BGM_KEY, _bgmVolume);
        }
    }

    public float SFXVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Mathf.Clamp01(value);
            ApplySFXVolume();
            PlayerPrefs.SetFloat(SFX_KEY, _sfxVolume);
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);

        _bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 1.0f);
        _sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1.0f);
    }

    private void Start()
    {
        ApplyBGMVolume();
        ApplySFXVolume();

        if (BGMSlider != null)
        {
            BGMSlider.SetValueWithoutNotify(BGMVolume);
        }

        if (SFXSlider != null)
        {
            SFXSlider.SetValueWithoutNotify(SFXVolume);
        }
    }

    public void PlaySFX(SFXType type, AudioClip clip = null)
    {
        PlaySFX((int)type, clip);
    }

    public void PlaySFX(int index, AudioClip clip = null)
    {
        if (sfxSources == null || index < 0 || index >= sfxSources.Length)
        {
            Debug.LogWarning($"[AudioManager] Invalid SFX index: {index}");
            return;
        }

        PlaySFX(sfxSources[index], clip);
    }

    public void PlaySFX(AudioSource source, AudioClip clip = null)
    {
        if (source == null) return;

        source.volume = SFXVolume;
        if (clip != null)
        {
            source.PlayOneShot(clip);
        }
        else
        {
            source.Play();
        }
    }

    public void StopSFX(SFXType type)
    {
        StopSFX((int)type);
    }

    public void StopSFX(int index)
    {
        if (sfxSources != null && index >= 0 && index < sfxSources.Length)
        {
            if (sfxSources[index] != null)
            {
                sfxSources[index].Stop();
            }
        }
    }


    public void PlayBGM(int index, AudioClip clip = null)
    {
        if (bgmSources == null || index < 0 || index >= bgmSources.Length)
        {
            Debug.LogWarning($"[AudioManager] Invalid BGM index: {index}");
            return;
        }

        PlayBGM(bgmSources[index], clip);
    }

    public void PlayBGM(AudioSource source, AudioClip clip = null)
    {
        if (source == null) return;

        source.volume = BGMVolume;
        if (clip != null)
        {
            source.clip = clip;
        }
        source.Play();
    }


    private void ApplyBGMVolume()
    {
        if (bgmSources == null) return;
        foreach (AudioSource source in bgmSources)
        {
            if (source != null) source.volume = _bgmVolume;
        }
    }

    private void ApplySFXVolume()
    {
        if (sfxSources == null) return;
        foreach (AudioSource source in sfxSources)
        {
            if (source != null) source.volume = _sfxVolume;
        }
    }


    public void OnBGMChanged(float value)
    {
        BGMVolume = value;
    }

    public void OnSFXChanged(float value)
    {
        SFXVolume = value;
    }
}