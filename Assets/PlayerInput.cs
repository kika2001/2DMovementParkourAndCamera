using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Movement currentMovement;
    private InputMaster controls;
    private Vector2 _moveAxis;
    void OnEnable()
    {
        //controls.Player.Movement.performed += HandleMove;
        controls.Player.Movement.Enable();
        controls.Player.Jump.Enable();
        controls.Player.Jump.performed += HandleJump;
        //controls.Player.Dash.Enable();
        //controls.Player.Dash.performed += DashHandler;
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    private void Awake()
    {
        controls = new InputMaster();
        currentMovement = GetComponent<Movement>();
    }

    private void Update()
    {
        HandleInputs();
    }

    private void FixedUpdate()
    {
        if (_moveAxis.x !=0)
        {
            currentMovement.Move(_moveAxis.x);
        }
        
    }

    void HandleInputs()
    {
        _moveAxis = controls.Player.Movement.ReadValue<Vector2>();
    }
    void HandleJump(InputAction.CallbackContext ctx)
    {
        Debug.Log("HandlerWorked");
        currentMovement.Jump();
    }
}
