using System.Collections;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
   public enum AIstate { Idle, Walk, Attack, Defend, jump}
    public AIstate currentstate = AIstate.Idle;

    public Transform player;
    public float detectionrange = 10f;
    public float attackrange = 2f;
    public float movespeed = 2f;
    public int Health = 100;
    public int maxhealth = 100;
    public float defendchance = 0.2f;

   
    public float attackcooldown = 1.5f;
    public float regencooldown = 20f;

    public float Jumpforce = 10f;
    public Transform groundcheck;
    public LayerMask groundlayer;
    private bool isgrounded;

    private Rigidbody2D rb;
    private Animator anim;
    private bool canattack = true;
    private bool canregen = true;
    private bool isdefending = false;
    private bool isActive = true;
    public AudioClip Dieclip;
    public AudioClip punchclip;
    public AudioClip kickclip;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SetDifficulty();
    }

    private void Update()
    {
        if (!isActive) return;

        isgrounded = Physics2D.OverlapCircle(groundcheck.position, 0.2f, groundlayer);
        if(Health <= 0)
        {
            SoundManager.instance.PlayerSound(Dieclip);
            return;
        }
        if (player.position.y - transform.position.y > 1.5f && isgrounded)
        {
            currentstate = AIstate.jump;
        }
        switch (currentstate)
        {
            case AIstate.Idle:
                Idle();
                break;
            case AIstate.Walk:
                Walk();
                break;
            case AIstate.Attack:
                Attack();
                break;
            case AIstate.Defend:
                Defend();
                break;
            case AIstate.jump:
                Jump();
                break;
        }

        if(!isActive)return;
    }

    void Idle()
    {
        if(Vector3.Distance(transform.position, player.position) < detectionrange)
        {
            currentstate = AIstate.Walk;
        }
    }

    void Walk()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, movespeed*Time.deltaTime);
        anim.SetBool("walking", true);

        if(Vector3.Distance(transform.position,player.position) < attackrange)
        {
            anim.SetBool("walking", false);

            currentstate =(Random.value < defendchance) ? AIstate.Defend : AIstate.Attack;
        }

    }

    void Attack()
    {
        if (canattack)
        {
            canattack = false;
            if (Random.value < 0.5f)
            {
                SoundManager.instance.PlayerSound(punchclip);
                anim.SetTrigger("Punch");
            }
            else
            {
                SoundManager.instance.PlayerSound(kickclip);
                anim.SetTrigger("kick");
            }

           
            StartCoroutine(resetattack());
        }
    }

    void Defend()
    {
        anim.SetTrigger("Defence");
        isdefending = true;
        StartCoroutine(ResetDefend());
    }

    void Jump()
    {
        if (isgrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, Jumpforce);
            anim.SetTrigger("Jump");
        }
        currentstate = AIstate.Walk;
    }

    IEnumerator resetattack()
    {
        yield return new WaitForSeconds(attackcooldown);
        canattack = true;
        currentstate = AIstate.Walk;
    }

    IEnumerator ResetDefend()
    {
        yield return new WaitForSeconds(1f);
       currentstate = AIstate.Walk;
    }

    IEnumerator HealthRenge()
    {
        canregen = false;
        yield return new WaitForSeconds(regencooldown);
        Health += 30;
        if (Health > maxhealth) Health = maxhealth;
        canregen = true;
    }

   

    public void TakeDamage(int amount)
    {
        if (isdefending)
        {
            amount /= 2;
        }
        Health -= amount;
        if (canregen)
        {
            StartCoroutine(HealthRenge());
        }
    }

    public void resetAi()
    {
        isActive = true;
        Debug.Log("working");
    }
    public void StopAi()
    {
        isActive = false;
    }

    IEnumerator AI_Behavior()
    {
        while (isActive)
        {
            if (player != null && Vector3.Distance(transform.position, player.position) < detectionrange)
            {
                Walk();
            }
            yield return null;
        }
    }

    public void ResetAI()
    {      
        isActive = true;
        Health = maxhealth;

        transform.position = new Vector2(5, 0); 
        rb.velocity = Vector2.zero;

        canattack = true;
        isdefending = false;

        anim.ResetTrigger("Punch");
        anim.ResetTrigger("kick");
        anim.ResetTrigger("Defence");            
        anim.SetBool("walking", false);     
       currentstate = AIstate.Idle; 

        StopAllCoroutines();
        StartCoroutine(AI_Behavior());
    }

    void SetDifficulty()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty", "Normal");

        switch (difficulty)
        {
            case "Easy":
                movespeed = 1.5f;
                attackcooldown = 2f;
                defendchance = 0.1f;
                break;
            case "Normal":
                movespeed = 2f;
                attackcooldown = 1f;
                defendchance = 0.3f;
                break;
            case "Hard":
                movespeed = 3;
                attackcooldown = 0.1f;
                defendchance = 0.6f;
                break;
        }
    }
}
