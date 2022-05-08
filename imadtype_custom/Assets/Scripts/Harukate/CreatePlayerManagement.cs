using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreatePlayerManagement : MonoBehaviour
{

    public MatchManager matchManager;
    public GameObject[] PlayerPrefub;

    public GameObject[] Character;

    void Awake()
    {
        Character = new GameObject[PlayerPrefub.Length];

        //生成
        for (int i = 0; i < PlayerPrefub.Length; i++)
        {
            Character[i] = CreatePlayer(PlayerPrefub[i]).gameObject;
            var fpscon = Character[i].GetComponent<FirstPersonController>();
            fpscon.PlayerManagement = this;
            fpscon.PlayerIndex = i;
        }

        //アニメーション中入力出来ないようにする
        StartCoroutine(StayInputSystem());
    }

    private PlayerInput CreatePlayer(GameObject player)
    {
        var input = PlayerInput.Instantiate(player);
        return input;
    }

    private IEnumerator StayInputSystem()
    {
        //すべてのデバイス取得
        var gamepads = Gamepad.all;

        //すべてのデバイスの入力を止める
        foreach (var gamepad in gamepads)
        {
            InputSystem.DisableDevice(gamepad);
        }
        yield return new WaitForSeconds(5.0f);

        //すべてのデバイスの入力を動かす
        foreach (var gamepad in gamepads)
        {
            InputSystem.EnableDevice(gamepad);
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log(playerInput.gameObject.name + "参加");

        //FirstPersonController入手
        var fpscon = playerInput.gameObject.GetComponent<FirstPersonController>();

        //ゲームパッド探し
        foreach (var device in playerInput.devices)
        {
            //nullじゃない場合入れる
            if (device is Gamepad pad)
                fpscon.gamepad = pad;

            Debug.Log("デバイス名：" + device.name);
        }
    }
}
