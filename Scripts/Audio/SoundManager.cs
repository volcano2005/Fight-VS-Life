using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
   public static SoundManager instance {  get; private set; }
   public AudioSource source;
 

    private void Awake()
    {
       
       source = GetComponent<AudioSource>();
        
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void PlayerSound(AudioClip _sound)
    {
        source.PlayOneShot(_sound);
    }
   
}
