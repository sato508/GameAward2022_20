using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int hp;

    public int Hp
    {
        set //値をhpに代入する
        {
            this.hp = value;
            if (this.hp < 0)
            {
                this.hp = 0;
            }
        }
        get //値を返す
        {
            return this.hp;
        }
    }
}
