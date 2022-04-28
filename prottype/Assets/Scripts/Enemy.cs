using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    enum Mode
    {
        Move,
        Stay,
        None
    }
    [SerializeField] private int hp;
    public int Hp
    {
        set
        {
            this.hp = value;
            if (this.hp < 0)
            {
                this.hp = 0;
            }
        }
        get
        {
            return this.hp;
        }
    }
    private bool attackFrag;
    [SerializeField] private int interval = 1;

    [SerializeField] private AttackCollision hitBox;

    [Header("Move")]
    [SerializeField] private float distance;
    [SerializeField] private float time;
    private float currentTime;
    private float oldDistance;

    [Header("Mode(途中で変更不可)")]
    [SerializeField] private Mode mode;

    [Header("必要な情報")]
    [SerializeField] private RippleManager Ripple;

    [Header("当たった時に表示するやつ")]
    [SerializeField] protected Text HitText;
    [SerializeField] protected string WriteText;

    private AudioSource audio;

    void Start()
    {
        oldDistance = 0;
        attackFrag = false;
        StartCoroutine(AttackAtRegularIntervals());
        TryGetComponent<AudioSource>(out audio);

        HitText.text = "";
    }

    void Update()
    {
        switch(mode)
        {
            case Mode.Move:
                UpdateAttack();
                UpdateMove();
                break;
            case Mode.Stay:
                UpdateAttack();
                break;
            case Mode.None:
                break;
            default:
                break;
        }

        if (hp <= 0)
            HitText.text = WriteText;
    }

    private void UpdateAttack()
    {
        if(hitBox.frag && attackFrag)
        {
            if(hitBox.hitGO && hitBox.hitGO.tag == "Player")
            {
                Player player = hitBox.hitGO.GetComponent<Player>();
                if(player)
                {
                    Debug.Log(hitBox.hitGO);
                    player.Hp -= 1;
                }
            }

            Ripple.StartRipple();

            attackFrag = false;
        }
    }

    private void UpdateMove()
    {
        currentTime += Time.deltaTime;
        if(currentTime > time) currentTime = 0;
        float _rate = currentTime / time;
        float _distance = distance * Mathf.Sin(2 * Mathf.PI * _rate);
        transform.Translate(new Vector3(oldDistance - _distance, 0, 0));
        oldDistance = _distance;
    }

    private IEnumerator AttackAtRegularIntervals()
    {
        while(true)
        {
            yield return new WaitForSeconds(interval);

            attackFrag = !attackFrag;
            Debug.Log("attack");
        }
    }
}

