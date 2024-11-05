using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BujeroScript : MonoBehaviour
{
    // Start is called before the first frame update
    public string id_Screen;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Ball")
        {
            Vector2 velocity = other.GetComponent<Rigidbody2D>().velocity;
            //Debug.Log("Ha entrat amb una velocitat de: " + velocity);
            float speedThreshold = 1.5f; 
            if (velocity.magnitude < speedThreshold)
            {
                //Debug.Log("La bola se ha eliminado porque superó la velocidad umbral.");
                Destroy(other.gameObject);
            }
            else
            {
                //Debug.Log("La bola no se eliminó porque no superó la velocidad umbral.");
            }
        }
    }
}
