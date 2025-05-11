using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    public FusionPrivateRoomManager roomManager;

    [Header("UI Elements")]
    public GameObject createPanel;
    public GameObject joinPanel;
  
    public TMP_Text roomCodeText;
    public TMP_InputField joinCodeInput;
    public TMP_Text statusText;

    private void Start()
    {
        roomCodeText.gameObject.SetActive(false);
        statusText.text = "";
    }

    public void OnCreateRoomClicked()
    {
        if (roomManager == null)
        {
            statusText.text = "Room manager not assigned!";
            return;
        }

        roomManager.CreateGame();
        statusText.text = "Waiting for game to start...";
        // Do not set roomCodeText yet; wait for callback
    }

    public void OnJoinRoomClicked()
    {
        string code = joinCodeInput.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(code))
        {
            statusText.text = "Please enter a valid room code.";
            return;
        }

        if (roomManager == null)
        {
            statusText.text = "Room manager not assigned!";
            return;
        }

        roomManager.JoinGame(code);
        statusText.text = "Joining room...";
    }

    // Method to update UI when session name is available
    public void UpdateRoomCode(string sessionName)
    {
        if (roomCodeText == null) return;

        roomCodeText.text = "Your Room Code: " + sessionName;
        roomCodeText.gameObject.SetActive(true);
        statusText.text = "Waiting for another player...";
    }

   

}