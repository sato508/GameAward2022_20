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

    private void OnMove(InputValue value)
    {
        m_move = value.Get<Vector2>();
    }
    private void OnLook(InputValue value)
    {
        m_look = value.Get<Vector2>();
    }
    private void OnAttack(InputValue value)
    {
        m_attack = value.Get<float>() >= InputSystem.settings.defaultButtonPressPoint;
    }
    private void OnJump(InputValue value)
    {
        m_jump = value.Get<float>() >= InputSystem.settings.defaultButtonPressPoint;
    }
}
