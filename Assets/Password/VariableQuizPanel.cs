using UnityEngine;
using TMPro;

public class VariableQuizPanel : MonoBehaviour
{
    [SerializeField] private GameObject quizPanel; // Panel containing the UI
    [SerializeField] private TMP_Text questionText; // Question text
    [SerializeField] private TMP_InputField answerInput; // InputField for player's answer
    [SerializeField] private UnityEngine.UI.Button submitButton; // Button to submit the answer
    [SerializeField] private SojaExiles.opencloseDoor targetDoor; // Reference to the door script

    private bool isAnswered = false; // Tracks if the question has been answered

    private readonly (string question, string answer)[] questions = new (string, string)[]
    {
        ("What is the default value of an int in C#?", "0"),
        ("What data type is used to store true or false values?", "bool"),
        ("What data type is used for storing single characters?", "char"),
        ("Which data type is used to store decimal values?", "float"),
        ("What is the keyword for declaring a constant variable in C#?", "const"),
        ("Which data type is used for storing large integer values?", "long"),
        ("What is the default value of a string in C#?", "null"),
        ("What data type is used for very precise decimal numbers?", "double"),
        ("Which data type in C# is used for storing a sequence of characters?", "string"),
        ("What is the size (in bytes) of a char in C#?", "2")
    };

    private (string question, string answer) currentQuestion;

    private CanvasGroup panelCanvasGroup;

    private void Start()
    {
        if (quizPanel == null || questionText == null || answerInput == null || submitButton == null || targetDoor == null)
        {
            Debug.LogError("One or more references are not assigned in the inspector.");
            return;
        }

        panelCanvasGroup = quizPanel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = quizPanel.AddComponent<CanvasGroup>();
        }

        quizPanel.SetActive(false);
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        submitButton.onClick.AddListener(CheckAnswer);

        submitButton.interactable = false;
    }

    private void OnMouseDown()
    {
        if (!isAnswered && !panelCanvasGroup.blocksRaycasts)
        {
            ShowQuestion();
        }
    }

    private void ShowQuestion()
    {
        if (questions.Length == 0)
        {
            Debug.LogError("No questions available!");
            return;
        }

        currentQuestion = questions[Random.Range(0, questions.Length)];
        questionText.text = currentQuestion.question;
        quizPanel.SetActive(true);
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;
        submitButton.interactable = true;
    }

    private void CheckAnswer()
    {
        if (string.IsNullOrEmpty(currentQuestion.question))
        {
            Debug.LogError("No question is currently being asked!");
            return;
        }

        string playerAnswer = answerInput.text.Trim().ToLower();
        if (playerAnswer == currentQuestion.answer.ToLower())
        {
            Debug.Log("Correct Answer!");
            isAnswered = true;
            targetDoor.UnlockDoor(); // Unlock the door
            HidePanel();
        }
        else
        {
            Debug.Log("Wrong Answer. Try again!");
        }

        answerInput.text = "";
    }

    private void HidePanel()
    {
        quizPanel.SetActive(false);
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;
        submitButton.interactable = false;
    }
}
