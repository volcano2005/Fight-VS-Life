using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject PauseMenuUI;
    private bool isPaused = false;
    public AudioClip Clicksound;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.instance.PlayerSound(Clicksound);
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        SoundManager.instance.PlayerSound(Clicksound);
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        isPaused = false;
    }

    public void Pause()
    {
        SoundManager.instance.PlayerSound(Clicksound);
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Replay()
    {
        SoundManager.instance.PlayerSound(Clicksound);
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BacktoMeanu()
    {
        SoundManager.instance.PlayerSound(Clicksound);
        Time.timeScale = 1f;
        SceneManager.LoadScene("ModeSelection");
    }
}
