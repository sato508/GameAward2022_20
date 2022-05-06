using System.Collections;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class IntroSequence : MonoBehaviour{
    
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineDollyCart dolly;
    [SerializeField] private Light sun;
    [SerializeField] private TextMeshProUGUI textMode;
    [SerializeField] private TextMeshProUGUI textReady;
    [SerializeField] private TextMeshProUGUI textGo;
    [SerializeField] private StarterAssets.FirstPersonController player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip soundIntro;
    [SerializeField] private AudioClip soundGo;

    private void Reset(){
        audioSource = GetComponent<AudioSource>();
    }

    private void Start(){
        player.enabled = false;
        StartCoroutine(Play());
    }

    private IEnumerator Play(){
        audioSource.PlayOneShot(soundIntro);
        DOVirtual.Float(0, 1, 3, v => dolly.m_Position = v).SetEase(Ease.OutCubic).WaitForCompletion();
        yield return new WaitForSeconds(1.5f);
        sun.DOIntensity(0, 1f);
        yield return new WaitForSeconds(1.5f);
        textMode.gameObject.SetActive(false);
        textReady.gameObject.SetActive(true);
        vcam.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        textReady.gameObject.SetActive(false);
        textGo.gameObject.SetActive(true);
        player.enabled = true;
        textGo.gameObject.transform.DOScale(Vector3.one, 1f).From(new Vector3(1.2f, 1.2f, 1.2f));
        matchManager.StartMatch();
        audioSource.PlayOneShot(soundGo);
        yield return new WaitForSeconds(0.5f);
        DOTween.Sequence()
        .Append(DOVirtual.Float(1, 0, 0.2f, p => textGo.alpha = Mathf.Round(p)).SetEase(Ease.Flash, 10))
        .Append(textGo.DOFade(0, 0.2f));
    }

}
