using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class TriviaManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text[] answerTexts; // Array to hold answer text elements
    [SerializeField] private GameObject readyButton;
    [SerializeField] private TMP_Text timerText; // Timer text for countdown
    [SerializeField] private TMP_Text scoreText; // For keeping track of score
    [SerializeField] private float displayQuestionTime = 5.0f;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private TMP_Text strikeCounter; // Text object for strikes

    private int currentQuestionIndex = 0;
    private List<Question> questions = new List<Question>(); // List of questions
    private int score = 0;
    private int failedQuestions = 0;
    private const int maxFailedQuestions = 3;
    private float questionTimer; // Timer for current question
    private bool isCoroutineRunning = false; // Tracking if the coroutine is already running
    private bool gameStarted = false;
    private bool gameEnded = false; // New flag to track if the game has ended


    void Start()
    {
        settingsPopup.SetActive(false);
        readyButton.SetActive(true);
        LoadQuestions();
        UpdateStrikeCounter(); // Initialize strike counter text
        Debug.Log("TriviaManager started.");
    }

    void LoadQuestions()
    {
        // Load questions here
        questions.Add(new Question("What is the time complexity of a For loop? ", new string[] { "O(N)", "Log(N)", "O(N^2)", "O(2N)"}, 0));
        questions.Add(new Question("What does CPU stand for?", new string[] { "Central Processing Unit", "Computer Personal Unit", "Central Processor Unit", "Computer Processing Unit" }, 0));
        questions.Add(new Question("What does GPU stand for?", new string[] { "Graphics Processing Unit", "Graphical Processing Unit", "Graphics Processor Unit", "Graphical Processing Unit" }, 0));

        Debug.Log("Questions loaded.");
    }

    public void StartGame()
    {
        Debug.Log("StartGame called.");
        if (!gameStarted)
        {
            gameStarted = true;
            ResetGame();
            readyButton.SetActive(false);
            DisplayQuestion();
            sceneController.StartSpawningSkeets();
        }
    }

    public void OnReadyButtonClicked()
    {
        StartGame();
    }

    void DisplayQuestion()
    {
        Debug.Log("DisplayQuestion called with index: " + currentQuestionIndex);

        // Set up the question and answers
        Question question = questions[currentQuestionIndex];
        questionText.text = question.Text;

        for (int i = 0; i < answerTexts.Length; i++)
        {
            answerTexts[i].text = question.Answers[i];
            answerTexts[i].color = GetColorForAnswer(i); // Set color based on index
        }

        questionTimer = displayQuestionTime;

        // Show the popup with the question and answers
        settingsPopup.SetActive(true);

        if (!isCoroutineRunning)
        {
            Debug.Log("Starting HideQuestionAfterDelay coroutine.");
            StartCoroutine(HideQuestionAfterDelay());
        }
        else
        {
            Debug.LogWarning("HideQuestionAfterDelay coroutine already running.");
        }
    }

    IEnumerator HideQuestionAfterDelay()
    {
        isCoroutineRunning = true;
        Debug.Log("HideQuestionAfterDelay coroutine started.");
        while (questionTimer > 0)
        {
            timerText.text = "Time: " + questionTimer.ToString("F1");
            questionTimer -= Time.deltaTime;
            yield return null;
        }
        timerText.text = "";
        settingsPopup.SetActive(false);
        Debug.Log("HideQuestionAfterDelay coroutine ended. Timer reached 0.");
        isCoroutineRunning = false;

        // Increment the question index and display the next question
        currentQuestionIndex = (currentQuestionIndex + 1) % questions.Count;
        Debug.Log("Incremented question index to: " + currentQuestionIndex);
    }

    public Color GetColorForAnswer(int index)
    {
        switch (index)
        {
            case 0: return Color.red;
            case 1: return Color.blue;
            case 2: return Color.green;
            case 3: return Color.yellow;
            default: return Color.white;
        }
    }

    public void OnAnswerSelected(int index)
    {
        Debug.Log("OnAnswerSelected called with index: " + index);
        if (index == questions[currentQuestionIndex].CorrectAnswerIndex)
        {
            Debug.Log("Correct!");
            HighlightAnswer(index, Color.green);
            IncrementScore();
        }
        else
        {
            Debug.Log("Wrong!");
            HighlightAnswer(index, Color.red);
            DecrementScore();
        }

        // Increment the question index and display the next question
        currentQuestionIndex = (currentQuestionIndex + 1) % questions.Count;
        Debug.Log("Incremented question index to: " + currentQuestionIndex);
        DisplayQuestion();
    }

    void HighlightAnswer(int index, Color color)
    {
        answerTexts[index].color = color;
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateStrikeCounter()
    {
        Debug.Log("UpdateStrikeCounter called. Strikes: " + failedQuestions); // Add this line to verify method call
        strikeCounter.text = "Strikes: " + failedQuestions + "/" + maxFailedQuestions;
    }

    public void IncrementScore()
    {
        Debug.Log("IncrementScore called.");
        score++;
        UpdateScore();
    }

    public void DecrementScore()
    {
        Debug.Log("DecrementScore called. Current Strikes: " + failedQuestions); // Add this line to check if method is called
        score--;
        failedQuestions++;
        UpdateScore();
        UpdateStrikeCounter();
        if (failedQuestions >= maxFailedQuestions)
        {
            Debug.Log("Max failed questions reached. Calling EndGame.");
            EndGame();
        }
    }

    public int CurrentQuestionCorrectAnswerIndex
    {
        get { return questions[currentQuestionIndex].CorrectAnswerIndex; }
    }

void EndGame()
{
    Debug.Log("EndGame called. Final Score: " + score + ", Strikes: " + failedQuestions);

    // Reset all necessary variables and states
    gameStarted = false;
    isCoroutineRunning = false;
    StopAllCoroutines();
    ResetGame();

    // Start the overlay effect
    FindObjectOfType<FadeEffect>().StartOverlay();

    // Wait for 5 seconds before reloading the scene
    StartCoroutine(ReloadSceneAfterDelay(5.0f));
    }

    IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reload the scene (assuming the scene name is "GameScene")
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

void ResetGame()
{
    Debug.Log("ResetGame called.");
    score = 0;
    failedQuestions = 0;
    currentQuestionIndex = 0;
    UpdateScore();
    UpdateStrikeCounter();
    ClearQuestionAndAnswers();
    sceneController.StopSpawningSkeets();
    sceneController.ResetSkeets();
    settingsPopup.SetActive(true);
    readyButton.SetActive(true);
    Debug.Log("Game has been reset.");
}


    void ResetUI()
    {
        settingsPopup.SetActive(true);
        readyButton.SetActive(true);
    }

    void ClearQuestionAndAnswers()
    {
        questionText.text = "";
        foreach (TMP_Text answerText in answerTexts)
        {
            answerText.text = "";
            answerText.color = Color.white;
        }
    }
}

[System.Serializable]
public class Question
{
    public string Text;
    public string[] Answers;
    public int CorrectAnswerIndex;

    public Question(string text, string[] answers, int correctAnswerIndex)
    {
        Text = text;
        Answers = answers;
        CorrectAnswerIndex = correctAnswerIndex;
    }
}
