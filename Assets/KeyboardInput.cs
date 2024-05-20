using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/Keyboard Input")]

public class KeyboardInput : MonoBehaviour {
    public float speed = 6.0f;
    public float gravity = -9.8f;

    private CharacterController charController;
    private float verticalSpeed = 0;

    void Start() {
        charController = GetComponent<CharacterController>();
    }

    void Update() {
        // Get input for movement
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;
        Vector3 movement = new Vector3(-deltaX, 0, -deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        // Apply gravity
        if (charController.isGrounded) {
            verticalSpeed = 0; // Reset vertical speed when grounded
        } else {
            verticalSpeed += gravity * Time.deltaTime; // Accumulate gravity over time
        }

        // Combine movement with vertical speed
        movement.y = verticalSpeed;

        // Convert local movement to world coordinates
        movement = transform.TransformDirection(movement);

        // Move the character controller
        charController.Move(movement * Time.deltaTime);
    }
}
