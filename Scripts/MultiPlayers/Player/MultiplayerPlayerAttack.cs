using UnityEngine;
using Fusion;

public class MultiplayerPlayerAttack : NetworkBehaviour
{
    public float Damage1 = 20f;
    public float Damage2 = 25f;
    public float knockbackForce = 5f;

    private KeyCode KickAttack = KeyCode.C;
    private KeyCode PunchAttack = KeyCode.V;

    public Transform attackPoint;
    public float attackRange = 1;
    public LayerMask playerLayerMask;

    private void Update()
    {
        if (!HasInputAuthority) return;

        if (Input.GetKeyDown(PunchAttack))
        {
            Attack1();
        }
        if (Input.GetKeyDown(KickAttack))
        {
            Attack2();
        }
    }

    private void Attack1()
    {
        RpcDoAttack(Damage1);
    }

    private void Attack2()
    {
        RpcDoAttack(Damage2);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RpcDoAttack(float damage)
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayerMask);
        Debug.Log($"RpcDoAttack by {Object.InputAuthority} hit {hitPlayers.Length} players.");

        foreach (Collider2D playerCollider in hitPlayers)
        {
            MultiplayerMovement player = playerCollider.GetComponent<MultiplayerMovement>();
            if (player != null && player != this)
            {
                Debug.Log($"Damaging {playerCollider.name}");

                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;

                player.TakeDamage(damage);

                Rigidbody2D enemyRb = playerCollider.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
        foreach (var p in FindObjectsOfType<MultiplayerMovement>())
        {
            float dist = Vector2.Distance(p.transform.position, attackPoint.position);
            Debug.Log($"Player {p.name} is {dist} units away from attack point.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}