using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject skeetPrefab;
    private GameObject skeet;
    private bool firstSpawnDone = false; // Make sure not to spawn duplicates at loading time
    public float spawnDelay = 2.0f;

    void Start()
    {
        StartCoroutine(SpawnSkeet());
    }

    IEnumerator SpawnSkeet()
    {
        while (true)
        {
            if (skeet == null)
            {
                if (!firstSpawnDone)
                {
                    firstSpawnDone = true; // Setting flag to indicate first spawn has completed
                }
                else
                {
                    yield return new WaitForSeconds(spawnDelay); // wait to spawn
                }

                // Instantiate and position the skeet
                skeet = Instantiate(skeetPrefab);
                skeet.transform.position = new Vector3(6, 8, 6);

                // Apply initial force to simulate throw
                Rigidbody rb = skeet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 initialForce = new Vector3(5, 10, 0); // Adjust the values to control the curve
                    rb.AddForce(initialForce, ForceMode.VelocityChange);
                }
            }
            yield return null;
        }
    }

}
