using System.Collections;
using UnityEngine;
using StarterAssets;

public class SwordCollision : MonoBehaviour
{

    [SerializeField] private GameObject self;

    private void OnTriggerEnter(Collider other) {
        if(self == other.gameObject) return;
        if(other.gameObject.TryGetComponent<FirstPersonController>(out var p)){
            p.Die();
        }
    }

}
