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

    void Start()
    {
        settingsPopup.SetActive(false);
        readyButton.SetActive(true);
        LoadQuestions();
    }

    void LoadQuestions()
    {
        // Load questions here
        questions.Add(new Question("What does CPU stand for?", new string[] { "Central Processing Unit", "Computer Personal Unit", "Central Processor Unit", "Computer Processing Unit" }, 0));
        questions.Add(new Question("What does GPU stand for?", new string[] { "Graphics Processing Unit", "Graphical Processing Unit", "Graphics Processor Unit", "Graphical Processor Unit" }, 0));
    }

    public void OnReadyButtonClicked()
    {
        readyButton.SetActive(false);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        // Countdown logic
        for (int i = 3; i > 0; i--)
        {
            // Display countdown
            Debug.Log(i);
            yield return new WaitForSeconds(1);
        }
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        settingsPopup.SetActive(true);
        Question question = questions[currentQuestionIndex];
        questionText.text = question.Text;
        for (int i = 0; i < answerTexts.Length; i++)
        {
            answerTexts[i].text = question.Answers[i];
            answerTexts[i].color = GetColorForAnswer(i); // Set color based on index
        }
        questionTimer = displayQuestionTime;
        StartCoroutine(HideQuestionAfterDelay());
    }

    IEnumerator HideQuestionAfterDelay()
    {
        while (questionTimer > 0)
        {
            timerText.text = "Time: " + questionTimer.ToString("F1");
            questionTimer -= Time.deltaTime;
            yield return null;
        }
        timerText.text = "";
        settingsPopup.SetActive(false);
        // Logic to start spawning skeets
        sceneController.StartSpawningSkeets();
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
        if (index == questions[currentQuestionIndex].CorrectAnswerIndex)
        {
            score++;
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
