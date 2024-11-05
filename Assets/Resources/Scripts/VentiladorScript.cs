using UnityEngine;

public class VentiladorScript : MonoBehaviour
{
    // Parámetros configurables
    public Vector2 direccion = Vector2.right;  // Dirección de la fuerza del ventilador
    public float fuerza = 5f;                  // Fuerza que aplicará el ventilador

    private void OnTriggerStay2D(Collider2D other)
    {
        // Verifica si el objeto que entra es la pelota
        if (other.gameObject.name == "Ball")
        {
            // Obtiene el Rigidbody2D de la pelota
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Aplica una fuerza continua en la dirección especificada
                rb.AddForce(direccion.normalized * fuerza);
            }
        }
    }
}
