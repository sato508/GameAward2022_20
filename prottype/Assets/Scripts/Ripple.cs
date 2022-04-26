using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripple : MonoBehaviour
{
    //[SerializeField] protected Shader shader;

    private Material material;
    private MeshRenderer meshRenderer;

    private float TimeCount = 0.0f;

    public bool IsPlay
    {
        get { return TimeCount <= material.GetFloat("Vector1_4fddaf0c35ff4e3daa5d2eb61a6dde33"); }
    }

    private void Awake()
    {
        TryGetComponent<MeshRenderer>(out meshRenderer);
        material = meshRenderer.material;
    }

    void Update()
    {
        //ƒ^ƒCƒ€‚ªDuration‚ð’´‚¦‚Ä‚½‚çŽ©–Å
        if (TimeCount >= material.GetFloat("Vector1_3c0b91f88a5546f8be4b27c02f135b66"))
            Destroy(this.gameObject);


        material.SetFloat("Vector1_4fddaf0c35ff4e3daa5d2eb61a6dde33", TimeCount);
        TimeCount += Time.deltaTime;
    }
}
