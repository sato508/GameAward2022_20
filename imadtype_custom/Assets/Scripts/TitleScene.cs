using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TitleScene : MonoBehaviour{

    [SerializeField] private Button buttonPlay;
    [SerializeField] private CanvasGroup fade;

    private void Start(){
        buttonPlay.onClick.AddListener(() => {
            fade.gameObject.SetActive(true);
            fade.DOFade(1, 0.2f).From(0).OnComplete(() => {
                SceneManager.LoadScene("Playground");
            });
        });
    }

}
