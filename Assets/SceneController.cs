using System.Collections;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject[] skeetPrefabs; // Array of different colored skeet prefabs
    private GameObject[] skeets; // Array to hold the spawned skeet objects

    public float spawnDelay = 2.0f;
    private float launchDelay = 0.5f; // Delay before launching skeets

    // Y positions for different colors (adjust as needed)
    private float[] yPos = { 8.0f, 10.0f, 12.0f, 14.0f };

    void Start()
    {
        StartCoroutine(SpawnSkeets());
    }

    IEnumerator SpawnSkeets()
    {
        // Instantiate all skeets at once
        skeets = new GameObject[skeetPrefabs.Length];
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
                Vector3 initialForce = new Vector3(5, 10, 0); // Adjust the values to control the curve
                rb.AddForce(initialForce, ForceMode.VelocityChange);
            }
        }
    }
}
