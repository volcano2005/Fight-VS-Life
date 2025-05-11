using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitingRoomUIManager : MonoBehaviour
{
    public TMP_Text roomcodeText;
    public TMP_Text statusText;
    public FusionPrivateRoomManager roomManager;
    public GameObject Uicanvas;

    private void Start()
    {
        if (roomManager == null)
        {
            roomManager = FindObjectOfType<FusionPrivateRoomManager>();
        }

        if (roomManager != null && !string.IsNullOrEmpty(roomManager.CurrentSessionName))
        {
            roomcodeText.text = "Room Code: " + roomManager.CurrentSessionName;
            Debug.Log("Room code set from roomManager: " + roomManager.CurrentSessionName);
        }
        else if (!string.IsNullOrEmpty(FusionPrivateRoomManager.TempSessionName))
        {
            roomcodeText.text = "Room Code: " + FusionPrivateRoomManager.TempSessionName;
            Debug.Log("Room code set from TempSessionName: " + FusionPrivateRoomManager.TempSessionName);
        }
        else
        {
            roomcodeText.text = "Room Code: Not available";
            Debug.LogWarning("roomManager and TempSessionName are null or not set.");
        }

        statusText.text = "Waiting for second player...";
    }


    public void OnCreateRoomClicked()
    {
        if (roomManager != null)
        {
            roomManager.CreateGame();
            Debug.Log("Create room button clicked, session name: " + roomManager.CurrentSessionName);
        }
        else
        {
            Debug.LogError("roomManager is not assigned!");
        }
    }

    public void OncancelClicked()
    {
        if (roomManager == null)
        {
            roomManager = FindObjectOfType<FusionPrivateRoomManager>();
        }

        if (roomManager != null && roomManager.runner != null)
        {
            roomManager.runner.Shutdown();
        }

        FusionPrivateRoomManager._tempSessionName = null;
        SceneManager.LoadScene("LobbyScene");
    }

    public void UIoffer()
    {
        Uicanvas.SetActive(false);
        ////if(waitingRoomUI != null)
        ////{
        //waitingRoomUI.SetActive(false);
        ////    Destroy(waitingRoomUI);
        ////}
        Debug.Log("scene is shifted");
    }

}