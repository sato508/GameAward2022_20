using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoPlayer : MonoBehaviour
{
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

    void Update()
    {
        if(hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
