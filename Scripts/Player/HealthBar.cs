using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public Image healthBar;

    private bool canRegenerate = true;
    public float regencooldown = 10f;
    public float regenAmount = 30;
    public KeyCode regenkey = KeyCode.K;

    public Image Regenicon;
    private Color availablecolor = Color.white;
    private Color Cooldowncolor = Color.black;
    public AudioClip Dieclip;


    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (Input.GetKeyDown(regenkey))
        {
            StartCoroutine(ReagenerateHealth());
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            SoundManager.instance.PlayerSound(Dieclip);
            Die();
            FindObjectOfType<RoundManager>().EndRound(false);
        }
    }

    IEnumerator ReagenerateHealth()
    {
        canRegenerate = false;
        UpdateRegenUI();

        float targetHealth = Mathf.Min(currentHealth + regenAmount, maxHealth);

        while(currentHealth < targetHealth)
        {
            currentHealth += 2f;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            UpdateHealthBar();
            yield return new WaitForSeconds(0.1f);
            
        }
        yield return new WaitForSeconds(regencooldown);
        canRegenerate = true;
        UpdateRegenUI();
       
    }

    void UpdateHealthBar()
    {
        if(healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
          
        }
    }
     

    void UpdateRegenUI()
    {
        if(healthBar != null)
        {
            Regenicon.color = canRegenerate ? availablecolor : Cooldowncolor;
        }
        
    }
    public void RestHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        gameObject.SetActive(true);
    }


    void Die() 
    {  
        gameObject.SetActive(false);
    }
}


