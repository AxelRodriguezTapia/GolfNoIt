using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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
        }else{
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

    public void SaveScore()
    {
        List<(string, int)> ranking = LoadRanking();
        ranking.Add((playerName, playerScore));

        ranking.Sort((x, y) => x.Item2.CompareTo(y.Item2));
        if (ranking.Count > 5) ranking.RemoveAt(5);

        for (int i = 0; i < ranking.Count; i++)
        {
            PlayerPrefs.SetString($"RankingName{i}", ranking[i].Item1);
            PlayerPrefs.SetInt($"RankingScore{i}", ranking[i].Item2);
        }
        PlayerPrefs.Save();
    }

    private List<(string, int)> LoadRanking()
    {
        List<(string, int)> ranking = new List<(string, int)>();
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPrefs.HasKey($"RankingName{i}") && PlayerPrefs.HasKey($"RankingScore{i}"))
            {
                string name = PlayerPrefs.GetString($"RankingName{i}");
                int score = PlayerPrefs.GetInt($"RankingScore{i}");
                ranking.Add((name, score));
            }
        }
        return ranking;
    }

    private void DisplayRanking()
    {
        rankingText = GameObject.FindWithTag("Ranking").GetComponent<TextMeshProUGUI>();
        List<(string, int)> ranking = LoadRanking();
        rankingText.text = "Top 5 Players:\n";
        for (int i = 0; i < ranking.Count; i++)
        {
            rankingText.text += $"{i + 1}. {ranking[i].Item1} - {ranking[i].Item2} hits\n";
        }
    }

    public void EndGame()
    {
        SaveScore();
        playerScore = 0;
        SceneManager.LoadScene("Home");
    }
}
