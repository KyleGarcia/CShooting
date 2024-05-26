using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private int currentQuestionIndex = 0;
    private List<Question> questions = new List<Question>(); // List of questions
    private int score = 0;
    private int failedQuestions = 0;
    private const int maxFailedQuestions = 3;
    private float questionTimer; // Timer for current question
    private bool isCoroutineRunning = false; // Tracking if the coroutine is already running
    private bool gameStarted = false;

    void Start()
    {
        settingsPopup.SetActive(false);
        readyButton.SetActive(true);
        LoadQuestions();
        Debug.Log("TriviaManager started.");
    }

    void LoadQuestions()
    {
        // Load questions here
        questions.Add(new Question("What does CPU stand for?", new string[] { "Central Processing Unit", "Computer Personal Unit", "Central Processor Unit", "Computer Processing Unit" }, 0));
        questions.Add(new Question("What does GPU stand for?", new string[] { "Graphics Processing Unit", "Graphical Processing Unit", "Graphics Processor Unit", "Graphical Processing Unit" }, 0));
        Debug.Log("Questions loaded.");
    }

    public void OnReadyButtonClicked()
    {
        if (!gameStarted)
        {
            readyButton.SetActive(false);
            gameStarted = true;
            DisplayQuestion();
            sceneController.StartSpawningSkeets(); // Start spawning skeets when the game starts
        }
    }

    void DisplayQuestion()
    {
        Debug.Log("DisplayQuestion called.");

        // Set up the question and answers
        Question question = questions[currentQuestionIndex];
        questionText.text = question.Text;

        for (int i = 0; i < answerTexts.Length; i++)
        {
            answerTexts[i].text = question.Answers[i];
            answerTexts[i].color = GetColorForAnswer(i); // Set color based on index
        }

        questionTimer = displayQuestionTime;
        Debug.Log("Question timer set to: " + questionTimer);

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
            Debug.Log("Question Timer: " + questionTimer);
            questionTimer -= Time.deltaTime;
            yield return null;
        }
        timerText.text = "";
        settingsPopup.SetActive(false);
        Debug.Log("HideQuestionAfterDelay coroutine ended. Timer reached 0.");
        isCoroutineRunning = false;
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
            
        }
        else
        {
            failedQuestions++;
            Debug.Log("Wrong!");
            HighlightAnswer(index, Color.red);
            if (failedQuestions >= maxFailedQuestions)
            {
                EndGame();
                return;
            }
        }
        currentQuestionIndex = (currentQuestionIndex + 1) % questions.Count;
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

    public void IncrementScore()
    {
        Debug.Log("IncrementScore called.");
        score++;
        UpdateScore();
    }

    public int CurrentQuestionCorrectAnswerIndex
    {
        get { return questions[currentQuestionIndex].CorrectAnswerIndex; }
    }

    void EndGame()
    {
        Debug.Log("Game Over! Your score: " + score);
        // Display final score and reset game
        gameStarted = false; // Reset game started flag
        isCoroutineRunning = false; // Reset coroutine flag
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
