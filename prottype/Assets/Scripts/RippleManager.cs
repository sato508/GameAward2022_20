using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RippleManager : MonoBehaviour
{
    [Header("波紋のプレハブ")]
    [SerializeField]protected GameObject RipplePrefub;

    [Header("クールタイム最大")]
    [SerializeField] protected int CoolTimeMax = 60;//カウント数
    private int CoolTimeCount = 0;//フレームカウント
    

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        if (RipplePrefub == null)
            Debug.LogError("RipplePrefubがありません");

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
