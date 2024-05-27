using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject[] skeetPrefabs; // Array of different colored skeet prefabs
    [SerializeField] private TMP_Text timerText; // Timer text for countdown

    private List<GameObject> skeets = new List<GameObject>(); // List to hold the spawned skeet objects
    private int[] skeetCounts; // Array to track the number of each color skeet in play
    private int activeSkeets; // Counter to track the number of active skeets
    private bool gameStarted = false;
    private TriviaManager triviaManager;
    private bool isSpawningSkeets = false; // Flag to prevent multiple launches

    public float spawnDelay = 2.0f; // Control the delay between skeet destruction and next spawn
    private float launchDelay = 0.5f; // Delay before launching skeets (applying force)

    // Adjusted initial force values
    private Vector3 initialForce = new Vector3(5, 12, 0); // Adjust the values to control speed and altitude

    // Y positions for different colors (Subject to change as needed)
    private float[] yPos = { 8.0f, 10.0f, 12.0f, 14.0f };

    void Start()
    {
        skeetCounts = new int[skeetPrefabs.Length];
        Debug.Log("SceneController started.");
        triviaManager = FindObjectOfType<TriviaManager>();
        if (triviaManager == null)
        {
            Debug.LogError("TriviaManager not found. Ensure there is a TriviaManager component in the scene.");
        }
    }

    public void StartSpawningSkeets()
    {
        Debug.Log("StartSpawningSkeets function called at: " + Time.time);
        StopAllCoroutines(); // Ensure any previous coroutines are stopped
        StartCoroutine(SpawnAndLaunchSkeetsContinuously());
    }

    public void StopSpawningSkeets()
    {
        Debug.Log("StopSpawningSkeets called.");
        StopAllCoroutines(); // Stop all coroutines related to spawning
    }

    IEnumerator SpawnAndLaunchSkeetsContinuously()
    {
        Debug.Log("SpawnAndLaunchSkeetsContinuously started.");
        while (true)
        {
            if (!isSpawningSkeets)
            {
                yield return StartCoroutine(SpawnAndLaunchSkeets());
                yield return new WaitForSeconds(spawnDelay); // Wait before respawning
            }
            yield return null; // Wait for the next frame before checking again
        }
    }

    IEnumerator SpawnAndLaunchSkeets()
    {
        isSpawningSkeets = true;

        // Clear any existing skeets
        foreach (GameObject skeet in skeets)
        {
            if (skeet != null)
            {
                Destroy(skeet);
            }
        }
        skeets.Clear();
        System.Array.Clear(skeetCounts, 0, skeetCounts.Length); // Reset skeet counts

        // Instantiate 5 skeets of each color at fixed positions
        for (int i = 0; i < skeetPrefabs.Length; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                SpawnSkeet(i, j);
            }
        }

        yield return new WaitForSeconds(launchDelay); // Wait before launching

        // Apply initial force to simulate throw for all skeets
        LaunchAllSkeets();

        // Start the countdown timer for skeets
        yield return StartCoroutine(SkeetTimer());

        isSpawningSkeets = false;
    }

    void SpawnSkeet(int colorIndex, int offset)
    {
        GameObject skeet = Instantiate(skeetPrefabs[colorIndex]);
        skeet.transform.position = new Vector3(6, yPos[colorIndex], 6 + offset * 2); // Adjust z-position to avoid overlap
        skeet.GetComponent<Renderer>().material.color = GetColorForSkeet(colorIndex); // Assign color to skeet

        Rigidbody rb = skeet.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = skeet.AddComponent<Rigidbody>(); // Add Rigidbody component
        }

        SkeetPhysics skeetPhysics = skeet.GetComponent<SkeetPhysics>();
        if (skeetPhysics == null)
        {
            skeetPhysics = skeet.AddComponent<SkeetPhysics>(); // Add SkeetPhysics component 
        }

        skeetPhysics.ResetVelocity(); // Reset velocity

        skeet.AddComponent<SkeetColor>().colorIndex = colorIndex; // Add SkeetColor component and set color index
        skeets.Add(skeet);
        skeetCounts[colorIndex]++;
        activeSkeets++;
        Debug.Log($"Skeet of color index {colorIndex} spawned at position: {skeet.transform.position}. Count: {skeetCounts[colorIndex]}");
    }

    void LaunchAllSkeets()
    {
        Debug.Log("Launching all skeets at: " + Time.time);

        foreach (GameObject skeet in skeets)
        {
            Rigidbody rb = skeet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Reset all relevant properties of the Rigidbody
                rb.velocity = Vector3.zero; // Ensure starting from zero velocity
                rb.angularVelocity = Vector3.zero; // Ensure no initial rotation
                rb.drag = 0; // Reset drag
                rb.angularDrag = 0; // Reset angular drag
                rb.useGravity = true; // Ensure gravity is applied

                Debug.Log("Before launching - Position: " + skeet.transform.position + ", Velocity: " + rb.velocity + ", Force: " + initialForce);

                // apply force
                rb.AddForce(initialForce, ForceMode.Impulse);

                Debug.Log("After launching - Position: " + skeet.transform.position + ", Velocity: " + rb.velocity);
            }
            else
            {
                Debug.LogWarning("No Rigidbody attached to the skeet: " + skeet.name);
            }
        }
    }

    IEnumerator SkeetTimer()
    {
        float timer = 5.0f;
        while (timer > 0)
        {
            if (timerText != null)
            {
                timerText.text = "Time: " + timer.ToString("F1");
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        if (timerText != null)
        {
            timerText.text = "Time: 0.0";
        }

        // Destroy all active skeets
        foreach (GameObject skeet in skeets)
        {
            if (skeet != null)
            {
                Destroy(skeet);
            }
        }

        skeets.Clear();
        activeSkeets = 0;
        System.Array.Clear(skeetCounts, 0, skeetCounts.Length);
    }

    public void OnSkeetDestroyed(GameObject skeet)
    {
        Debug.Log("OnSkeetDestroyed called.");
        if (triviaManager != null)
        {
            // Check if the correct skeet was hit and increment score
            if (skeet.GetComponent<Renderer>().material.color == triviaManager.GetColorForAnswer(triviaManager.CurrentQuestionCorrectAnswerIndex))
            {
                triviaManager.IncrementScore();
                Debug.Log("Correct skeet hit. Incrementing score.");
            }
            else
            {
                triviaManager.DecrementScore();
                Debug.Log("Wrong skeet hit. Decrementing score.");
            }
        }

        HandleSkeetDestruction(skeet);
    }

    public void OnSkeetHitGround(GameObject skeet)
    {
        HandleSkeetDestruction(skeet);
    }

    void HandleSkeetDestruction(GameObject skeet)
    {
        int index = skeets.IndexOf(skeet);
        if (index >= 0)
        {
            Destroy(skeet);
            skeets.RemoveAt(index);
            activeSkeets--;
        }
    }

    Color GetColorForSkeet(int index)
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

    public void ResetSkeets()
    {
        // Destroy all active skeets
        foreach (GameObject skeet in skeets)
        {
            if (skeet != null)
            {
                Destroy(skeet);
            }
        }
        skeets.Clear();
        activeSkeets = 0;
        System.Array.Clear(skeetCounts, 0, skeetCounts.Length);
        Debug.Log("Skeets have been reset.");
    }
}
