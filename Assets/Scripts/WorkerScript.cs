using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorkerScript : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    private bool isMoving;
    private bool reachedTarget;
    private Vector3 currentTargetPosition;
    private Vector3 currentPosition;

    private void Start() {
        isMoving = true;
        reachedTarget = true;
    }

    private void Update() {
        currentPosition = transform.position;


        if (isMoving) {

            if (Vector3.Distance(currentPosition, currentTargetPosition) < 0.01f) reachedTarget = true;

            if (reachedTarget) {
                FindNewTargetPosition();
            } else {
                HandleMovement();
            }
        }

        Debug.DrawLine(currentPosition, currentTargetPosition, Color.green);
        Debug.DrawLine(currentTargetPosition, new Vector3(currentTargetPosition.x, currentTargetPosition.y + 10, currentTargetPosition.z), Color.red);

    }

    private void FindNewTargetPosition() {
        float randomX = Random.Range(-5f, 5f);
        float randomZ = Random.Range(-5f, 5f);

        Vector3 newPosition = new Vector3(currentPosition.x + randomX, currentPosition.y, currentPosition.z + randomZ);
        currentTargetPosition = newPosition;
        reachedTarget = false;

        Debug.Log(randomX + "" + randomZ);
        Debug.Log(currentPosition);
        Debug.Log(newPosition);
    }

    private void SetIdle() {
        isMoving = false;
    }

    private void HandleMovement() {

        Vector3 direction = currentTargetPosition - transform.position;

        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);
    }
}
