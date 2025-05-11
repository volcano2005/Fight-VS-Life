using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using TMPro;

public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Transform roomListContainer;
    [SerializeField] private GameObject roomListItemPrefab;
    [SerializeField] private Image image;
    [SerializeField] private NetworkObject PlayerPrefab;

    private NetworkRunner lobbyRunner;
    private NetworkRunner hostRunner;
    private NetworkRunner clientRunner;
    private bool isShuttingDown = false;
    private List<SessionInfo> availableSessions = new List<SessionInfo>();
    private string currentRoomName;

    private void Awake()
    {
        if (roomNameInput == null || createRoomButton == null || joinRoomButton == null || roomListContainer == null || roomListItemPrefab == null)
        {
            Debug.LogError("One or more UI elements are not assigned in the Inspector!");
        }

        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
    }

    private async void Start()
    {
        await JoinLobbyAsync();
      //  Destroy(gameObject, 3f);
    }

    private async Task<NetworkRunner> CreateNewRunnerAsync(string runnerName, NetworkRunner existingRunner = null)
    {
        if (existingRunner != null)
        {
            if (existingRunner.IsRunning && !isShuttingDown)
            {
                isShuttingDown = true;
                Debug.Log($"Shutting down {runnerName} NetworkRunner...");
                await existingRunner.Shutdown();
                isShuttingDown = false;
            }
            Destroy(existingRunner.gameObject);
        }

        GameObject runnerObj = new GameObject(runnerName);
        NetworkRunner runner = runnerObj.AddComponent<NetworkRunner>();
        runner.AddCallbacks(this);
        runner.ProvideInput = true;
        Debug.Log($"Created new {runnerName} NetworkRunner.");
        return runner;
    }

    private async Task JoinLobbyAsync()
    {
        Debug.Log("Joining session lobby...");

        lobbyRunner = await CreateNewRunnerAsync("LobbyRunner", lobbyRunner);

        StartGameResult result = await lobbyRunner.JoinSessionLobby(SessionLobby.Shared);
        if (result.Ok)
        {
            Debug.Log("Successfully joined session lobby.");
           
        }
        else
        {
            Debug.LogError($"Failed to join session lobby: {result.ShutdownReason}");
        }
    }

    public async void CreateRoom()
    {
        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("Room name cannot be empty!");
            return;
        }

        Debug.Log($"Creating room: {roomName}");
        currentRoomName = roomName;

        // Create a new NetworkRunner for hosting the room
        hostRunner = await CreateNewRunnerAsync("HostRunner", hostRunner);

        NetworkSceneManagerDefault sceneManager = gameObject.GetComponent<NetworkSceneManagerDefault>();
        if (sceneManager == null)
        {
            Debug.Log("Adding NetworkSceneManagerDefault...");
            sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        try
        {
            StartGameResult result = await hostRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = roomName,
                Scene = SceneRef.FromIndex(3),
                SceneManager = sceneManager
            });

            if (result.Ok)
            {
                Debug.Log($"Successfully created room: {roomName}");
                joinRoomButton.gameObject.SetActive(false);
                roomNameInput.gameObject.SetActive(false);
                createRoomButton.gameObject.SetActive(false);
                image.gameObject.SetActive(false);
                
                // Do NOT shut down the hostRunner; keep it alive to maintain the room
                await JoinLobbyAsync(); // Refresh the lobby to update the session list
            }
            else
            {
                Debug.LogError($"Failed to create room: {result.ShutdownReason}");
                hostRunner = null;
                currentRoomName = null;
                await JoinLobbyAsync();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception during room creation: {e.Message}");
            hostRunner = null;
            currentRoomName = null;
            await JoinLobbyAsync();
        }
    }

    public async void JoinRoom()
    {
        string roomName = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogWarning("Room name cannot be empty!");
            return;
        }

        Debug.Log($"Attempting to join room: {roomName}");

        bool roomExists = availableSessions.Exists(session => session.Name == roomName);
        if (!roomExists)
        {
            Debug.LogWarning($"Room '{roomName}' not found in the lobby. Refreshing session list...");
            await JoinLobbyAsync();
            roomExists = availableSessions.Exists(session => session.Name == roomName);
            if (!roomExists)
            {
                Debug.LogError($"Room '{roomName}' does not exist. Please create the room first.");
                return;
            }
        }

        clientRunner = await CreateNewRunnerAsync("ClientRunner", clientRunner);

        try
        {
            StartGameResult result = await clientRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = roomName
            });

            if (result.Ok)
            {
                Debug.Log($"Successfully joined room: {roomName}");
                joinRoomButton.gameObject.SetActive(false);
                roomNameInput.gameObject.SetActive(false);
                createRoomButton.gameObject.SetActive(false);
                image.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError($"Failed to join room: {result.ShutdownReason}");
                clientRunner = null;
                await JoinLobbyAsync();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception during room join: {e.Message}");
            clientRunner = null;
            await JoinLobbyAsync();
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Session list updated. Found {sessionList.Count} sessions.");
        availableSessions = sessionList;

        foreach (Transform child in roomListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var session in sessionList)
        {
            if (!session.IsValid || string.IsNullOrEmpty(session.Name)) continue;

            Debug.Log($"Found session: {session.Name}");
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContainer);
            roomItem.GetComponentInChildren<TMP_Text>().text = session.Name;
            roomItem.GetComponent<Button>().onClick.AddListener(() => JoinRoomByNameAsync(session.Name));
        }
    }

    private async void JoinRoomByNameAsync(string roomName)
    {
        Debug.Log($"Joining room by name: {roomName}");

        clientRunner = await CreateNewRunnerAsync("ClientRunner", clientRunner);

        try
        {
            StartGameResult result = await clientRunner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = roomName
            });

            if (result.Ok)
            {
                Debug.Log($"Successfully joined room: {roomName}");
            }
            else
            {
                Debug.LogError($"Failed to join room: {result.ShutdownReason}");
                clientRunner = null;
                await JoinLobbyAsync();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception during room join by name: {e.Message}");
            clientRunner = null;
            await JoinLobbyAsync();
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log($"Connected to server with {runner.name}.");
        if(runner.IsServer || runner.IsSharedModeMasterClient)
        {
            Spawnplayer(runner);
        }
    }

    private void Spawnplayer(NetworkRunner runner)
    {
        if(runner != null && runner.IsRunning && PlayerPrefab != null)
        {
            Vector3 spawnposition = new Vector3(UnityEngine.Random.Range(-5, 5), 1, UnityEngine.Random.Range(-5, 5));
            NetworkObject playerobject = runner.Spawn(PlayerPrefab, spawnposition, Quaternion.identity, runner.LocalPlayer);
        }
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Disconnected from server with {runner.name}: {reason}");
        // If the hostRunner disconnects, clear the current room
        if (runner == hostRunner)
        {
            hostRunner = null;
            currentRoomName = null;
            Debug.Log("Host runner disconnected. Room is no longer active.");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player joined in {runner.name}: {player.PlayerId}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player left in {runner.name}: {player.PlayerId}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"NetworkRunner {runner.name} shutdown: {shutdownReason}");
        isShuttingDown = false;
        if (runner == hostRunner)
        {
            hostRunner = null;
            currentRoomName = null;
            Debug.Log("Host runner shut down. Room is no longer active.");
        }
        else if (runner == clientRunner)
        {
            clientRunner = null;
        }
        else if (runner == lobbyRunner)
        {
            lobbyRunner = null;
        }
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogError($"Connection failed with {runner.name}: {reason} at address {remoteAddress}");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log($"Scene load completed in {runner.name}.");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken token) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}