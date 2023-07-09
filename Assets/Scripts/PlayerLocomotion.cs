using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    Vector3 moveDirection;
    Camera mainCamera;
    Transform cameraObject;

    Rigidbody playerRigidBody;

    public float MovementSpeed;
    public float RotationSpeed;
    // Start is called before the first frame update
    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidBody = GetComponent<Rigidbody>();

        mainCamera = Camera.main;
        cameraObject = mainCamera.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        Aim();
    }
    
    void HandleMovement()
    {
        //moveDirection = AimTarget.transform.forward * inputManager.VerticalInput * -1;
        //moveDirection = moveDirection + AimTarget.transform.right * inputManager.HorizontalInput * -1;
        moveDirection = mainCamera.transform.forward * inputManager.VerticalInput;
        //moveDirection = new Vector3(0,0,1) * inputManager.VerticalInput;
        moveDirection = moveDirection + mainCamera.transform.right * inputManager.HorizontalInput;
        //moveDirection = moveDirection + new Vector3(1, 0, 0) * inputManager.HorizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;
        
        moveDirection = moveDirection * MovementSpeed;

        Vector3 movementVelocity = moveDirection;
        movementVelocity.y = playerRigidBody.velocity.y;
        playerRigidBody.velocity = movementVelocity;
    }

    private void Aim()
    {
        var direction = inputManager.AimTarget.transform.position - transform.position;
            
        direction.y = 0;

        transform.forward = direction;
    }
}
