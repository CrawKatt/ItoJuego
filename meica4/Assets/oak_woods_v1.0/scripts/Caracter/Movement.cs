using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public enum PlayerState { Idle, Running, Jumping, DoubleJumping, Attacking, Hurt, Dead }
    
    [SerializeField] float velocity; // 5f
    [SerializeField] float jumpForce; // 6f
    [SerializeField] float raycastLength; // 0.68f
    [SerializeField] LayerMask groundLayer;
    
    private bool isInGround;
    private Rigidbody2D rb;
    public Animator animator;
    private bool lookingRight = true;
    
    [Header("Vida")]
    [SerializeField] float maxLife; // 100
    private float life;
    public Image lifeBar;

    private bool canMove = true;
    private bool isInvincible = false;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce; // 10f
    public float fireRate; // 0.5f
    private float nextFireTime; // 0

    [Header("Doble Salto")]
    private bool canDoubleJump = false; 
    private bool hasDoubleJumpAbility = false;

    private PlayerState state;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        life = maxLife;
        UpdateLifeBar();

        if (firePoint == null)
        {
            firePoint = transform.Find("firePoint");
        }
    }

    void Update()
    {
        if (!canMove) return;

        float velocityX = Input.GetAxis("Horizontal") * velocity * Time.deltaTime;
        if (Mathf.Abs(velocityX) > 0 && isInGround)
        {
            state = PlayerState.Running;
        }
        else if (isInGround)
        {
            state = PlayerState.Idle;
        }

        transform.position += new Vector3(velocityX, 0, 0);
        animator.SetFloat("Movement", Mathf.Abs(velocityX));

        if (velocityX < 0 && lookingRight) TurnCharacter();
        else if (velocityX > 0 && !lookingRight) TurnCharacter();

        isInGround = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, groundLayer);
        animator.SetBool("suelo", isInGround);

        if (isInGround) canDoubleJump = hasDoubleJumpAbility;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isInGround)
            {
                Jump();
                state = PlayerState.Jumping;
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
                state = PlayerState.DoubleJumping;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            Shooter();
            nextFireTime = Time.time + fireRate;
            state = PlayerState.Attacking;
        }
    }

    void TurnCharacter()
    {
        lookingRight = !lookingRight;
        transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
    }

    void Shooter()
    {
        if (firePoint == null || projectilePrefab == null) return;

        float direccion = lookingRight ? 1f : -1f;
        Vector2 shootDirection = new Vector2(direccion, 0);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null) projectileScript.SetDirection(shootDirection);
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        life -= damage;
        life = Mathf.Clamp(life, 0, maxLife);
        UpdateLifeBar();
        animator.SetTrigger("TomarDano");

        StartCoroutine(KnockbackRutina());
        StartCoroutine(SetupInvincibleStatus());

        if (life <= 0)
        {
            state = PlayerState.Dead;
            Die();
        }
        else
        {
            state = PlayerState.Hurt;
        }
    }

    IEnumerator KnockbackRutina()
    {
        canMove = false;
        float direccion = lookingRight ? -1 : 1;
        rb.velocity = new Vector2(5 * direccion, rb.velocity.y);

        yield return new WaitForSeconds(0.2f);
        canMove = true;
        state = isInGround ? PlayerState.Idle : PlayerState.Jumping;
    }

    IEnumerator SetupInvincibleStatus()
    {
        isInvincible = true;
        float tiempo = 0f;
        while (tiempo < 1.5f)
        {
            GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(0.1f);
            tiempo += 0.1f;
        }

        GetComponent<SpriteRenderer>().enabled = true;
        isInvincible = false;
    }

    void UpdateLifeBar()
    {
        if (lifeBar != null) lifeBar.fillAmount = life / maxLife;
    }

    void Die()
    {
        Debug.Log("Personaje ha muerto");
        gameObject.SetActive(false);
    }

    public void EnableDoubleJump()
    {
        hasDoubleJumpAbility = true;
        canDoubleJump = true;
    }
}
