using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Cinemachine;
using StarterAssets;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Vector3 = UnityEngine.Vector3;

public class PlayerManagement : MonoBehaviour
{
    public MatchManager matchManager;

#if UNITY_EDITOR
    [Header("P１にキーボードマウス対応(デバッグのみ&true時P1がパッド使用不可)")]
    public bool EnableKeybordMouse = true;
#endif

    [Header("カメラ")]
    [Tooltip("カメラプレハブ")]
    public GameObject CameraPrefub;

    [Header("プレイヤー")]
    [Tooltip("プレイヤー数")]
    public int PlayerCount = 1;
    [Tooltip("プレイヤープロパティ")]
    public PlayerProperty PlayerProperty;
    [Tooltip("プレイヤープレハブ")]
    public GameObject PlayerPrefub;
    [Tooltip("開始位置")]
    public Transform[] StartPosition;

    //キャラクター情報
    private GameObject[] _character;

    private GameObject[] _camera;
    public GameObject[] CharacterCamera
    {
        get { return _camera; }
    }

    void Awake()
    {
#if UNITY_EDITOR
        if(StartPosition.Length < PlayerCount)
            Debug.LogWarning("StartPositionとPlayerCountの数が少ないデス。");
#endif

        var pad = Gamepad.all;

        _character = new GameObject[PlayerCount];
        _camera = new GameObject[PlayerCount];

        //生成
        for (int i = 0; i < PlayerCount; i++)
        {
            PlayerInput input;
#if UNITY_EDITOR
            //キーボードマウス込み
            if (EnableKeybordMouse && i == 0)
            {
                input = PlayerInput.Instantiate(PlayerPrefub,
                    pairWithDevices: new InputDevice[] {Keyboard.current, Mouse.current});
            }
            else
            {
                if (pad.Count <= i)
                    break;

                input = PlayerInput.Instantiate(PlayerPrefub, pairWithDevice: pad[i]);
            }
#else
            if (pad.Count <= i)
                    break;
                    
            input = PlayerInput.Instantiate(PlayerPrefub, pairWithDevice: pad[i]);
#endif

            //保持
            _character[i] = input.gameObject;

            //座標と回転をセット
            _character[i].transform.SetPositionAndRotation(StartPosition[i].position, StartPosition[i].rotation);

            //FirstPersonControllerに情報送信
            TransmissionPlayerData(input, i);

            //カメラ生成
            _camera[i] = Instantiate(CameraPrefub);
            TransmissionCameraData(_camera[i], _character[i], i);
        }

        //アニメーション中入力出来ないようにする
        StartCoroutine(StayInputSystem());
    }

    //設定されてるゲームパッドの情報を送信する
    private void TransmissionPlayerData(PlayerInput playerInput, int playerindex)
    {
        //FirstPersonController入手
        var fpscon = playerInput.gameObject.GetComponent<FirstPersonController>();

        //必要な情報を送る
        fpscon.PlayerManagement = this;
        fpscon.PlayerIndex = playerindex;
    }

    //カメラ情報セット
    private void TransmissionCameraData(GameObject cameraobj, GameObject player, int playerindex)
    {
        int LayerIndex = LayerMask.NameToLayer("P" + (playerindex + 1));

        //CinemachineVirtualCamera貰う
        CinemachineVirtualCamera cinemavc =
            cameraobj.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemavc.gameObject.layer = LayerIndex;//レイヤー変更する

        //CinemachineTargetのオブジェクト探してセットする
        foreach (Transform child in player.transform)
            if (child.CompareTag("CinemachineTarget"))
                cinemavc.Follow = child;


        //カメラに必要な情報セットする
        Camera camera = cameraobj.transform.GetChild(1).GetComponent<Camera>();
        camera.targetDisplay = playerindex;
        camera.cullingMask ^= 1 << LayerIndex;
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

    //カメラ情報セット

    //敵のオブジェクトを取得する
    public GameObject GetEnemy(int MyPlayerIndex)
    {
        for (int i = 0; i < _character.Length; i++)
        {
            if(MyPlayerIndex == i)
                continue;

            return _character[i];
        }

        return null;
    }

    //敵のオブジェクトを取得する(リスト)
    public List<GameObject> GetEnemyList(int MyPlayerIndex)
    {
        List<GameObject> enemy = new List<GameObject>();

        for (int i = 0; i < _character.Length; i++)
        {
            if(MyPlayerIndex == i)
                continue;

            enemy.Add(_character[i]);
        }

        return enemy;
    }
}