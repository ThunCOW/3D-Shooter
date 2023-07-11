using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;

    [SerializeField]
    PlayerGunSelector GunSelector;

    // Start is called before the first frame update
    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion= GetComponent<PlayerLocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        inputManager.HandleAllInputs();
    }

    void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
}