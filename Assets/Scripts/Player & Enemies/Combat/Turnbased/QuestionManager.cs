using UnityEngine;

[System.Serializable]
public class Question
{
    public string questionText;        // The question text
    public string[] answers;           // Array of answer options
    public int correctAnswerIndex;     // Index of the correct answer
}

public class QuestionManager : MonoBehaviour
{
    public Question[] questions;       // Array of questions

    private int currentQuestionIndex = 0;

    public Question GetCurrentQuestion()
    {

        return questions[currentQuestionIndex];
    }

    public void NextQuestion()
    {
        currentQuestionIndex = (currentQuestionIndex + 1) % questions.Length;
    }
}
