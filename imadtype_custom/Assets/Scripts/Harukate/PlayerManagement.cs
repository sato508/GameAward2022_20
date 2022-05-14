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
    [Header("P�P�ɃL�[�{�[�h�}�E�X�Ή�(�f�o�b�O�̂�&true��P1���p�b�h�g�p�s��)")]
    public bool EnableKeybordMouse = true;
#endif

    [Header("�J����")]
    [Tooltip("�J�����v���n�u")]
    public GameObject CameraPrefub;

    [Header("�v���C���[")]
    [Tooltip("�v���C���[��")]
    public int PlayerCount = 1;
    [Tooltip("�v���C���[�v���p�e�B")]
    public PlayerProperty PlayerProperty;
    [Tooltip("�v���C���[�v���n�u")]
    public GameObject PlayerPrefub;
    [Tooltip("�J�n�ʒu")]
    public Transform[] StartPosition;

    //�L�����N�^�[���
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
            Debug.LogWarning("StartPosition��PlayerCount�̐������Ȃ��f�X�B");
#endif

        var pad = Gamepad.all;

        _character = new GameObject[PlayerCount];
        _camera = new GameObject[PlayerCount];

        //����
        for (int i = 0; i < PlayerCount; i++)
        {
            PlayerInput input;
#if UNITY_EDITOR
            //�L�[�{�[�h�}�E�X����
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

            //�ێ�
            _character[i] = input.gameObject;

            //���W�Ɖ�]���Z�b�g
            _character[i].transform.SetPositionAndRotation(StartPosition[i].position, StartPosition[i].rotation);

            //FirstPersonController�ɏ�񑗐M
            TransmissionPlayerData(input, i);

            //�J��������
            _camera[i] = Instantiate(CameraPrefub);
            TransmissionCameraData(_camera[i], _character[i], i);
        }

        //�A�j���[�V���������͏o���Ȃ��悤�ɂ���
        StartCoroutine(StayInputSystem());
    }

    //�ݒ肳��Ă�Q�[���p�b�h�̏��𑗐M����
    private void TransmissionPlayerData(PlayerInput playerInput, int playerindex)
    {
        //FirstPersonController����
        var fpscon = playerInput.gameObject.GetComponent<FirstPersonController>();

        //�K�v�ȏ��𑗂�
        fpscon.PlayerManagement = this;
        fpscon.PlayerIndex = playerindex;
    }

    //�J�������Z�b�g
    private void TransmissionCameraData(GameObject cameraobj, GameObject player, int playerindex)
    {
        int LayerIndex = LayerMask.NameToLayer("P" + (playerindex + 1));

        //CinemachineVirtualCamera�Ⴄ
        CinemachineVirtualCamera cinemavc =
            cameraobj.transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
        cinemavc.gameObject.layer = LayerIndex;//���C���[�ύX����

        //CinemachineTarget�̃I�u�W�F�N�g�T���ăZ�b�g����
        foreach (Transform child in player.transform)
            if (child.CompareTag("CinemachineTarget"))
                cinemavc.Follow = child;


        //�J�����ɕK�v�ȏ��Z�b�g����
        Camera camera = cameraobj.transform.GetChild(1).GetComponent<Camera>();
        camera.targetDisplay = playerindex;
        camera.cullingMask ^= 1 << LayerIndex;
    }

    private IEnumerator StayInputSystem()
    {
        //���ׂẴf�o�C�X�擾
        var gamepads = Gamepad.all;

        //���ׂẴf�o�C�X�̓��͂��~�߂�
        foreach (var gamepad in gamepads)
        {
            InputSystem.DisableDevice(gamepad);
        }
        yield return new WaitForSeconds(5.0f);

        //���ׂẴf�o�C�X�̓��͂𓮂���
        foreach (var gamepad in gamepads)
        {
            InputSystem.EnableDevice(gamepad);
        }
    }

    //�J�������Z�b�g

    //�G�̃I�u�W�F�N�g���擾����
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

    //�G�̃I�u�W�F�N�g���擾����(���X�g)
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