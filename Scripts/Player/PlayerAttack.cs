using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float Damage1 = 20f;
    public float Damage2 = 25f;
    public LayerMask ailayer;
    public float knockbackforce = 5f;
    public KeyCode KickAttack;
    public KeyCode PunchAttack;
    public Transform attackpoint;
    public float attackrange = 0.05f;
    public AudioClip Punchclip;
    public AudioClip Kickclip;


    private void Start()
    {
        SetDifficulty();        
    }
    private void Update()
    {
        if (Input.GetKeyDown(PunchAttack))
        {
            SoundManager.instance.PlayerSound(Punchclip);
            Attack1();
        }
        if (Input.GetKeyDown(KickAttack))
        {
           SoundManager.instance.PlayerSound(Kickclip);
            Attack2();
        }

    }
    private void Attack1()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackpoint.position, attackrange, ailayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            AIHealth aihealth = enemy.GetComponent<AIHealth>();
            if (aihealth != null)
            {
                Vector2 knockbackdirection = (enemy.transform.position - transform.position).normalized;

                aihealth.TakeDamage(Damage1, knockbackdirection, knockbackforce);
            }
        }
    }
    
    private void Attack2()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackpoint.position, attackrange, ailayer);
        foreach(Collider2D enemy in hitEnemies)
        {
            AIHealth aihealth = enemy.GetComponent<AIHealth>();
            if (aihealth != null)
            {
                Vector2 knockbackdirection = (enemy.transform.position - transform.position).normalized;

                aihealth.TakeDamage(Damage2, knockbackdirection, knockbackforce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackpoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackpoint.position, attackrange);
    }

    void SetDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");

        switch (difficulty)
        {
            case "Easy":
                Damage1 = 10f;
                break;
            case "Normal":
                Damage1 = 30f;
                break;
            case "Hard":
                Damage1 = 30f;
                knockbackforce = 7f;
                break;
        }
    }

}
