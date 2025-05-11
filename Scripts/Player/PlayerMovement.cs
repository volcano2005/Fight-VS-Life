using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float Movespeed = 5f;
    public float Jumpforce = 5f;
    private Rigidbody2D Rb;
    private Animator anim;
    private bool isGrounded;
    public AudioClip Walkclip;

    private void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        move();
    }

    private void Update()
    {
        Movement();
        Jump();
        Attack();       
    }

    public void move()
    {
        float Moveinput = Input.GetAxis("Horizontal");
        if (Moveinput > 0)
        {
            SoundManager.instance.PlayerSound(Walkclip);
        }

    }


    private void Movement()
    {
        float Moveinput = Input.GetAxis("Horizontal");
        Rb.velocity = new Vector2(Moveinput*Movespeed, Rb.velocity.y);

        if(Mathf.Abs(Moveinput) > 0.1f)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void Jump()
    {
        if(Input.GetKey(KeyCode.W) && isGrounded)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, Jumpforce);
            anim.SetTrigger("Jump");
            isGrounded = false; 
        }
    }

    void Attack()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            anim.SetTrigger("Kick");
        }
        
        if (Input.GetKey(KeyCode.X))
        {
            anim.SetTrigger("Punch");
        }       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

}



