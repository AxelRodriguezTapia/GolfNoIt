using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Vector3> cameraPositions;

    public static int playerScore;
    private GameObject ball;
    private Camera mainCamera;
    private TextMeshProUGUI scoreText;

    public TMP_InputField nameInputField;
    public Button submitButton;
    public TextMeshProUGUI rankingText;

    private string playerName;
    private bool isFirebaseInitialized = false; // Bandera para saber si Firebase se inicializó correctamente

    // Firebase variables
    private static DatabaseReference dbReference;

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
        // Inicializar Firebase
        iniciarFirebase();

        if (SceneManager.GetActiveScene().name == "Home")
        {
            DisplayRanking();
            submitButton = GameObject.FindWithTag("StartButton").GetComponent<Button>();
            nameInputField = GameObject.FindWithTag("EnterNameSpace").GetComponent<TMP_InputField>();
        }
        else
        {
            ball = GameObject.FindWithTag("Ball");
            scoreText = GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>();
            mainCamera = Camera.main;
            UpdateScoreText();
        }
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    private void Update()
    {
        if (CheckCurrentScene())
        {
            if (ball != null && !IsObjectVisible(ball))
            {
                RepositionCameraToViewBall();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SceneManager.LoadScene("Level1");
        if (Input.GetKeyDown(KeyCode.Alpha2)) SceneManager.LoadScene("Level2");
        if (Input.GetKeyDown(KeyCode.Alpha3)) SceneManager.LoadScene("Level3");
        if (Input.GetKeyDown(KeyCode.Alpha4)) SceneManager.LoadScene("Level4");
        if (Input.GetKeyDown(KeyCode.Alpha5)) SceneManager.LoadScene("Level5");
        if(SceneManager.GetActiveScene().name=="Home"){
            DisplayRanking();
        }
    }

    private bool IsObjectVisible(GameObject obj)
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0;
    }

    private void RepositionCameraToViewBall()
    {
        foreach (Vector3 position in cameraPositions)
        {
            mainCamera.transform.position = position;
            if (IsObjectVisible(ball)) break;
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
        return sceneName.StartsWith("Level");
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
        if (scene.name != "Home")
        {
            ball = GameObject.FindWithTag("Ball");
            scoreText = GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>();
            mainCamera = Camera.main;
            UpdateScoreText();
        } else {
            DisplayRanking();
            submitButton = GameObject.FindWithTag("StartButton").GetComponent<Button>();
            nameInputField = GameObject.FindWithTag("EnterNameSpace").GetComponent<TMP_InputField>();
        }
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Hits: " + playerScore.ToString();
        }
    }

    private void OnSubmitButtonClicked()
    {
        if (nameInputField != null)
        {
            playerName = nameInputField.text;
            if (!string.IsNullOrEmpty(playerName))
            {
                playerScore = 0;
                SceneManager.LoadScene("Level1");
            }
        }
    }

   private void iniciarFirebase()
{
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
    {
        if (task.Result == Firebase.DependencyStatus.Available)
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            FirebaseDatabase firebaseDatabase = FirebaseDatabase.GetInstance(app);
            firebaseDatabase.SetPersistenceEnabled(true);  // Habilita la persistencia si es necesario
            dbReference = firebaseDatabase.RootReference;
            isFirebaseInitialized = true;  // Establecer que Firebase se ha inicializado correctamente
            Debug.Log("Firebase inicializado correctamente y base de datos configurada.");
        }
        else
        {
            Debug.LogError("Firebase no está disponible: " + task.Result.ToString());
        }
    });
}


    // Método para guardar el puntaje en Firebase
    public void SaveScore()
    {
        if (!string.IsNullOrEmpty(playerName))
        {
            // Guardar puntaje en Firebase
            dbReference.Child("ranking").Child(playerName).SetValueAsync(playerScore).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Puntaje guardado exitosamente en Firebase.");
                }
                else
                {
                    Debug.LogError("Error al guardar el puntaje: " + task.Exception);
                }
            });
        }
    }

    // Método para cargar el ranking desde Firebase
   private void DisplayRanking()
{
    if (!isFirebaseInitialized)
    {
        Debug.LogError("Firebase no está inicializado correctamente.");
        return;
    }

    if (dbReference == null)
    {
        Debug.LogError("La referencia de la base de datos no está inicializada.");
        return;
    }

    rankingText = GameObject.FindWithTag("Ranking")?.GetComponent<TextMeshProUGUI>();

    if (rankingText != null)
    {
        rankingText.text = "Top 5 Players:\n";

        dbReference.Child("ranking").OrderByValue().LimitToFirst(5).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                List<string> rankingEntries = new List<string>();
                foreach (var entry in task.Result.Children)
                {
                    string name = entry.Key;
                    int score = int.Parse(entry.Value.ToString());
                    rankingEntries.Add($"{name} - {score} hits"); // Agregar al final para mantener el orden ascendente
                }

                // Mostrar los jugadores ordenados de menor a mayor puntaje
                rankingText.text = "Top 5 Players:\n" + string.Join("\n", rankingEntries);
            }
            else
            {
                Debug.LogError("Error al cargar el ranking o no existen datos: " + task.Exception);
            }
        });
    }
    else
    {
        Debug.LogError("No se encontró el objeto con el tag 'Ranking' o el componente TextMeshProUGUI.");
    }
}



    public void EndGame()
    {
        SaveScore();
        playerScore = 0;
        SceneManager.LoadScene("Home");
    }
}
