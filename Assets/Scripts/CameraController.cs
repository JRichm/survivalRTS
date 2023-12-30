using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] public float movementSpeed;
    [SerializeField] private GameObject holderObj;
    [SerializeField] private Camera _camera;

    private float zoomLevel = 0f;
    private float startZoom;


    private Vector3 moveDirection;
    private void Start() {
        startZoom = _camera.fieldOfView;
    }


    private void handleInput() {
        Vector3 moveDirection = Vector3.zero;


        // get user input
        if (Input.GetKey(KeyCode.W)) {
            moveDirection += Quaternion.Euler(0, 45, 0) * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveDirection += Quaternion.Euler(0, 45, 0) * Vector3.back;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveDirection += Quaternion.Euler(0, 45, 0) * Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveDirection += Quaternion.Euler(0, 45, 0) * Vector3.right;
        }

        // Normalize to ensure consistent speed in all directions
        if (moveDirection.magnitude > 1f) {
            moveDirection.Normalize();
        }

        // move camera
        transform.Translate(moveDirection * movementSpeed * Time.deltaTime, Space.World);

        // get mouse delta
        float mousedelta = Input.mouseScrollDelta.y;

        if (mousedelta != 0f) ZoomCamera(mousedelta);
    }

    private void ZoomCamera(float _mousedelta) {
        if (zoomLevel + -_mousedelta < -2) return;
        if (zoomLevel + -_mousedelta > 8) return;

        zoomLevel += -_mousedelta;
        _camera.fieldOfView = startZoom + zoomLevel * 5;

    }

    private void Update() {
        handleInput();
    }
}
