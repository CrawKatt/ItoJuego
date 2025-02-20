using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzasalto = 10f;
    public float longitudRaycast = 0.1f;
    public LayerMask capasuelo;

    private bool suelo;
    private Rigidbody2D rb;
    public Animator animator;
    private bool mirandoDerecha = true;

    [Header("Vida")]
    public float vidaMaxima = 100f;
    private float vidaActual;
    public Image barraDeVida;

    private bool puedeMoverse = true;
    private bool esInvulnerable = false;

    [Header("Knockback")]
    public float knockbackFuerza = 5f;
    public float knockbackDuracion = 0.2f;

    [Header("Invulnerabilidad")]
    public float tiempoInvulnerabilidad = 1.5f;
    public float velocidadParpadeo = 0.1f;
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
        vidaActual = vidaMaxima;
        ActualizarBarraDeVida();

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
        if (!puedeMoverse) return;

        float velocidadX = Input.GetAxis("Horizontal") * velocidad * Time.deltaTime;
        animator.SetFloat("Movement", Mathf.Abs(velocidadX));

        if (velocidadX < 0 && mirandoDerecha)
        {
            Girar();
        }
        else if (velocidadX > 0 && !mirandoDerecha)
        {
            Girar();
        }

        transform.position += new Vector3(velocidadX, 0, 0);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capasuelo);
        bool estabaEnSuelo = suelo;
        suelo = hit.collider != null;

        // 🔹 Si tocamos el suelo, restablecemos el doble salto
        if (suelo && !estabaEnSuelo)
        {
            canDoubleJump = hasDoubleJumpAbility;
        }

        // 🔹 Salto normal o doble salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (suelo)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false; // 🔹 Solo permite un doble salto
            }
        }

        animator.SetBool("suelo", suelo);

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextFireTime)
        {
            Shooter();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.localScale = new Vector3(mirandoDerecha ? 1 : -1, 1, 1);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0f, fuerzasalto), ForceMode2D.Impulse);
    }

    void Shooter()
    {
        if (firePoint == null || projectilePrefab == null)
        {
            Debug.LogError("⚠️ FirePoint o el prefab del proyectil NO están asignados.");
            return;
        }

        float direccion = mirandoDerecha ? 1f : -1f;
        Vector2 shootDirection = new Vector2(direccion, 0);

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetDirection(shootDirection);
        }
    }

    public void TomarDano(float dano)
    {
        if (esInvulnerable) return;

        vidaActual -= dano;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        ActualizarBarraDeVida();
        animator.SetTrigger("TomarDano");

        StartCoroutine(KnockbackRutina());
        StartCoroutine(InvulnerabilidadRutina());

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    IEnumerator KnockbackRutina()
    {
        puedeMoverse = false;
        float direccion = mirandoDerecha ? -1 : 1;
        rb.velocity = new Vector2(knockbackFuerza * direccion, rb.velocity.y);

        yield return new WaitForSeconds(knockbackDuracion);
        puedeMoverse = true;
    }

    IEnumerator InvulnerabilidadRutina()
    {
        esInvulnerable = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemigo"), true);

        float tiempo = 0f;
        while (tiempo < tiempoInvulnerabilidad)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(velocidadParpadeo);
            tiempo += velocidadParpadeo;
        }

        spriteRenderer.enabled = true;
        esInvulnerable = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemigo"), false);
    }

    void ActualizarBarraDeVida()
    {
        if (barraDeVida != null)
        {
            barraDeVida.fillAmount = vidaActual / vidaMaxima;
        }
    }

    void Morir()
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