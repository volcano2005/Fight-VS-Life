using System.Collections;
using UnityEngine;

public class PlayerDefence : MonoBehaviour
{
    public HealthBar PlayerHealth;
    public float BlockDamageReduction = 0.5f;
    public float SuperBlockCooldown = 5f;
    public float SuperBlockDuration = 2f;

    public KeyCode BlockKey = KeyCode.C;
    public KeyCode SuperBlockKey = KeyCode.V;

    private bool isBlocking = false;
    private bool isSuperBlocking = false;
    private bool canSuperBlock = true;
    private Animator anim;

    public Transform aiEnemy;  
    private bool isFacingRight = true;
    private PlayerAttack playerattack;

    private void Start()
    {
        anim = GetComponent<Animator>();
        playerattack = GetComponent<PlayerAttack>();
        SetDifficulty();
    }

    private void Update()
    {
        HandleDefense();
        FlipTowardsEnemy();
    }

    void HandleDefense()
    {
        if (Input.GetKey(BlockKey))
        {
            if (!isBlocking)
            {
                isBlocking = true;
                anim.SetBool("Block", true);              
            }
        }
        else
        {
            if (isBlocking)
            {
                isBlocking = false;
                anim.SetBool("Block", false);
            }
        }

        if (Input.GetKeyDown(SuperBlockKey) && canSuperBlock)
        {
            StartCoroutine(ActivateSuperBlock());
        }
    }

    IEnumerator ActivateSuperBlock()
    {
        isSuperBlocking = true;
        canSuperBlock = false;
        anim.SetBool("Block", true);

        yield return new WaitForSeconds(SuperBlockDuration);

        isSuperBlocking = false;
        anim.SetBool("Block", false);

        yield return new WaitForSeconds(SuperBlockCooldown);
        canSuperBlock = true;

    }

    public void TakeDamage(int damage)
    {
        if (isSuperBlocking)
        {
            return;
        }

        if (isBlocking)
        {
            damage = Mathf.RoundToInt(damage * BlockDamageReduction);
        }

        PlayerHealth.TakeDamage(damage);

        if (PlayerHealth.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("Die");
        gameObject.SetActive(false);
    }

    private void FlipTowardsEnemy()
    {
        if (aiEnemy == null) return;

        float direction = aiEnemy.position.x - transform.position.x;
        if ((direction > 0 && !isFacingRight) || (direction < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }

    }

    void SetDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");

        switch (difficulty)
        {
            case "Easy":
                SuperBlockCooldown = 5f;
                BlockDamageReduction = 0.5f;
                break;
            case "Normal":
                SuperBlockCooldown = 5f;
                BlockDamageReduction = 0.5f;
                break;
            case "Hard":
                SuperBlockCooldown = 3f;
                BlockDamageReduction = 0.3f;
                break;
        }
    }



}