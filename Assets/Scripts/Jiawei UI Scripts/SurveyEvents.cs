using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class SurveyEvents : MonoBehaviour
{
    private SheetsService sheetsService;
    [SerializeField]
    private TextAsset googleServiceAccountJson;

    public List<int> responses = new List<int>();

    private UIDocument document;
    private Button button1;
    private Button button2;
    private SliderInt numSlider;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        button1 = document.rootVisualElement.Q("SubmitButton") as Button;
        button1.RegisterCallback<ClickEvent>(OnSubmitClick);

        button2 = document.rootVisualElement.Q("BackButton") as Button;
        button2.RegisterCallback<ClickEvent>(OnBackClick);

        // Initialize Google Sheets API client
        var credential = GoogleCredential.FromJson(googleServiceAccountJson.text)
            .CreateScoped(SheetsService.Scope.Spreadsheets);

        sheetsService = new SheetsService(new SheetsService.Initializer()
        {
            HttpClientInitializer = credential
        });
    }

    private void OnSubmitClick(ClickEvent evt)
    {
        Debug.Log("Submit clicked");

        SubmitForm();
        SceneManager.LoadScene("HomeScene");
    }

    private async void SubmitForm()
    {
        if (ValidateForm())
        {
            string timestamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

            var submissionData = new List<object> { timestamp };
            submissionData.AddRange(responses.ConvertAll(r => (object)r));

            var range = "'Form Responses 1'!A1:K1";

            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { submissionData }
            };

            var request = sheetsService.Spreadsheets.Values.Append(valueRange, "1cWVOLa3uY-O6BhoNYcwZ6--B9Xrb8EjZOxxM1isCYoU", range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await request.ExecuteAsync();
            
            Debug.Log("Survey submitted successfully!");
        }
        else
        {
            Debug.Log("Please answer all questions before submitting.");
        }
    }

    private bool ValidateForm()
    {
        responses.Clear();
        for (int i = 0; i < 10; i++)
        {
            numSlider =  document.rootVisualElement.Q($"Slider{i}") as SliderInt;
            if (numSlider == null || numSlider.value < 1) return false;
            responses.Add(numSlider.value);
        }
        return true;
    }

    private void OnBackClick(ClickEvent evt)
    {
        SceneManager.LoadScene("HomeScene");
    }
}

// Spreadsheed ID: 1cWVOLa3uY-O6BhoNYcwZ6--B9Xrb8EjZOxxM1isCYoU
