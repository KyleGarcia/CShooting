using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeetPhysics : MonoBehaviour
{
    private Rigidbody rb;
    public float fallSpeedMultiplier = 0.5f; // Fall Speed Multiplier

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, rb.velocity.y * fallSpeedMultiplier, 0);
    }

    void FixedUpdate()
    {
        // Override the gravity effect to slow down the falling speed
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Max(rb.velocity.y, -Mathf.Abs(Physics.gravity.y * fallSpeedMultiplier)), rb.velocity.z);
    }
}
