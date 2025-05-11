using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RoundManager : MonoBehaviour
{
    public HealthBar player;
    public AIHealth enemy;
    public TextMeshProUGUI roundCounter;
    public TextMeshProUGUI timerText;
    public GameObject PlayerWinpanel;
    public GameObject AIWinPanel;

    private int playerWins = 0;
    private int enemyWins = 0;
    private int currentRound = 1;
    private bool roundOver = false;

    [SerializeField] private Transform playerStartPos;
    [SerializeField] private Transform enemyStartPos;

    private float roundTime = 30f;
    private float currentTime;
    private bool timerunning;

    void Start()
    {
        UpdateRoundText();
        ResetRound();
    }

    private void Update()
    {
        if (timerunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimeText();
            if(currentTime <= 0)
            {
                EndRoundOnTime();
            }
        }
    }

    public void EndRound(bool playerWon)
    {
        if (roundOver) return;
        roundOver = true;

        timerunning = false;

        if (playerWon) playerWins++;
        else enemyWins++;

        currentRound++;
        UpdateRoundText();

        if (playerWins >= 3)
        {
            roundCounter.text = "Player Wins!";
            StartCoroutine(EndMatch());
      
        }
        else if (enemyWins >= 3)
        {           
            roundCounter.text = "Enemy Wins!";
            StartCoroutine(EndMatch());
        }
        else
        {
            StartCoroutine(RestRoundcoroutine());
        }
    }

    private void EndRoundOnTime()
    {
        timerunning = false;
        if(player.currentHealth > enemy.currentHealth)
        {
            EndRound(true);
        }
        else if(enemy.currentHealth > player.currentHealth)
        {
            EndRound(false);
        }
        else
        {
            StartCoroutine(RestRoundcoroutine());
        }
    }

    IEnumerator RestRoundcoroutine()
    {
        roundCounter.text = "Next Round...";
        yield return new WaitForSeconds(2);
        ResetRound();
    }

    void ResetRound()
    {
        roundOver = false;

        player.RestHealth();
        enemy.RestHealth();

        player.transform.position = playerStartPos.position;
        enemy.transform.position = enemyStartPos.position;

        player.gameObject.SetActive(true);
        enemy.gameObject.SetActive(true);

        EnemyAi aiScript = enemy.GetComponent<EnemyAi>();
        if (aiScript != null)
        {
            StartCoroutine(ResetAI(aiScript));
        }

        currentTime = roundTime;

        timerunning = true;

        UpdateRoundText();
        UpdateTimeText();
    }   
        

    IEnumerator ResetAI(EnemyAi aiScript)
    {
        yield return null;
        aiScript.ResetAI();
    }

    IEnumerator EndMatch()
    {
        if (player.currentHealth > enemy.currentHealth)
        {
            PlayerWinpanel.gameObject.SetActive(true);
        } 
        if (player.currentHealth < enemy.currentHealth)
        {
            AIWinPanel.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(1);
    }

    void UpdateRoundText()
    {
        roundCounter.text = $"Round: {currentRound} Player: {playerWins}  Enemy: {enemyWins}";
    }

    void UpdateTimeText()
    {
        timerText.text = $"Time:{Mathf.CeilToInt(currentTime)}s";
    }

}
