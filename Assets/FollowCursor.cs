using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class FollowCursor : MonoBehaviour
{
    // Start is called before the first frame update
    /*
     * if (Input.GetKeyDown(KeyCode.Space))
             MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown); 
             To simulate mouse press             
            ^
            |
            nao funciona assim
    
        https://docs.unity3d.com/2018.1/Documentation/ScriptReference/UI.GraphicRaycaster.Raycast.html outra forma
     */
    private InputMaster controls;
    private SpriteRenderer sp;
    [SerializeField] private Sprite idleCursor;
    private Vector2 currentMousePos;
    private Vector2 _moveAxis;
    [SerializeField] private float mouseSpeed;
    private void Awake()
    {
        controls = new InputMaster();
        sp = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        controls.Player.RightAnalog.Enable();
        controls.Player.RightAnalog.performed += MoveCursorHandler;
    }

    private void OnDisable()
    {
        controls.Player.RightAnalog.Disable();
    }

    private void MoveCursorHandler(InputAction.CallbackContext obj)
    {
        _moveAxis = controls.Player.RightAnalog.ReadValue<Vector2>();
        currentMousePos += _moveAxis * mouseSpeed;
        //Mouse.current.position.WriteValueIntoState(currentMousePos, Tstate);
        Mouse.current.WarpCursorPosition(currentMousePos);
        Mouse.current.MakeCurrent();
        //MouseState s;
        //s.position = currentMousePos;
        //Mouse.current.position.WriteValueIntoState(currentMousePos,s.);

        
    }

    void Start()
    {
        Cursor.visible = false;
        sp.sprite = idleCursor;
        currentMousePos = new Vector2(Screen.width/2,Screen.height/2);
        
    }

    public void testCLick()
    {
        Debug.Log("Test CLick worked");
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        var worldpos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        transform.position = new Vector3(worldpos.x,worldpos.y,0);
       
    }

    
}
