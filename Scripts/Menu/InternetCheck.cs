using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InternetCheck : MonoBehaviour
{
    public static event Action<bool> OnnInternetStatusChanged;

    public Image wifiIndicator;
    public Sprite WifiinidacatorOnline;
    public Sprite WifiinidacatorOffline;

    private bool IsOnline = false;

    private void Start()
    {
        StartCoroutine(CheckInternetRoutine());
    }

    IEnumerator CheckInternetRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(CheckInternet());
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator CheckInternet()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"))
        {
            yield return request.SendWebRequest();
            bool newStatus = request.result == UnityWebRequest.Result.Success;
            if(newStatus != IsOnline)
            {
                IsOnline = newStatus;
                wifiIndicator.sprite = IsOnline ? WifiinidacatorOnline : WifiinidacatorOffline;
            }
        }
    }

}
