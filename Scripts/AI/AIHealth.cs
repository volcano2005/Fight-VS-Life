using UnityEngine;
using UnityEngine.UI;
using static EnemyAi;

public class AIHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public Image healthBar;
    public Image regenCooldownBar;
    public GameObject aiDeathEffect;

    public float regenerationAmount = 30f; 
    public float regenerationCooldown = 10f;

    private bool canRegenerate = true;
    private float cooldownTimer;
    public AudioClip DieClip;

    void Start()
    {
        SetDifficulty();
        currentHealth = maxHealth;
        UpdateHealthBar();
        UpdateCooldownBar(1);       
    }

    void Update()
    {
        if (!canRegenerate)
        {
            cooldownTimer -= Time.deltaTime;
            UpdateCooldownBar(cooldownTimer / regenerationCooldown);

            if (cooldownTimer <= 0)
            {
                canRegenerate = true;
                regenCooldownBar.color = Color.white;
            }
        }

        if(currentHealth < maxHealth / 2 && canRegenerate)
        {
            RegenerateHealth();
        }         
    }

    public void TakeDamage(float damage, Vector2 KnockbackDirection, float KnockBackforce)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        UpdateHealthBar();

        Rigidbody2D rb =GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.AddForce(KnockbackDirection * KnockBackforce, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
        {
            Die();
            FindAnyObjectByType<RoundManager>().EndRound(true);
            gameObject.SetActive(true);           
        }
    }

    public void RegenerateHealth()
    {
        if (canRegenerate && currentHealth > 0 && currentHealth < maxHealth)
        {
            currentHealth += regenerationAmount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            UpdateHealthBar();
            StartCooldown();
        }
    }

    void StartCooldown()
    {
        canRegenerate = false;
        cooldownTimer = regenerationCooldown;
        regenCooldownBar.color = Color.black;
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    void UpdateCooldownBar(float value)
    {
        regenCooldownBar.fillAmount = value;
    }
  
    void Die()
    {
        SoundManager.instance.PlayerSound(DieClip);
       gameObject.SetActive(false);
    }

    public void RestHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();   
       
    }

    void SetDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");

        switch (difficulty)
        {
            case "Easy":
                maxHealth = 100f;
                regenerationAmount = 40f;
                regenerationCooldown = 0.4f;
                break;
            case "Normal":
                maxHealth = 150f;
                regenerationAmount = 50f;
                regenerationCooldown = 2f;
                break;
            case "Hard":
                maxHealth = 200f;
                regenerationAmount = 70f;
                regenerationCooldown = 5f;
                break;
        }      
    }


}