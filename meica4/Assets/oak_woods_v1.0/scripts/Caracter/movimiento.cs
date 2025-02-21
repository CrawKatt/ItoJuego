using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movimiento : MonoBehaviour
{

    [SerializeField]
    float velocity;

    [SerializeField]
    float jumpForce;

    [SerializeField]
    float raycastLength;

    [SerializeField]
    LayerMask groundLayer;

    private bool isInGround;
    private Rigidbody2D rb;
    public Animator animator;
    private bool lookingRight = true;

    [Header("Vida")]
    [SerializeField]
    float maxLife;
    private float life;
    public Image lifeBar;

    private bool canMove = true;
    private bool isInvincible = false;

    [Header("Knockback")]
    [SerializeField]
    float knockbackForce;
    [SerializeField]
    float knockbackDuration;

    [Header("Invulnerabilidad")]
    [SerializeField]
    float invincibleTime;
    [SerializeField]
    float flickerSpeed;
    private SpriteRenderer spriteRenderer;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootForce = 10f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Doble Salto")]
    private bool canDoubleJump = false;
    private bool hasDoubleJumpAbility = false; // 🔹 Nueva variable
    public Image doubleJumpIcon;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        life = maxLife;
        UpdateLifeBar();

        if (doubleJumpIcon != null)
        {
            doubleJumpIcon.enabled = false;
        }

        if (firePoint == null)
        {
            firePoint = transform.Find("firePoint");
            if (firePoint == null)
            {
                Debug.LogError("❌ No se encontró 'firePoint' en la jerarquía.");
            }
        }
    }

    void Update()
    {
        if (!canMove) return;

        float velocityX = Input.GetAxis("Horizontal") * velocity * Time.deltaTime;
        animator.SetFloat("Movement", Mathf.Abs(velocityX));

        if (velocityX < 0 && lookingRight)
        {
            TurnCharacter();
        }
        else if (velocityX > 0 && !lookingRight)
        {
            TurnCharacter();
        }

        transform.position += new Vector3(velocityX, 0, 0);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, groundLayer);
        isInGround = hit.collider != null;

        // 🔹 Si tocamos el suelo, restablecemos el doble salto
        if (isInGround)
        {
            canDoubleJump = hasDoubleJumpAbility;
        }

        // 🔹 Salto normal o doble salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isInGround)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // 🔹 Solo permite un doble salto
            }
        }

        animator.SetBool("suelo", isInGround);

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            Shooter();
            nextFireTime = Time.time + fireRate;
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
        if (firePoint == null || projectilePrefab == null)
        {
            Debug.LogError("⚠️ FirePoint o el prefab del proyectil NO están asignados.");
            return;
        }

        float direccion = lookingRight ? 1f : -1f;
        Vector2 shootDirection = new Vector2(direccion, 0);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetDirection(shootDirection);
        }
    }

    public void TakeDamage(float dano)
    {
        if (isInvincible) return;

        life -= dano;
        life = Mathf.Clamp(life, 0, maxLife);
        UpdateLifeBar();
        animator.SetTrigger("TomarDano");

        StartCoroutine(KnockbackRutina());
        StartCoroutine(SetupInvincibleStatus());

        if (life <= 0)
        {
            Die();
        }
    }

    IEnumerator KnockbackRutina()
    {
        canMove = false;
        float direccion = lookingRight ? -1 : 1;
        rb.velocity = new Vector2(knockbackForce * direccion, rb.velocity.y);

        yield return new WaitForSeconds(knockbackDuration);
        canMove = true;
    }

    IEnumerator SetupInvincibleStatus()
    {
        isInvincible = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemigo"), true);

        float tiempo = 0f;
        while (tiempo < invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerSpeed);
            tiempo += flickerSpeed;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemigo"), false);
    }

    void UpdateLifeBar()
    {
        if (lifeBar != null)
        {
            lifeBar.fillAmount = life / maxLife;
        }
    }

    void Die()
    {
        Debug.Log("Personaje ha muerto");
        gameObject.SetActive(false);
    }

    // 🔹 Activar doble salto al obtener el ítem
    public void EnableDoubleJump()
    {
        hasDoubleJumpAbility = true;
        canDoubleJump = true;

        Debug.Log("✨ ¡Doble salto activado!");

        if (doubleJumpIcon != null)
        {
            doubleJumpIcon.enabled = true; // Activar el ícono

            // 🔹 Asegurar que la opacidad sea 100%
            Color iconColor = doubleJumpIcon.color;
            iconColor.a = 1f; // Alpha en 1 (totalmente visible)
            doubleJumpIcon.color = iconColor;
        }
        else
        {
            Debug.LogError("⚠️ No se asignó un ícono de doble salto en el Inspector.");
        }
    }
}