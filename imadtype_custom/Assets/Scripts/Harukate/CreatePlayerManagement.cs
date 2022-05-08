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

        //����
        for (int i = 0; i < PlayerPrefub.Length; i++)
        {
            Character[i] = CreatePlayer(PlayerPrefub[i]).gameObject;
            var fpscon = Character[i].GetComponent<FirstPersonController>();
            fpscon.PlayerManagement = this;
            fpscon.PlayerIndex = i;
        }

        //�A�j���[�V���������͏o���Ȃ��悤�ɂ���
        StartCoroutine(StayInputSystem());
    }

    private PlayerInput CreatePlayer(GameObject player)
    {
        var input = PlayerInput.Instantiate(player);
        return input;
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

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log(playerInput.gameObject.name + "�Q��");

        //FirstPersonController����
        var fpscon = playerInput.gameObject.GetComponent<FirstPersonController>();

        //�Q�[���p�b�h�T��
        foreach (var device in playerInput.devices)
        {
            //null����Ȃ��ꍇ�����
            if (device is Gamepad pad)
                fpscon.gamepad = pad;

            Debug.Log("�f�o�C�X���F" + device.name);
        }
    }
}
