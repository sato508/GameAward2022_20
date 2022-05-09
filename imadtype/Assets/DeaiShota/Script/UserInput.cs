/*------------------------------------------------------
 * 
 *  [UserInput.cs]
 *  Author : �o���đ�
 *  ���[�U�[�̓��͏������Ǘ�
 * 
 ------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(StarterAssets.StarterAssetsInputs))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
[RequireComponent(typeof(PlayerInput))]
#endif
public class UserInput : MonoBehaviour
{
    /// <summary>
    /// InputSystem
    /// </summary>
    private StarterAssets.StarterAssetsInputs _Input;
    private PlayerInput _PlayerInput;

    private TitleTextEvent _Event;

    // Start is called before the first frame update
    void Start()
    {
        _Input = GetComponent<StarterAssets.StarterAssetsInputs>();
        _PlayerInput = GetComponent<PlayerInput>();
        _Event = GameObject.Find("EventManager").GetComponent<TitleTextEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        // ������
        if (_Input.attack)
        {
            _Event.NextEvent();
            _Input.attack = false;
        }       

        // ����{�^��

        // �߂�{�^��
    }
}
