using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class CreatePlayerManagement : MonoBehaviour
{

    public MatchManager matchManager;
    public GameObject[] PlayerPrefub;

    public PlayerInput[] Character;

    public PlayerInputManager inputmanager;

    void Awake()
    {
        var pad = Gamepad.all;

        Character = new PlayerInput[PlayerPrefub.Length];

        //生成
        for (int i = 0; i < PlayerPrefub.Length; i++)
        {
            Character[i] = PlayerInput.Instantiate(PlayerPrefub[i], controlScheme: "Gamepad", pairWithDevices: Gamepad.all.ToArray());
            Character[i].user.UnpairDevices();//すべてのデバイスリンク解除
            InputUser.PerformPairingWithDevice(pad[i], Character[i].user);//デバイスをリンクする
            TransmissionInputData(Character[i],i );//FirstPersonControllerにパッド情報送信
        }

        //アニメーション中入力出来ないようにする
        StartCoroutine(StayInputSystem());
    }

    private PlayerInput CreatePlayer(GameObject player, InputDevice device, int i)
    {
        var input = PlayerInput.Instantiate(player, controlScheme: "Gamepad", pairWithDevice: device);
        return input;
    }

    private void TransmissionInputData(PlayerInput playerInput, int playerindex)
    {
        //FirstPersonController入手
        var fpscon = playerInput.gameObject.GetComponent<FirstPersonController>();

        //ゲームパッド探し
        foreach (var device in playerInput.devices)
        {
            //nullじゃない場合入れる
            if (device is Gamepad pad)
                fpscon.gamepad = pad;
        }

        fpscon.PlayerManagement = this;
        fpscon.PlayerIndex = playerindex;
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
}
