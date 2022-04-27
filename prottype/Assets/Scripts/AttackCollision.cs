using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    public bool frag;
    public GameObject hitGO { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        frag = false;
    }

    void OnTriggerEnter(Collider other)
    {
        frag = true;
        hitGO = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        frag = false;
    }
}
