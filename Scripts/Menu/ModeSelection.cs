using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelection : MonoBehaviour
{
    public AudioClip SelectSound;

    public AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public void SelectEasy()
    {
        SoundManager.instance.PlayerSound(SelectSound);
        PlayerPrefs.SetString("Difficulty", "Easy");
        PlayerPrefs.Save();
        SceneManager.LoadScene("BattleScene");
    }
    
    public void selectNormal()
    {
        SoundManager.instance.PlayerSound(SelectSound);
        PlayerPrefs.SetString("Difficulty", "Normal");
        PlayerPrefs.Save();
        SceneManager.LoadScene("BattleScene");
    }
    
    public void SelectHard()
    {
       
        PlayerPrefs.SetString("Difficulty", "Hard");
        PlayerPrefs.Save();       
        SceneManager.LoadScene("BattleScene");
        SoundManager.instance.PlayerSound(SelectSound);
    }

    public void Backtomainmenu()
    {
        
        SceneManager.LoadScene("MainMenu");
    }
}
