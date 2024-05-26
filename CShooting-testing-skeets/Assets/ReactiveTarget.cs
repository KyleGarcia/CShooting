// ReactiveTarget.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    private SceneController sceneController;
    private bool isGrounded = false; // Flag to prevent multiple respawns

    public void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        if (sceneController == null)
        {
            Debug.LogError("SceneController not found in the scene.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isGrounded)
        {
            isGrounded = true;
            if (sceneController != null)
            {
                sceneController.OnSkeetHitGround(this.gameObject);
            }
            else
            {
                Debug.LogError("SceneController is not assigned.");
            }
        }
    }

    public void ReactToHit()
    {
        Debug.Log("Target hit");
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        this.transform.Rotate(-75, 0, 0);
        yield return new WaitForSeconds(1.5f);

        if (sceneController != null)
        {
            Debug.Log("Calling OnSkeetDestroyed from ReactiveTarget.");
            sceneController.OnSkeetDestroyed(this.gameObject);
        }
        else
        {
            Debug.LogError("SceneController is not assigned.");
        }

        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }
}
