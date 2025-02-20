using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab de la flecha
    public Transform firePoint; // Punto de disparo
    public float fireRate = 0.5f; // Tiempo entre disparos
    public float shootForce = 10f; // Fuerza de disparo

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= nextFireTime) // Dispara con "Espacio"
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Controla la cadencia de disparo
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("El prefab de la flecha no está asignado en el Inspector.");
            return;
        }

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = newProjectile.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("El prefab de la flecha no tiene un Rigidbody2D. Revisa su configuración.");
            return;
        }

        // Obtener la dirección basada en la orientación del personaje
        float direccion = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 shootDirection = new Vector2(direccion, 0);

        // Aplicar velocidad en la dirección correcta
        rb.velocity = shootDirection * shootForce;

        // Obtener la escala original de la flecha y ajustarla sin hacerla gigante
        Vector3 escalaOriginal = projectilePrefab.transform.localScale;
        newProjectile.transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x) * direccion, escalaOriginal.y, escalaOriginal.z);

        Destroy(newProjectile, 3f);
    }


}
