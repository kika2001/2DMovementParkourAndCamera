using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Movement currentMovement;
    private InputMaster controls;
    private Vector2 _moveAxis;
    private bool runInput;
    void OnEnable()
    {
        //controls.Player.Movement.performed += HandleMove;
        controls.Player.Movement.Enable();
        controls.Player.Jump.Enable();
        controls.Player.Jump.performed += HandleJump;
        controls.Player.Run.Enable();
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
        //currentMovement = GetComponent<Movement>();
    }

    private void Update()
    {
        HandleInputs();
    }

    private void FixedUpdate()
    {
        if (_moveAxis.x !=0)
        {
            currentMovement.Move(_moveAxis.x,runInput);
        }
        
    }

    private void OnDrawGizmos()
    {
        var input = _moveAxis;
        var vector = Vector2.up;
        var offset = 0.8f;
        input = input.normalized;
        if ((input.x >=vector.x-offset && input.x <= vector.x+offset) && (input.y >= vector.y - offset && input.y <=vector.y + offset))
        {
            Gizmos.color= Color.green;
        }
        else
        {
            Gizmos.color= Color.blue;
        }
        Gizmos.DrawRay(transform.position, _moveAxis*2f);
    }

    void HandleInputs()
    {
        _moveAxis = controls.Player.Movement.ReadValue<Vector2>();
        runInput = (controls.Player.Run.ReadValue<float>() != 0f) ? true : false;
        //Debug.LogError(_moveAxis);
    }
    void HandleJump(InputAction.CallbackContext ctx)
    {
        //Debug.Log("HandlerWorked");
        currentMovement.Jump(_moveAxis);
    }
}
