using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class VictoryDisplay : MonoBehaviour{
    
    [SerializeField] private TextMeshProUGUI textVictory;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sound;

    private void Reset(){
        audioSource = GetComponent<AudioSource>();
    }

    public void Display(){
        audioSource.PlayOneShot(sound);
        DOTween.Sequence()
        .Append(textVictory.transform.DOScale(1, 3).From(2).SetEase(Ease.OutQuart))
        .Append(TextAnimation.Flash(textVictory))
        .Append(textVictory.DOFade(0, 0.2f))
        ;
    }

}
