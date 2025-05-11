using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    public Slider volumeslider;
    private void Start()
    {
        volumeslider.value = AudioListener.volume;

        volumeslider.onValueChanged.AddListener(SetVolume);
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;

        if(SoundManager.instance != null)
        {
            SoundManager.instance.GetComponent<AudioSource>().volume = value;
        }
    }

    public void BacktoMainmenu()
    {
        SceneManager.LoadScene(1);
    }
}
