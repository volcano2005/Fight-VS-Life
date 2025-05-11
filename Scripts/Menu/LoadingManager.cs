using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Slider LoadingBar;

    public string sceneToLoad = "MainMenu";

    private void Start()
    {
        StartCoroutine(LoadsceneAsync());
    }

    IEnumerator LoadsceneAsync()
    {
        AsyncOperation Operation = SceneManager.LoadSceneAsync(sceneToLoad);
        Operation.allowSceneActivation = false;

        while(Operation.progress < 0.9f)
        {
            LoadingBar.value = Operation.progress;
            yield return null;
        }
        while(LoadingBar.value < 1f)
        {
            LoadingBar.value += Time.deltaTime;
            yield return null;
        }
        Operation.allowSceneActivation = true;
    }

}
