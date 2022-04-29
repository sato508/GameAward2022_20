using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTelepoter : MonoBehaviour
{
    public GameObject Enemy;

    public Transform[] TeleportPosition;

    // Start is called before the first frame update
    void Start()
    {
        int random = Random.Range(0, TeleportPosition.Length - 1);
        Transform enemytransform = Enemy.transform;
        enemytransform.position =
            new Vector3(TeleportPosition[random].position.x, 6.0f, TeleportPosition[random].position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
