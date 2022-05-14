using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperty", menuName = "Property/PlayerProperty")]
public class PlayerProperty : ScriptableObject
{
    [Header("�v���C���[")]
    [Tooltip("�L�����N�^�[�̈ړ����x(m/s)")]
    public float MoveSpeed = 4.0f;
    [Tooltip("�L�����N�^�[�̑��鑬�x(m/s)")]
    public float SprintSpeed = 6.0f;
    [Tooltip("�L�����N�^�̉�]���x")]
    public float RotationSpeed = 1.0f;
    [Tooltip("�����x�E�����x")]
    public float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("�W�����v�͂��ł����˂�")]
    public float JumpHeight = 1.2f;
    [Tooltip("�L�����N�^�[�͓Ǝ��̏d�͒l�B�G���W���̃f�t�H���g��-9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("�ĂуW�����v�ł���悤�ɂȂ�܂ł̌o�ߎ��ԁB0f�ɐݒ肷��ƁA�u���ɍăW�����v�o����")]
    public float JumpTimeout = 0.1f;
    [Tooltip("������ԂɂȂ�܂ł̌o�ߎ��ԁB�K�i�������Ƃ��ɕ֗�")]
    public float FallTimeout = 0.15f;

    [Header("�n�ʏ��")]
    [Tooltip("�i���΍�̃I�t�Z�b�g")]
    public float GroundedOffset = -0.14f;
    [Tooltip("�n�ʔ�����󂯎�铖���蔻��̔��a")]
    public float GroundedRadius = 0.4f;
    [Tooltip("�n�ʂ̃��C���[")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("�J���������x�܂ŏ�ɓ������邩")]
    public float TopClamp = 90.0f;
    [Tooltip("�J���������x�܂ŉ������邩")]
    public float BottomClamp = -90.0f;

    [Header("�U��")]
    [Tooltip("�U�����o�����ꂷ�鑬�x")]
    public float AttackPopOutSpeed = 0.3f;
    [Tooltip("�U����]���x")]
    public float AttaclSpeed = 0.5f;

    [Header("Sound")]
    [Tooltip("�U����")]
    public AudioClip seSwing;
}
