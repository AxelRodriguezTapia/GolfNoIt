using UnityEngine;

public class PortalScript : MonoBehaviour
{
    // La referencia al otro portal para el teletransporte
    public Transform otroPortal;
    private float portalCooldown = 0;
    private bool portalPermission = true;

    private void Start()
    {
    }

    private void Update()
    {
        if (!portalPermission)
        {
            portalCooldown += Time.deltaTime;
            //Debug.Log("CoolIntheDown"+portalCooldown);

            // Durante el cooldown, desactivamos las partículas
            if (portalCooldown >= 2)
            {
                
                // Reactivar el portal
                otroPortal.gameObject.GetComponent<PortalScript>().setPermission(true);
                setPermission(true);

                portalCooldown = 0;
            }
        }
        else
        {
            portalCooldown = 0;
        }
    }

    // Método que se llama cuando la pelota entra en el portal
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (portalPermission)
        {
            if (other.gameObject.name == "Ball")
            {
                // Obtiene el Rigidbody2D de la pelota
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    // Teletransporta la pelota al otro portal
                    Teletransportar(other.transform, rb);
                }
            }
        }
    }

    // Método para teletransportar la pelota
    private void Teletransportar(Transform pelota, Rigidbody2D rb)
    {
        // Desactiva el permiso del otro portal
        otroPortal.gameObject.GetComponent<PortalScript>().setPermission(false);
        setPermission(false);

        // Posiciona la pelota en el otro portal
        pelota.position = otroPortal.position;

        // Mantiene la direccion y la velocidad del movimiento
        Vector2 direccion = rb.velocity.normalized;
        rb.velocity = direccion * rb.velocity.magnitude;
    }

    public void setPermission(bool p)
    {
        portalPermission = p;
        portalCooldown = 0;
    }

    public bool getPermission()
    {
        return portalPermission;
    }
}
