using UnityEngine;
using TMPro;

public class CSharpQuizPanel : MonoBehaviour
{
    [SerializeField] private GameObject quizPanel; // Panel containing the UI
    [SerializeField] private TMP_Text questionText; // Question text
    [SerializeField] private TMP_InputField answerInput; // InputField for player's answer
    [SerializeField] private UnityEngine.UI.Button submitButton; // Button to submit the answer
    [SerializeField] private SojaExiles.opencloseDoor targetDoor; // Reference to the door script

    private bool isAnswered = false; // Tracks if the question has been answered

    private readonly (string question, string answer)[] questions = new (string, string)[]
    {
        ("How do you declare an array of integers in C#?", "int[]"),
        ("What is the default value of an element in a bool array?", "false"),
        ("Which property is used to get the number of elements in an array?", "Length"),
        ("How do you access the first element of an array named 'myArray'?", "myArray[0]"),
        ("What is the rank of an array in C#?", "The number of dimensions"),
        ("What keyword is used to initialize an array in C#?", "new"),
        ("What exception is thrown when accessing an index out of bounds in an array?", "IndexOutOfRangeException"),
        ("Can arrays in C# store elements of different types? (yes/no)", "no"),
        ("How do you create a two-dimensional array of integers in C#?", "int[,]"),
        ("What method is used to sort an array in C#?", "Array.Sort")
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
