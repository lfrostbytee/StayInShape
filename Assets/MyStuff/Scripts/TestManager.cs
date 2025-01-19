using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    public static TestManager instance;
    [SerializeField] private GameObject destinycubePrefab;
    [SerializeField] private TestEvents testEvents;
    [SerializeField] private SurveyEvents surveyEvents;
    DefaultInputActions actions;

    private Renderer targetRenderer;
    private int currLevel = 1;
    private GameObject[] spawnedCubes;
    private GameObject[] spawnedTargets;
    private float result1;
    private float result2;
    private float result3;
    public float distanceBetweenCubes = 2f; // Distance between each cubes for lvl2
    public float timeToMemorise;
    private int[] coloursShown;
    private int cubesDestroyed = 0;
    private string formUrl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfV89LiHuzMxwitY1to1A9uSXEnHriAzssPY1Yi2aA0fsotFw/formResponse";
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            actions = new DefaultInputActions();
            actions.Enable();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        actions.Disable();
    }

    private void Start()
    {

    }

    public int GetCurrLevel()
    {
        return currLevel;
    }

    public void Level1()
    {
        float spawnDistance = 5f;
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;

        // Calculate the spawn position (camera position + forward direction * spawnDistance)
        Vector3 spawnPosition = cameraPosition + cameraForward * spawnDistance;

        // Instantiate the cube at the calculated position
        GameObject spawnedCube = Instantiate(destinycubePrefab, spawnPosition, Quaternion.identity);
        targetRenderer = spawnedCube.GetComponentInChildren<Renderer>();
        StartCoroutine(WaitForPlayerTap());
    }
    public void Level2(float timeMemorise, int[] colours)
    {

        // Initialize the array to hold the references to the cubes
        spawnedCubes = new GameObject[5];

        for (int i = 0; i < 5; i++)
        {
            // Calculate the position for each cube along the camera's forward direction
            Vector3 spawnPosition = Camera.main.transform.position + Camera.main.transform.forward * 5 + Camera.main.transform.right * (distanceBetweenCubes * (i - 2));

            // Instantiate the cube at the calculated position
            spawnedCubes[i] = Instantiate(destinycubePrefab, spawnPosition, Quaternion.identity);

            // Make the cube face the camera
            spawnedCubes[i].transform.LookAt(Camera.main.transform);
        }
        timeToMemorise = timeMemorise;
        coloursShown = colours;
        StartCoroutine(WaitForPlayerRecall());
    }

    public void Level3()
    {
        float distFromcamera = 5f;
        float maxLeft = -1.5f;
        float maxright = 1.5f;
        float maxtop = 1.5f;
        float maxbottom = -1.5f;

        spawnedTargets = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            //generate random position
            Vector3 spawnPosition = Camera.main.transform.position +
                (Camera.main.transform.forward * distFromcamera) +
                (Camera.main.transform.right * Random.Range(maxLeft, maxright)) +
                (Camera.main.transform.up * Random.Range(maxbottom, maxtop));
            // Instantiate the cube at the calculated position
            spawnedTargets[i] = Instantiate(destinycubePrefab, spawnPosition, Quaternion.identity);

            // Make the cube face the camera
            spawnedTargets[i].transform.LookAt(Camera.main.transform);
            spawnedTargets[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        StartCoroutine(WaitForPlayerShoot());
    }

    private IEnumerator WaitForPlayerShoot()
    {
        float startTime = Time.time;
        while(cubesDestroyed != 3)
        {
            // Check if the player clicks on the object
            if (actions.UI.Click.WasPressedThisFrame())
            {
                Vector2 clickPosition = actions.UI.Point.ReadValue<Vector2>();
                Ray ray = Camera.main.ScreenPointToRay(clickPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the player clicked the target object
                    if (hit.collider.gameObject.TryGetComponent<DestinyCube>(out DestinyCube destiny))
                    {
                        destiny.Shot();
                        cubesDestroyed++;
                    }
                }
            }
            // Yield for a frame before checking again
            yield return null;
        }
        float timeFinish = Time.time - startTime;
        result3 = timeFinish;
        Debug.Log("Player destroyed all cubes in " + timeFinish + " seconds.");
        testEvents.DisplayResults(result1, result2, result3);
    }

    private IEnumerator WaitForPlayerTap()
    {
        // Pick a random time between 3 and 8 seconds
        float timeToWait = Random.Range(3f, 8f);

        // Wait for the random duration
        yield return new WaitForSeconds(timeToWait);

        // Change the color of the object to green
        targetRenderer.material.color = Color.green;

        // Start measuring the time from when the object turns green
        float startTime = Time.time;

        // Wait until the player clicks on the object
        bool playerClicked = false;

        while (!playerClicked)
        {
            // Check if the player clicks on the object
            if (actions.UI.Click.WasPressedThisFrame())
            {
                Vector2 clickPosition = actions.UI.Point.ReadValue<Vector2>();
                Ray ray = Camera.main.ScreenPointToRay(clickPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the player clicked the target object
                    if (hit.collider.gameObject.TryGetComponent<DestinyCube>(out DestinyCube destiny))
                    {
                        playerClicked = true;

                        // Calculate how long it took for the player to tap
                        float timeClicked = Time.time - startTime;
                        Destroy(destiny.gameObject);
                        Debug.Log("Player clicked the object after " + timeClicked + " seconds.");
                        result1 = timeClicked;
                        currLevel = 2;
                        testEvents.NextLevel(currLevel);
                    }
                }
            }

            // Yield for a frame before checking again
            yield return null;
        }
    }

    private bool arrayChecker()
    {
        for (int i = 0; i < 5; i++)
        {
            spawnedCubes[i].TryGetComponent<DestinyCube>(out DestinyCube destiny);
            if(destiny.GetColor() == coloursShown[i])
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator WaitForPlayerRecall()
    {

        float startTime = Time.time;

        while (!arrayChecker())
        {
            // Check if the player clicks on the object
            if (actions.UI.Click.WasPressedThisFrame())
            {
                Vector2 clickPosition = actions.UI.Point.ReadValue<Vector2>();
                Ray ray = Camera.main.ScreenPointToRay(clickPosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the player clicked the target object
                    if (hit.collider.gameObject.TryGetComponent<DestinyCube>(out DestinyCube destiny))
                    {
                        destiny.ChangeColor();
                    }
                }
            }

            // Yield for a frame before checking again
            yield return null;
        }
        for(int i = 0;i < 5; i++)
        {
            Destroy(spawnedCubes[i].gameObject);
        }
        float timefinished = Time.time - startTime;
        float totalTimetaken = timefinished + timeToMemorise;
        result2 = totalTimetaken;
        Debug.Log("Player managed to memorise in " + totalTimetaken + " seconds.");
        currLevel = 3;
        testEvents.NextLevel(currLevel);
    }

    public void SubmitResults(string data1, string data2, string data3)
    {
        StartCoroutine(Post(data1, data2, data3));
    }
    private IEnumerator Post(string data1, string data2, string data3)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.1389526925", data1);
        form.AddField("entry.2076729817", data2);
        form.AddField("entry.142012223", data3);

        using (UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Results submitted successfully.");
            }
            else
            {
                Debug.LogError("Error in feedback submission: " + www.error);
            }
        }
    }

}
