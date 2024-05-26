using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private Canvas hudCanvas;

    void Start()
    {
        // Get the Cardboard camera (main camera)
        Camera cardboardCamera = Camera.main;
        
        // Set the canvas parent to the camera
        hudCanvas.transform.SetParent(cardboardCamera.transform);

        // Adjust the position and rotation relative to the camera
        hudCanvas.transform.localPosition = new Vector3(0, 0, 2);
        hudCanvas.transform.localRotation = Quaternion.identity;
        hudCanvas.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
