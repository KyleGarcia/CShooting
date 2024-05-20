using System.Collections;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject[] skeetPrefabs; // Array of different colored skeet prefabs
    private GameObject[] skeets; // Array to hold the spawned skeet objects

    public float spawnDelay = 2.0f; // Control the delay between skeet destruction and next spawn
    private float launchDelay = 0.5f; // Delay before launching skeets(applying force) instantiate and position skeets
    private bool skeetHit = false; // Track if any skeet has been hit (i.e.:Question answered)

    // Adjusted initial force values
    private Vector3 initialForce = new Vector3(5, 12, 0); // Adjust the values to control speed and altitude

    // Y positions for different colors (Subject to change as needed)
    private float[] yPos = { 8.0f, 10.0f, 12.0f, 14.0f };

    void Start()
    {
        skeets = new GameObject[skeetPrefabs.Length];
        StartCoroutine(SpawnSkeets());
    }

    IEnumerator SpawnSkeets()
    {
        // Instantiate all skeets at once
        for (int i = 0; i < skeetPrefabs.Length; i++)
        {
            skeets[i] = Instantiate(skeetPrefabs[i]);
            skeets[i].transform.position = new Vector3(6, yPos[i], 6);
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
        StartCoroutine(SpawnSkeets()); // Spawn new set
    }

    public void OnSkeetDestroyed(GameObject skeet)
    {
        // Set the flag to indicate that a skeet has been hit
        skeetHit = true;
    }
}
