using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
    public Image localHealthBar;
    public Image remoteHealthBar;

    public Image localRegenIcon;

    private MultiplayerMovement localPlayer;
    private MultiplayerMovement remotePlayer;

    private Color availableColor = Color.white;
    private Color cooldownColor = Color.black;

    void Update()
    {
        if (localPlayer != null)
        {
            localHealthBar.fillAmount = localPlayer.CurrentHealth / localPlayer.MaxHealth;
        }

        if (remotePlayer != null)
        {
            remoteHealthBar.fillAmount = remotePlayer.CurrentHealth / remotePlayer.MaxHealth;
        }
    }

    public void AssignLocalPlayer(MultiplayerMovement player)
    {
        localPlayer = player;
        UpdateRegenUI(true);
    }

    public void AssignRemotePlayer(MultiplayerMovement player)
    {
        remotePlayer = player;
    }

    public void UpdateRegenUI(bool isAvailable)
    {
        if (localRegenIcon != null)
        {
            localRegenIcon.color = isAvailable ? availableColor : cooldownColor;
        }
    }
}
