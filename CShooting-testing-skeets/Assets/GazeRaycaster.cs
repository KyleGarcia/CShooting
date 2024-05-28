using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeRaycaster : MonoBehaviour
{
    public float maxDistance = 10.0f; // Max distance for the raycast

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            GazeTrigger gazeTrigger = hit.collider.GetComponent<GazeTrigger>();

            if (gazeTrigger != null)
            {
                gazeTrigger.SetGazeStatus(true);
            }
        }
        else
        {
            GazeTrigger[] gazeTriggers = FindObjectsOfType<GazeTrigger>();

            foreach (GazeTrigger gazeTrigger in gazeTriggers)
            {
                gazeTrigger.SetGazeStatus(false);
            }
        }
    }
}