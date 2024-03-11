using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sensitivity = 2f;

    void Update() {
        // Camera Movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float upDownInput = Input.GetAxis("UpDown");

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput + transform.up * upDownInput;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Camera Rotation
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 rotation = new Vector3(-mouseY * sensitivity, mouseX * sensitivity, 0f);
        transform.eulerAngles += rotation;
    }
}
