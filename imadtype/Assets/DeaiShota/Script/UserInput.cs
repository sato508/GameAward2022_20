/*------------------------------------------------------
 * 
 *  [UserInput.cs]
 *  Author : 出合翔太
 *  ユーザーの入力処理を管理
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
        // 仮実装
        if (_Input.attack)
        {
            _Event.NextEvent();
            _Input.attack = false;
        }       

        // 決定ボタン

        // 戻るボタン
    }
}
