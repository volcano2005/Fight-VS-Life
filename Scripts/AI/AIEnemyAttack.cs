using UnityEngine;

public class AIEnemyAttack : MonoBehaviour
{
    public float Damage = 15f;
    public LayerMask playerlayer;
    public float knockbackforce = 5f;
    public Transform attackpoint;
    public float attackrange = 0.5f;
    public float attackCooldown = 1f;

    private float nextAttackTime = 0f;

    public Transform player;  
    private bool isFacingRight = true;

    

    private void Start()
    {
        GameObject playerobject = GameObject.FindWithTag("Player");

        if (playerobject != null)
        {
            player = playerobject.transform;
        }

        SetDifficulty();      
    }
    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        FlipAI();
    }

    private void Attack()
    {
       
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackpoint.position, attackrange, playerlayer);

        foreach (Collider2D player in hitPlayers)
        {
            PlayerDefence playerDefence = player.GetComponent<PlayerDefence>();
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (playerDefence != null && rb != null)
            {
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                rb.AddForce(knockbackDirection * knockbackforce, ForceMode2D.Impulse);

                playerDefence.TakeDamage((int)Damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackpoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackpoint.position, attackrange);
    }

    private void FlipAI()
    {
        if (player == null) return;

        float direction = player.position.x - transform.position.x;
        if ((direction < 0 && !isFacingRight) || (direction > 0 && isFacingRight))
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
                Damage = 10f;
                attackCooldown = 0.5f;
                break;
            case "Normal":
                Damage = 25f;
                attackCooldown = 0.3f;
                break;
            case "Hard":
                Damage = 40f;
                attackCooldown = 0.2f;
                break;
        }
    }

}



