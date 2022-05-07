using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerKilledDisplay : MonoBehaviour
{

    [SerializeField] private CanvasGroup container;
    [SerializeField] private TextMeshProUGUI textPlayer;

    public void Display(string playerName){
        textPlayer.gameObject.SetActive(true);
        container.alpha = 1;
        TextAnimation.RevealFromRandomChars(textPlayer, $"KILLED {playerName}", 20);
        StartCoroutine(HideLater());
    }

    private IEnumerator HideLater(){
        yield return new WaitForSeconds(2);
        yield return TextAnimation.Flash(container).WaitForCompletion();
        yield return container.DOFade(0, 0.2f);
        textPlayer.gameObject.SetActive(false);
    }
    
}
