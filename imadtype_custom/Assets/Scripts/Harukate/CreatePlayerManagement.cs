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

        //����
        for (int i = 0; i < PlayerPrefub.Length; i++)
        {
            Character[i] = PlayerInput.Instantiate(PlayerPrefub[i], controlScheme: "Gamepad", pairWithDevices: Gamepad.all.ToArray());
            Character[i].user.UnpairDevices();//���ׂẴf�o�C�X�����N����
            InputUser.PerformPairingWithDevice(pad[i], Character[i].user);//�f�o�C�X�������N����
            TransmissionInputData(Character[i],i );//FirstPersonController�Ƀp�b�h��񑗐M
        }

        //�A�j���[�V���������͏o���Ȃ��悤�ɂ���
        StartCoroutine(StayInputSystem());
    }

    private PlayerInput CreatePlayer(GameObject player, InputDevice device, int i)
    {
        var input = PlayerInput.Instantiate(player, controlScheme: "Gamepad", pairWithDevice: device);
        return input;
    }

    private void TransmissionInputData(PlayerInput playerInput, int playerindex)
    {
        //FirstPersonController����
        var fpscon = playerInput.gameObject.GetComponent<FirstPersonController>();

        //�Q�[���p�b�h�T��
        foreach (var device in playerInput.devices)
        {
            //null����Ȃ��ꍇ�����
            if (device is Gamepad pad)
                fpscon.gamepad = pad;
        }

        fpscon.PlayerManagement = this;
        fpscon.PlayerIndex = playerindex;
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
}
