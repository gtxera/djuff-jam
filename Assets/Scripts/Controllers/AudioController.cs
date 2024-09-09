using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;

    public void Start()
    {
        _sfxSlider.onValueChanged.AddListener((value) =>
        {
            var bus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            bus.setVolume(Mathf.Lerp(-80f, 10f, value));
        });

        _musicSlider.onValueChanged.AddListener((value) =>
        {
            var bus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            bus.setVolume(Mathf.Lerp(-80f, 10f, value));
        });
    }
}
