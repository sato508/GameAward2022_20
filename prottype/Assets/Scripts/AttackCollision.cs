using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackCollision : MonoBehaviour
{
    public bool frag;
    public GameObject hitGO { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        frag = false;
    }

    void OnTriggerStay(Collider other)
    {
        if(transform.parent.gameObject != other.gameObject && other.gameObject.CompareTag("Player"))
        {
            frag = true;
            hitGO = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        frag = false;
    }
}
