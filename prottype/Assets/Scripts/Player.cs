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
    // �O���p�����[�^�[
    [Header("Hp")]
    [SerializeField] AttackCollision attackCollision;
    [SerializeField] private int hp;
    public int Hp
    {
        set //�l��hp�ɑ������
        {
            this.hp = Math.Max(this.hp - value, 0);
        }
        get //�l��Ԃ�
        {
            return this.hp;
        }
    }
    [Header("Movement")]
    [SerializeField] private float      MovingSpeedAttenuate    = 0.0f; // �ړ����x�̌����l
    [SerializeField] private float      MovingSpeedAccel        = 0.0f; // �ړ����x�̉����l
    [SerializeField] private float      MovingSpeedMin          = 0.0f; // �ړ����x�̍Œ�l
    [SerializeField] private float      MovingSpeedMax          = 0.0f; // �ړ����x�̍ō��l
    [Header("Jump")]
    [SerializeField] private Transform  Foot                = null; // �����̊�_
    [SerializeField] private float      IsGroundDistance    = 0.0f; // ���n�Ɣ��f���鋗��
    [SerializeField] private float      JumpForce           = 0.0f; // �W�����v�����
    [SerializeField] private float      JumpCooltime        = 0.0f; // �W�����v����Ԋu
    [Header("Camera")]
    [SerializeField] private Transform  Eye                 = null;             // �J�����̊�_
    [SerializeField] private AxisState  VerticalAxis        = new AxisState();  // �c���̐ݒ�
    [SerializeField] private AxisState  HorizontalAxis      = new AxisState();  // �����̐ݒ�
    [Header("Attack")]
    [SerializeField] private Transform  Hand                = null; // 
    [SerializeField] private float      AttackCooltime      = 0.0f; // 
    [SerializeField] private float      AttackMotionTime    = 0.0f; // 
    [SerializeField] private float      AttackMotionAngle   = 0.0f; // 
    [SerializeField] private float      AttackMotionDamage  = 0.0f; // 
    [Header("Effect")]
    [SerializeField] private RippleManager  Ripple  = null; // �g
    [SerializeField] private TrailRenderer  Trail   = null; // �O��

    [Header("�����������ɕ\��������")]
    [SerializeField] protected Text HitText;
    [SerializeField] protected string WriteText;

    // �����p�����[�^�[
    private Vector3     MovingDirection     = new Vector3(0, 0, 0); // �ړ�����
    private Vector3     HorizontalVelocity  = new Vector3(0, 0, 0); // �������̈ړ����x (x, 0, z)
    private Vector3     VerticalVelocity    = new Vector3(0, 0, 0); // �c�����̈ړ����x (0, y, 0)
    private Quaternion  HorizontalRotation  = Quaternion.identity;  // �������̉�]
    private Quaternion  VerticalRotation    = Quaternion.identity;  // �c�����̉�]
    private float       JumpCooldown        = 0.0f;                 // �W�����v�܂ł̎���
    private float       AttackCooldown      = 0.0f;                 // �U���܂ł̎���
    private bool        IsJumpInput         = false;                // �W�����v���͂����ꂽ��
    private bool        IsGround            = true;                 // �n�ʂɑ������Ă��邩

    // �����R���|�[�l���g
    private GameInput       input;
    private Rigidbody       rb;
    private CapsuleCollider collision;
    private AudioSource audio;

    private void Awake()
    {
        // �����R���|�[�l���g�̎擾
        TryGetComponent<GameInput>(out input);
        TryGetComponent<Rigidbody>(out rb);
        TryGetComponent<CapsuleCollider>(out collision);
        TryGetComponent<AudioSource>(out audio);

        audio.mute = true;
        HitText.text = "";
    }

    private void Start()
    {
        // �J�[�\���̌Œ�
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // ���n����
        Ray ray = new Ray(Foot.position, Vector3.down);
        
        // �W�����v����
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

        // �J��������
        HorizontalAxis.m_InputAxisValue = input.Look.x;
        VerticalAxis.m_InputAxisValue   = input.Look.y;

        HorizontalAxis.Update(Time.deltaTime);
        VerticalAxis.Update(Time.deltaTime);

        // �J��������
        HorizontalRotation  = Quaternion.AngleAxis(HorizontalAxis.Value, Vector3.up);
        VerticalRotation    = Quaternion.AngleAxis(VerticalAxis.Value, Vector3.right);

        transform.rotation  = HorizontalRotation;
        Eye.localRotation   = VerticalRotation;

        // �U��
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

        //����
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
        // �ړ�
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

        // �W�����v
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
