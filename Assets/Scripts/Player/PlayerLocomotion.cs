using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    Rigidbody rigidBody;


    public float MovementSpeed;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        inputManager = GetComponent<InputManager>();
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        Aim();
    }
    
    void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        
        moveDirection.z = inputManager.VerticalInput * -1;
        moveDirection.x = inputManager.HorizontalInput * -1;
        moveDirection.Normalize();
        moveDirection.y = 0;
        
        moveDirection = moveDirection * MovementSpeed;

        //Vector3 movementVelocity = moveDirection;
        //movementVelocity.y = rigidBody.velocity.y;
        //rigidBody.velocity = movementVelocity;
        transform.position += moveDirection * Time.deltaTime;
    }

    private void Aim()
    {
        var direction = inputManager.AimTarget.transform.position - transform.position;
            
        direction.y = 0;

        transform.forward = direction;
    }
}
