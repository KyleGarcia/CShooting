using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject[] skeetPrefabs; // Array of different colored skeet prefabs
    private GameObject[] skeets; // Array to hold the spawned skeet objects

    public float spawnDelay = 2.0f; // Control the delay between skeet destruction and next spawn
    private float launchDelay = 0.5f; // Delay before launching skeets(applying force)
    private bool skeetHit = false; // Track if any skeet has been hit (i.e.:Question answered)
    private bool isSpawning = false; // Flag to prevent double spawning
    private TriviaManager triviaManager;

    // Adjusted initial force values
    private Vector3 initialForce = new Vector3(5, 12, 0); // Adjust the values to control speed and altitude

    // Y positions for different colors (Subject to change as needed)
    private float[] yPos = { 8.0f, 10.0f, 12.0f, 14.0f };

    void Start()
    {
        triviaManager = FindObjectOfType<TriviaManager>();
        if (triviaManager == null)
        {
            Debug.LogError("TriviaManager not found. Ensure there is a TriviaManager component in the scene.");
            return;
        }
        
        skeets = new GameObject[skeetPrefabs.Length];
    }

    public void StartSpawningSkeets()
    {
        if (!isSpawning) // Check if spawning is already in progress
        {
            isSpawning = true;
            StartCoroutine(SpawnSkeets());
        }
    }

    IEnumerator SpawnSkeets()
    {
        // Instantiate all skeets at fixed positions
        for (int i = 0; i < skeetPrefabs.Length; i++)
        {
            skeets[i] = Instantiate(skeetPrefabs[i]);
            skeets[i].transform.position = new Vector3(6, yPos[i], 6);
            skeets[i].GetComponent<Renderer>().material.color = GetColorForSkeet(i); // Assign color to skeet
        }

        yield return new WaitForSeconds(launchDelay); // Wait before launching

        // Apply initial force to simulate throw for all skeets
        foreach (GameObject skeet in skeets)
        {
            Rigidbody rb = skeet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(initialForce, ForceMode.VelocityChange);
            }
        }

        // Wait for skeets to be hit before spawning the next set
        while (!skeetHit)
        {
            yield return null;
        }

        // Wait for a brief period before spawning the next set
        yield return new WaitForSeconds(spawnDelay);

        // Clear current skeets
        foreach (GameObject skeet in skeets)
        {
            if (skeet != null)
            {
                Destroy(skeet);
            }
        }

        skeetHit = false; // Reset hit flag
        isSpawning = false; // Reset spawning flag
        StartSpawningSkeets(); // Spawn new set
    }

    public void OnSkeetDestroyed(GameObject skeet)
    {
        // Check if the correct skeet was hit
        if (skeet.GetComponent<Renderer>().material.color == triviaManager.GetColorForAnswer(triviaManager.CurrentQuestionCorrectAnswerIndex))
        {
            triviaManager.IncrementScore();
        }
        
        // Set the flag to indicate that a skeet has been hit
        skeetHit = true;
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
}
