using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTrigger : MonoBehaviour
{
    public float gazeDuration = 3.0f; // Time in seconds the player needs to look at the object
    private float gazeTimer = 0.0f;
    private bool isLooking = false;

    // Reference to the TriviaManager to start the game
    private TriviaManager triviaManager;

    // Renderer for changing color
    private Renderer objectRenderer;
    private Color startColor = Color.red;
    private Color endColor = Color.green;

    void Start()
    {
        triviaManager = FindObjectOfType<TriviaManager>();
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.material.color = startColor; // Set initial color to red
    }

    void Update()
    {
        if (isLooking)
        {
            gazeTimer += Time.deltaTime;
            float progress = gazeTimer / gazeDuration;
            objectRenderer.material.color = Color.Lerp(startColor, endColor, progress);

            if (gazeTimer >= gazeDuration)
            {
                triviaManager.StartGame();
                gazeTimer = 0.0f;
                
                // Delete the cube (or this game object)
                Destroy(gameObject); // Destroy the current object
            }
        }
        else
        {
            gazeTimer = 0.0f;
            objectRenderer.material.color = startColor; // Reset to start color
        }
    }

    public void SetGazeStatus(bool status)
    {
        isLooking = status;
    }
}
