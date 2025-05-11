using Fusion;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private NetworkRunner _runner;
    public GameObject pannel;
    public AudioClip clicksound;

    private void Awake()
    {

        DontDestroyOnLoad(gameObject);
    }
    public void Playgame()
    {
        
        SceneManager.LoadScene("Modeselection");
        SoundManager.instance.PlayerSound(clicksound);
    }

    public void settings()
    {
       
        StartCoroutine(wait());
        SoundManager.instance.PlayerSound(clicksound);
    }
 

    public void OpenSettimgs()
    {

        SceneManager.LoadScene(4);
        SoundManager.instance.PlayerSound(clicksound);
    }

    public void QuitGame()
    {
       
        Application.Quit();
        SoundManager.instance.PlayerSound(clicksound);
    }
    IEnumerator wait()
    {
        pannel.SetActive(true);
        yield return new WaitForSeconds(3);
        pannel.SetActive(false);
    }
}
