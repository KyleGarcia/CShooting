using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeetPhysics : MonoBehaviour
{
    private Rigidbody rb;
    public float fallSpeedMultiplier = 0.2f; // Fall Speed Multiplier
    public float gravityReduction = 0.1f; // Reduction factor for gravity

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on skeet.");
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Reduce the gravity effect to slow down the falling speed
            Vector3 reducedGravity = Physics.gravity * gravityReduction;
            rb.AddForce(-reducedGravity, ForceMode.Acceleration);

            // Ensure the falling speed does not exceed a certain limit
            if (rb.velocity.y < -Mathf.Abs(Physics.gravity.y * fallSpeedMultiplier))
            {
                rb.velocity = new Vector3(rb.velocity.x, -Mathf.Abs(Physics.gravity.y * fallSpeedMultiplier), rb.velocity.z);
            }
        }
    }

    public void ResetVelocity()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.drag = 0;
            rb.angularDrag = 0;
            rb.useGravity = true;
        }
        else
        {
            Debug.LogError("Rigidbody component not found when trying to reset velocity.");
        }
    }
}
