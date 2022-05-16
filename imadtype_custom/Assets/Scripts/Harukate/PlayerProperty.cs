using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProperty", menuName = "Property/PlayerProperty")]
public class PlayerProperty : ScriptableObject
{
    [Header("プレイヤー")]
    [Tooltip("キャラクターの移動速度(m/s)")]
    public float MoveSpeed = 4.0f;
    [Tooltip("キャラクターの走る速度(m/s)")]
    public float SprintSpeed = 6.0f;
    [Tooltip("キャラクタの回転速度")]
    public float RotationSpeed = 1.0f;
    [Tooltip("加速度・減速度")]
    public float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("ジャンプ力ぅですかねぇ")]
    public float JumpHeight = 1.2f;
    [Tooltip("キャラクターは独自の重力値。エンジンのデフォルトは-9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("再びジャンプできるようになるまでの経過時間。0fに設定すると、瞬時に再ジャンプ出来る")]
    public float JumpTimeout = 0.1f;
    [Tooltip("落下状態になるまでの経過時間。階段を下りるときに便利")]
    public float FallTimeout = 0.15f;

    [Header("地面情報")]
    [Tooltip("段差対策のオフセット")]
    public float GroundedOffset = -0.14f;
    [Tooltip("地面判定を受け取る当たり判定の半径")]
    public float GroundedRadius = 0.4f;
    [Tooltip("地面のレイヤー")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("カメラを何度まで上に動かせるか")]
    public float TopClamp = 90.0f;
    [Tooltip("カメラを何度まで下げられるか")]
    public float BottomClamp = -90.0f;

    [Header("攻撃")]
    [Tooltip("攻撃を出し入れする速度")]
    public float AttackPopOutSpeed = 0.3f;
    [Tooltip("攻撃回転速度")]
    public float AttaclSpeed = 0.5f;

    [Header("Sound")]
    [Tooltip("攻撃音")]
    public AudioClip seSwing;
}
