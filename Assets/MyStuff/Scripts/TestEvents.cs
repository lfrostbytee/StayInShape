using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TestEvents : MonoBehaviour
{

    private UIDocument _document;

    private Button startButton;
    private Button startButton2;
    private Button shootButton;
    private Button backButton;
    private Label introduction;
    private Label box1;
    private Label box2;
    private Label box3;
    private Label box4;
    private Label box5;
    private VisualElement colourContainer;
    private float level2MemoriseStartTime;
    private int[] colours = new int[5];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        startButton = _document.rootVisualElement.Q("startButton") as Button;
        startButton.RegisterCallback<ClickEvent>(OnStartButtonClick);
        introduction = _document.rootVisualElement.Q("introduction") as Label;
        introduction.text = "This level tests your reaction speed. \nTap the cube once it turns green.";
        box1 = _document.rootVisualElement.Q("box1") as Label;
        box2 = _document.rootVisualElement.Q("box2") as Label;
        box3 = _document.rootVisualElement.Q("box3") as Label;
        box4 = _document.rootVisualElement.Q("box4") as Label;
        box5 = _document.rootVisualElement.Q("box5") as Label;
        startButton2 = _document.rootVisualElement.Q("startbutton2") as Button;
        startButton2.RegisterCallback<ClickEvent>(OnStartButton2Click);
        shootButton = _document.rootVisualElement.Q("shootButton") as Button;
        shootButton.RegisterCallback<ClickEvent>(OnShootButton);
        shootButton.style.display = DisplayStyle.None;
        backButton = _document.rootVisualElement.Q("BackButton") as Button;
        backButton.RegisterCallback<ClickEvent>(OnBackButton);
        colourContainer = _document.rootVisualElement.Q("colourContainer") as VisualElement;
        colourContainer.style.display = DisplayStyle.None;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(OnStartButtonClick);
        startButton2.UnregisterCallback<ClickEvent>(OnStartButton2Click);
        shootButton.UnregisterCallback<ClickEvent>(OnShootButton);
    }

    private void OnBackButton(ClickEvent evt)
    {
        SceneManager.LoadScene("SurveyScene");
    }

    private void OnStartButtonClick(ClickEvent evt)
    {
        Debug.Log("Start button clicked");
        startButton.style.display = DisplayStyle.None;
        introduction.style.display = DisplayStyle.None;
        TestManager.instance.Level1();
    }

    private void OnStartButton2Click(ClickEvent evt)
    {
        colourContainer.style.display = DisplayStyle.None;
        introduction.style.display= DisplayStyle.None;
        float endMemorise = Time.time - level2MemoriseStartTime;
        TestManager.instance.Level2(endMemorise, colours);
    }

    private void OnShootButton(ClickEvent evt)
    {
        introduction.style.display = DisplayStyle.None;
        TestManager.instance.Level3();
    }

    public void DisplayResults(float result1, float result2, float result3)
    {
        shootButton.style.display = DisplayStyle.None;
        introduction.text = "\n\nThis is the end of the test, \nthese are your results: \n" + result1 + ", \n" + result2 + ", \n" + result3;
        introduction.style.display = DisplayStyle.Flex;
        TestManager.instance.SubmitResults(result1.ToString(), result2.ToString(), result3.ToString());
    }
    public void NextLevel(int nextLevel)
    {
       if(nextLevel == 2)
        {
            introduction.text = "This level tests your memory. \nA number of colours will be shown. \nThen tap the cubes to match the sequence";
            introduction.style.display = DisplayStyle.Flex;
            colourContainer.style.display = DisplayStyle.Flex;
            level2MemoriseStartTime = Time.time;
            for (int i = 0; i < 5; i++)
            {
                if(i == 0)
                {
                    int colour = Random.Range(1, 4);
                    if (colour == 1)
                    {
                        box1.style.backgroundColor = Color.red;
                    }
                    else if (colour == 2)
                    {
                        box1.style.backgroundColor = Color.green;
                    }
                    else if (colour == 3)
                    {
                        box1.style.backgroundColor = Color.blue;
                    }
                    colours[0] = colour;
                }
                if (i == 1)
                {
                    int colour = Random.Range(1, 3);
                    if (colour == 1)
                    {
                        box2.style.backgroundColor = Color.red;
                    }
                    if (colour == 2)
                    {
                        box2.style.backgroundColor = Color.green;
                    }
                    if (colour == 3)
                    {
                        box2.style.backgroundColor = Color.blue;
                    }
                    colours[1] = colour;
                }
                if (i == 2)
                {
                    int colour = Random.Range(1, 3);
                    if (colour == 1)
                    {
                        box3.style.backgroundColor = Color.red;
                    }
                    if (colour == 2)
                    {
                        box3.style.backgroundColor = Color.green;
                    }
                    if (colour == 3)
                    {
                        box3.style.backgroundColor = Color.blue;
                    }
                    colours[2] = colour;
                }
                if (i == 3)
                {
                    int colour = Random.Range(1, 3);
                    if (colour == 1)
                    {
                        box4.style.backgroundColor = Color.red;
                    }
                    if (colour == 2)
                    {
                        box4.style.backgroundColor = Color.green;
                    }
                    if (colour == 3)
                    {
                        box4.style.backgroundColor = Color.blue;
                    }
                    colours[3] = colour;
                }
                if (i == 4)
                {
                    int colour = Random.Range(1, 3);
                    if (colour == 1)
                    {
                        box5.style.backgroundColor = Color.red;
                    }
                    if (colour == 2)
                    {
                        box5.style.backgroundColor = Color.green;
                    }
                    if (colour == 3)
                    {
                        box5.style.backgroundColor = Color.blue;
                    }
                    colours[4] = colour;
                }
            }

        }
       else if(nextLevel == 3)
        {
            introduction.text = "This level tests your motor skills. \nFind and tap to destroy the 3 cubes!\nWhen you are ready, press the Fire! button";
            introduction.style.display = DisplayStyle.Flex;
            shootButton.style.display = DisplayStyle.Flex;

        }
    }
}
