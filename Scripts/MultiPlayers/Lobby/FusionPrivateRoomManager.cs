using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionPrivateRoomManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner runner;
    public float waitTime = 120f;
    private Coroutine waitCoroutine;
    private LobbyUIManager uiManager;
    private string currentSessionName;
    public GameObject LobbyUI;

    [Header("Player Prefabs")]
    public NetworkPrefabRef hostPrefab;
    public NetworkPrefabRef clientPrefab;

    [Header("Scenes")]
    public string waitingRoomSceneName = "WaitingRoomScene";
    public string fightSceneName = "FightScene";

    public static string _tempSessionName;
    public static string TempSessionName => _tempSessionName;
    public string CurrentSessionName => currentSessionName;

    private void Awake()
    {
        uiManager = FindObjectOfType<LobbyUIManager>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (runner != null && runner.IsRunning && !string.IsNullOrEmpty(currentSessionName) && uiManager != null)
        {
            uiManager.UpdateRoomCode(currentSessionName);
        }
    }

    public void CreateGame()
    {
        if (runner != null)
        {
            runner.Shutdown();
            Destroy(runner);
        }

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        currentSessionName = GenerateRandomRoomCode();
        _tempSessionName = currentSessionName;

        int waitingRoomIndex = SceneUtility.GetBuildIndexByScenePath(waitingRoomSceneName);
        SceneRef waitingRoomScene = SceneRef.FromIndex(waitingRoomIndex);

        runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = currentSessionName,
            Scene = waitingRoomScene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        LobbyUI.gameObject.SetActive(false);
        StartCoroutine(LoadWaitingRoomAfterStart());
        waitCoroutine = StartCoroutine(WaitForClientTimeout());
    }

    private IEnumerator LoadWaitingRoomAfterStart()
    {
        yield return null;
        SceneManager.LoadScene(waitingRoomSceneName);
        uiManager?.UpdateRoomCode(currentSessionName);
    }

    private IEnumerator WaitForClientTimeout()
    {
        float timer = 0;
        while (timer < waitTime)
        {
            if (runner.SessionInfo != null && runner.SessionInfo.PlayerCount > 1)
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }

        runner.Shutdown();
        SceneManager.LoadScene("LobbyScene");
    }

    public void JoinGame(string joinCode)
    {
        if (runner != null)
        {
            runner.Shutdown();
            Destroy(runner);
        }

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        runner.AddCallbacks(this);

        currentSessionName = joinCode;
        _tempSessionName = currentSessionName;

        int waitingRoomIndex = SceneUtility.GetBuildIndexByScenePath(waitingRoomSceneName);
        SceneRef initialScene = SceneRef.FromIndex(waitingRoomIndex);

        runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = currentSessionName,
            Scene = initialScene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        SceneManager.LoadScene(waitingRoomSceneName);
        uiManager?.UpdateRoomCode(currentSessionName);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer && runner.SessionInfo.PlayerCount == 2)
        {
            if (waitCoroutine != null)
                StopCoroutine(waitCoroutine);

            runner.LoadScene(fightSceneName);
        }
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (!runner.IsServer) return;

        int index = 0;
        foreach (var player in runner.ActivePlayers)
        {
            Vector3 spawnPos = index == 0 ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0);
            NetworkPrefabRef prefabToSpawn = index == 0 ? hostPrefab : clientPrefab;
            Quaternion rotation = index == 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

            runner.Spawn(prefabToSpawn, spawnPos, rotation, player);

            index++;
        }
    }

    private string GenerateRandomRoomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] code = new char[6];
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(code);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        runner?.Shutdown();
        _tempSessionName = null;
        SceneManager.LoadScene("LobbyScene");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerNetworkInput playerInput = new PlayerNetworkInput();

        playerInput.MovementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        playerInput.JumpPressed = Input.GetKeyDown(KeyCode.Space);
        playerInput.RegenPressed = Input.GetKeyDown(KeyCode.K);

        input.Set(playerInput);
    }

    // Unused callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }  
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
}
