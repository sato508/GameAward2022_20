using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GameInput))]
public class Player : MonoBehaviour
{
    // 外部パラメーター
    [Header("Hp")]
    [SerializeField] AttackCollision attackCollision;
    [SerializeField] private int hp;
    public int Hp
    {
        set //値をhpに代入する
        {
            this.hp = Math.Max(this.hp - value, 0);
        }
        get //値を返す
        {
            return this.hp;
        }
    }
    [Header("Movement")]
    [SerializeField] private float      MovingSpeedAttenuate    = 0.0f; // 移動速度の減衰値
    [SerializeField] private float      MovingSpeedAccel        = 0.0f; // 移動速度の加速値
    [SerializeField] private float      MovingSpeedMin          = 0.0f; // 移動速度の最低値
    [SerializeField] private float      MovingSpeedMax          = 0.0f; // 移動速度の最高値
    [Header("Jump")]
    [SerializeField] private Transform  Foot                = null; // 足元の基準点
    [SerializeField] private float      IsGroundDistance    = 0.0f; // 着地と判断する距離
    [SerializeField] private float      JumpForce           = 0.0f; // ジャンプする力
    [SerializeField] private float      JumpCooltime        = 0.0f; // ジャンプする間隔
    [Header("Camera")]
    [SerializeField] private Transform  Eye                 = null;             // カメラの基準点
    [SerializeField] private AxisState  VerticalAxis        = new AxisState();  // 縦軸の設定
    [SerializeField] private AxisState  HorizontalAxis      = new AxisState();  // 横軸の設定
    [Header("Attack")]
    [SerializeField] private Transform  Hand                = null; // 
    [SerializeField] private float      AttackCooltime      = 0.0f; // 
    [SerializeField] private float      AttackMotionTime    = 0.0f; // 
    [SerializeField] private float      AttackMotionAngle   = 0.0f; // 
    [SerializeField] private float      AttackMotionDamage  = 0.0f; // 
    [Header("Effect")]
    [SerializeField] private RippleManager  Ripple  = null; // 波
    [SerializeField] private TrailRenderer  Trail   = null; // 軌跡

    [Header("当たった時に表示するやつ")]
    [SerializeField] protected Text HitText;
    [SerializeField] protected string WriteText;

    // 内部パラメーター
    private Vector3     MovingDirection     = new Vector3(0, 0, 0); // 移動方向
    private Vector3     HorizontalVelocity  = new Vector3(0, 0, 0); // 横方向の移動速度 (x, 0, z)
    private Vector3     VerticalVelocity    = new Vector3(0, 0, 0); // 縦方向の移動速度 (0, y, 0)
    private Quaternion  HorizontalRotation  = Quaternion.identity;  // 横方向の回転
    private Quaternion  VerticalRotation    = Quaternion.identity;  // 縦方向の回転
    private float       JumpCooldown        = 0.0f;                 // ジャンプまでの時間
    private float       AttackCooldown      = 0.0f;                 // 攻撃までの時間
    private bool        IsJumpInput         = false;                // ジャンプ入力がされたか
    private bool        IsGround            = true;                 // 地面に足がついているか

    // 内部コンポーネント
    private GameInput       input;
    private Rigidbody       rb;
    private CapsuleCollider collision;
    private AudioSource audio;

    private void Awake()
    {
        // 内部コンポーネントの取得
        TryGetComponent<GameInput>(out input);
        TryGetComponent<Rigidbody>(out rb);
        TryGetComponent<CapsuleCollider>(out collision);
        TryGetComponent<AudioSource>(out audio);

        audio.mute = true;
        HitText.text = "";
    }

    private void Start()
    {
        // カーソルの固定
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 着地判定
        Ray ray = new Ray(Foot.position, Vector3.down);
        
        // ジャンプ入力
        if (JumpCooldown > 0.0f)
        {
            JumpCooldown -= Time.deltaTime;

            if (JumpCooldown < 0.0f)
            {
                JumpCooldown = 0.0f;
            }
        }
        else if (IsGround == false && Physics.SphereCast(ray, collision.radius, IsGroundDistance))
        {
            IsGround = true;
        }
        if (IsJumpInput == false && IsGround == true && input.Jump == true && JumpCooldown == 0.0f)
        {
            IsJumpInput = true;
        }

        // カメラ入力
        HorizontalAxis.m_InputAxisValue = input.Look.x;
        VerticalAxis.m_InputAxisValue   = input.Look.y;

        HorizontalAxis.Update(Time.deltaTime);
        VerticalAxis.Update(Time.deltaTime);

        // カメラ操作
        HorizontalRotation  = Quaternion.AngleAxis(HorizontalAxis.Value, Vector3.up);
        VerticalRotation    = Quaternion.AngleAxis(VerticalAxis.Value, Vector3.right);

        transform.rotation  = HorizontalRotation;
        Eye.localRotation   = VerticalRotation;

        // 攻撃
        if (AttackCooldown > 0.0f)
        {
            AttackCooldown -= Time.deltaTime;

            if (AttackCooldown < 0.0f)
            {
                //Trail.emitting = false;
                AttackCooldown = 0.0f;
            }
        }
        if (input.Attack == true && AttackCooldown == 0.0f)
        {
            //Trail.emitting = true;

            //if (Ripple != null)
            //    Ripple.StartRipple();

            AttackCooldown = AttackCooltime;

            if(attackCollision.frag)
            {
                Enemy enemy = attackCollision.hitGO.GetComponent<Enemy>();
                if(enemy)
                {
                    enemy.Hp -= 1;
                    Debug.Log("hit");
                }
            }
            else
            {
                
            }
        }

        //足音
        if (IsGround && HorizontalVelocity.sqrMagnitude > 0.9f)
        {
            audio.mute = false;
        }
        else
        {
            audio.mute = true;
            audio.time = 0.0f;
        }

        if (hp <= 0)
            HitText.text = WriteText;
    }

    private void LateUpdate()
    {
        
    }

    private void FixedUpdate()
    {
        // 移動
        MovingDirection = HorizontalRotation * new Vector3(input.Move.x, 0, input.Move.y);

        HorizontalVelocity *= MovingSpeedAttenuate;
        HorizontalVelocity += MovingDirection * MovingSpeedAccel;

        if (HorizontalVelocity.magnitude > MovingSpeedMax)
        {
            HorizontalVelocity = HorizontalVelocity.normalized * MovingSpeedMax;
        }
        if (HorizontalVelocity.magnitude >= MovingSpeedMin)
        {
            rb.position += (HorizontalVelocity + VerticalVelocity) * Time.fixedDeltaTime;
        }

        // ジャンプ
        if (IsJumpInput == true)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(JumpForce * Vector3.up, ForceMode.Impulse);

            JumpCooldown = JumpCooltime;

            IsJumpInput = false;
            IsGround = false;
        }
    }
}
