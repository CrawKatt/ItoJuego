using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;

    public float fuerzasalto = 10f;
    public float longitudRaycast = 0.1f;
    public LayerMask capasuelo;

    private bool suelo;
    private bool recibiendoDanio;
    private Rigidbody2D rb;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velocidad;

        animator.SetFloat("Movement", Mathf.Abs(velocidadX * velocidad)); // Asegúrate de usar valores absolutos para la animación.

        if (velocidadX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (velocidadX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        Vector3 posicion = transform.position;

        transform.position = new Vector3(velocidadX + posicion.x, posicion.y, posicion.z);

        // Corregido: Se usa capasuelo como LayerMask
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capasuelo);
        suelo = hit.collider != null;

        if (suelo && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0f, fuerzasalto), ForceMode2D.Impulse);
        }

        animator.SetBool("suelo", suelo);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
