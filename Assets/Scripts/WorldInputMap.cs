using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.Animations;

public class WorldInputMap : MonoBehaviour
{

    public static WorldInputMap Instance;

    public PlayerInputAction.WorldActions InputActions;

    public event EventHandler OnJumpAction;


    private void Awake()
    {
        Instance = this;
        InputActions = InputManager.Instance.InputActions.World;
        InputActions.Jump.performed += JumpPerformed;
    }

    private void JumpPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Jump");
        OnJumpAction?.Invoke(this , EventArgs.Empty);
    }

    public Vector2 GetLookAxis()
    {
        return InputActions.Look.ReadValue<Vector2>();
    }


    public Vector2 GetMoveDirection()
    {
        return InputActions.Move.ReadValue<Vector2>();
    }

}
