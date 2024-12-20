using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BujeroScript : MonoBehaviour
{
    // Start is called before the first frame update
    public string id_Screen;
    private GameManager gm;
    
    void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
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
                if(id_Screen=="Home"){
                    gm.EndGame();
                }else{
                    //Debug.Log("La bola se ha eliminado porque superó la velocidad umbral.");
                    Destroy(other.gameObject);
                    SceneManager.LoadScene(id_Screen);
                }
                
            }
        }
    }
}
