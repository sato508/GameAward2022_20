using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ripple : MonoBehaviour
{
    Material        material;
    MeshRenderer    meshRenderer;

    private void Awake()
    {
        TryGetComponent<MeshRenderer>(out meshRenderer);
    }

    private void Start()
    {
        material = meshRenderer.material;
        material.EnableKeyword("Start");
        if (material.IsKeywordEnabled("Start"))
        {
            Debug.Log("Start is Enabled");
        }

        Step();
    }

    public void Step()
    {
        if (material.HasProperty("Start"))
        {
            material.SetFloat("Start", Time.time);
            Debug.Log("Step!" + material.GetFloat("Start").ToString());
        }
    }
}
