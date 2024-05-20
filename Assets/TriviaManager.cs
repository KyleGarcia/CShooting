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
    [SerializeField] private float displayQuestionTime = 5.0f;
    
    private int currentQuestionIndex = 0;
    private List<Question> questions = new List<Question>(); // List of questions
    private int score = 0;
    private int failedQuestions = 0;
    private const int maxFailedQuestions = 3;

    void Start()
    {
        settingsPopup.SetActive(false);
        readyButton.SetActive(true);
        LoadQuestions();
    }

    void LoadQuestions()
    {
        // Load your questions here
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
        StartCoroutine(HideQuestionAfterDelay());
    }

    IEnumerator HideQuestionAfterDelay()
    {
        yield return new WaitForSeconds(displayQuestionTime);
        settingsPopup.SetActive(false);
        // Logic to start spawning skeets
    }

    Color GetColorForAnswer(int index)
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
        }
        else
        {
            failedQuestions++;
            Debug.Log("Wrong!");
            if (failedQuestions >= maxFailedQuestions)
            {
                EndGame();
                return;
            }
        }
        currentQuestionIndex = (currentQuestionIndex + 1) % questions.Count;
        DisplayQuestion();
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
