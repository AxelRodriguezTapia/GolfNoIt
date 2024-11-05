using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Asegúrate de importar el espacio de nombres para UI

public class BallScript : MonoBehaviour
{
    private float force;
    public int seconds = 1; // Asegúrate de que el valor por defecto no sea cero.
    public float speed = 1;
    public Slider slider;
    private LineRenderer lineRenderer; // Línea para conectar la pelota con el ratón
    public float maxLineLength = 2.0f; // Longitud máxima de la línea

    // Start is called before the first frame update
    void Start()
    {
        slider.gameObject.SetActive(false);
        lineRenderer = gameObject.AddComponent<LineRenderer>(); // Agregar el LineRenderer
        lineRenderer.positionCount = 2; // Dos puntos: la pelota y el cursor
        lineRenderer.startWidth = 0.05f; // Ancho inicial de la línea
        lineRenderer.endWidth = 0.05f; // Ancho final de la línea
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Material para la línea
        lineRenderer.startColor = Color.blue; // Color inicial de la línea
        lineRenderer.endColor = Color.cyan; // Color final de la línea
        lineRenderer.sortingOrder = 1; // You can change this value as needed
    }

    // Update is called once por frame
    void Update()
    {
        // Obtener la posición del ratón en la pantalla
        Vector3 mousePosition = Input.mousePosition;
        // Convertir la posición del ratón a coordenadas del mundo
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector2(mousePosition.x, mousePosition.y));
        mousePosition.z = 0; // Asegúrate de que z sea 0 para el 2D

        // Calcular la dirección desde el objeto hacia el ratón
        Vector2 direction = (mousePosition - transform.position).normalized;
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;

        // Debug.Log(velocity.magnitude);
        if (velocity.magnitude <= 0.02)
        {
            velocity = new Vector2(0, 0);
            lineRenderer.enabled=true;
        }else{
            lineRenderer.enabled=false;
        }

        if (velocity.magnitude <= 0.02)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                force = 0; // Reinicia la fuerza al presionar la tecla
                slider.gameObject.SetActive(true);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if (seconds == 0) { seconds = 1; } // Asegúrate de que seconds no sea cero
                force += Time.deltaTime / seconds; // Aumenta la fuerza
                if (force >= 1) { force = 1; } // Limita la fuerza a 1
                //Debug.Log(force);
                slider.value = force;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                // Aplica la fuerza en la dirección calculada
                GetComponent<Rigidbody2D>().AddForce(direction * (force * speed), ForceMode2D.Impulse);
                Invoke("resetForce", 2);
            }
        }

        // Actualiza la posición de la línea
        lineRenderer.SetPosition(0, transform.position); // Posición de la pelota

        // Calcula la distancia entre la pelota y el cursor
        float distance = Vector2.Distance(transform.position, mousePosition);
        
        // Ajusta la posición del cursor si la distancia supera maxLineLength
        if (distance > maxLineLength)
        {
            // Calcula un punto en la dirección del cursor, pero limitado a maxLineLength
            Vector2 adjustedPosition = (Vector2)transform.position + direction * maxLineLength;
            lineRenderer.SetPosition(1, adjustedPosition); // Ajustar la posición de la línea
        }
        else
        {
            lineRenderer.SetPosition(1, mousePosition); // Posición del ratón
        }
    }

    private void resetForce()
    {
        force = 0;
        slider.value = 0; // Reinicia la fuerza después de aplicarla
        slider.gameObject.SetActive(false);
    }
}
