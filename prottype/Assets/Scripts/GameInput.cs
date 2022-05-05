using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class GameInput : MonoBehaviour
{
    private Vector2 m_move      = new Vector2(0, 0);
    private Vector2 m_look      = new Vector2(0, 0);
    private bool    m_attack    = false;
    private bool    m_jump      = false;

    public Vector2  Move { get { return m_move; } }
    public Vector2  Look { get { return m_look; } }
    public bool     Attack { 
        get { return m_attack; }
        set { m_attack = value; }
    }
    public bool     Jump { get { return m_jump; } }

    private void OnMove(InputAction.CallbackContext context)
    {
        m_move = context.ReadValue<Vector2>();
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        m_look = context.ReadValue<Vector2>();
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        m_attack = context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        m_jump = context.ReadValue<float>() >= InputSystem.settings.defaultButtonPressPoint;
    }
}
