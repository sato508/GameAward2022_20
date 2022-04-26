using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RippleManager : MonoBehaviour
{
    [Header("�g��̃v���n�u")]
    [SerializeField]protected GameObject RipplePrefub;

    [Header("�N�[���^�C���ő�")]
    [SerializeField] protected int CoolTimeMax = 60;//�J�E���g��
    private int CoolTimeCount = 0;//�t���[���J�E���g
    

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        if (RipplePrefub == null)
            Debug.LogError("RipplePrefub������܂���");

        CoolTimeCount = CoolTimeMax;
    }

    void FixedUpdate()
    {
        if (CoolTimeCount >= CoolTimeMax && UnityEngine.InputSystem.Keyboard.current.pKey.isPressed)
        {
            StartRipple();
            CoolTimeCount = 0;
        }

        CoolTimeCount++;
    }

    void StartRipple()
    {
        Instantiate(RipplePrefub, this.transform);
    }
    

}
