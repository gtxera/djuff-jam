using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("Mixers Groups")]
    [SerializeField] AudioMixer mixer;

    // Área para acesso dos sliders
    [Header("Sliders Groups")]
    [SerializeField] Slider musicVolSlider;
    [SerializeField] Slider VFXVolSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            LoadMusicPrefs();
        }
        else
        {
            SetMusicVolume();
            SetVFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicVolSlider.value;
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        // Salva última configuração de som definida pelo jogador
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    // Método responsável pela mudança de volume do som da música
    public void SetVFXVolume()
    {
        float volume = VFXVolSlider.value;
        mixer.SetFloat("VFXVolume", Mathf.Log10(volume) * 20);

        // Salva última configuração de som definida pelo jogador
        PlayerPrefs.SetFloat("VFXVolume", volume);
    }

    // Método para carregar última configuração de som definida pelo jogador
    public void LoadMusicPrefs()
    {
        musicVolSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        VFXVolSlider.value = PlayerPrefs.GetFloat("VFXVolume");

        SetMusicVolume();
        SetVFXVolume();
    }
}
