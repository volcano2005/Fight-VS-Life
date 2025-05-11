using UnityEngine;
using Fusion;
using System.Collections;

public class MultiplayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Networked] public float CurrentHealth { get; set; } = 100f;
    public float MaxHealth = 100f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 originalScale;
    private bool isGrounded = false;

    public HealthUIManager healthUI;

    private Vector2 moveDirection;
    private bool jumpPressed;
    private bool regenPressed;
    private bool canRegen = true;

    public float regenAmount = 30f;
    public float regenCooldown = 10f;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;

        if (HasInputAuthority)
            healthUI.AssignLocalPlayer(this);
        else
            healthUI.AssignRemotePlayer(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerNetworkInput input))
        {
            moveDirection = input.MovementDirection;
            jumpPressed = input.JumpPressed;
            regenPressed = input.RegenPressed;
        }

        HandleMovement();
        HandleJump();
        HandleFlip();
        HandleRegen();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        Vector2 targetVelocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);
        rb.velocity = targetVelocity;
    }

    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            animator?.SetTrigger("Jump");
        }
    }

    private void HandleRegen()
    {
        if (regenPressed && canRegen)
        {
            StartCoroutine(Regenerate());
        }
    }

    private IEnumerator Regenerate()
    {
        canRegen = false;
        healthUI?.UpdateRegenUI(false);

        float targetHealth = Mathf.Min(CurrentHealth + regenAmount, MaxHealth);

        while (CurrentHealth < targetHealth)
        {
            if (HasStateAuthority)
            {
                CurrentHealth += 2f;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            }
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(regenCooldown);
        canRegen = true;
        healthUI?.UpdateRegenUI(true);
    }

    private void UpdateAnimations()
    {
        if (animator)
        {
            animator.SetFloat("IsWalking", Mathf.Abs(moveDirection.x));
            animator.SetBool("IsGrounded", isGrounded);
        }
    }

    private void HandleFlip()
    {
        if (moveDirection.x != 0)
        {
            transform.localScale = new Vector3(originalScale.x * Mathf.Sign(moveDirection.x), originalScale.y, originalScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (HasStateAuthority)
        {
            CurrentHealth -= amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            Debug.Log($"[Server] Player {Object.InputAuthority} took {amount} damage. Health: {CurrentHealth}");
        }
        else
        {
            RPC_TakeDamage(amount);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        Debug.Log($"[RPC] Player {Object.InputAuthority} took {amount} damage. Health: {CurrentHealth}");
    }
}