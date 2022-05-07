using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Light))]
public class LightFade : MonoBehaviour
{
    
    [SerializeField] private float duration;
    [SerializeField] private GameObject destroyOnComplete;

    void Start()
    {
        var tween = GetComponent<Light>().DOIntensity(0, duration).SetEase(Ease.InQuart).SetLink(gameObject);
        if(destroyOnComplete != null){
            Destroy(destroyOnComplete, duration);
        }
    }

}
