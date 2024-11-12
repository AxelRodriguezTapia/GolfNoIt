using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  
    public List<Vector3> cameraPositions; // Lista pública de posiciones para la cámara

    // Variables de ejemplo
    public static int playerScore;
    private GameObject ball;
    private Camera mainCamera;
    public TextMeshProUGUI scoreText;  // Arrastra el TextMeshPro desde el Canvas aquí en el Inspector

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);  
        }
    }

    private void Start()
    {
        // Referencia al objeto ball y la cámara principal
        ball = GameObject.FindWithTag("Ball");
        scoreText = GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>();
        mainCamera = Camera.main;
        // Inicializa el texto del puntaje en cero
        UpdateScoreText();
    }

    private void Update()
    {

        if(SceneManager.GetActiveScene().name == "Home"){
            
        }

        if(CheckCurrentScene()){
            if (ball != null && !IsObjectVisible(ball))
            {
                RepositionCameraToViewBall();
            }
        }
        // Check if the ball is visible and reposition the camera if needed
        

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("Level1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("Level2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("Level3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("Level4");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("Level5");
        }
    }

    private bool IsObjectVisible(GameObject obj)
    {
        // Convertir la posición de la bola al espacio de pantalla
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
        // Comprobar si el objeto está dentro de los límites de la cámara
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0;
    }

    private void RepositionCameraToViewBall()
    {
        // Encuentra la posición más cercana en cameraPositions que pueda ver la bola
        foreach (Vector3 position in cameraPositions)
        {
            mainCamera.transform.position = position;

            if (IsObjectVisible(ball))
            {
                // Si el objeto es visible desde esta posición, detenemos el bucle
                break;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;  
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  
    }

    private bool CheckCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "Level5";
    }

    public void AddScore(int score)
    {
        playerScore += score;
        UpdateScoreText();
    }
    
    public int getScore()
    {
        return playerScore;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Redefinir la referencia a ball cuando se cargue una nueva escena
        ball = GameObject.FindWithTag("Ball");
        scoreText = GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>();
        mainCamera = Camera.main;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
        scoreText.text = "Hits: " + playerScore.ToString();
        }
    }
}
