using UnityEngine;
using TMPro;
using DG.Tweening;

public static class TextAnimation{
    
    public static Tweener Flash(CanvasGroup container){
        return DOVirtual.Float(1, 0, 0.2f, p => container.alpha = Mathf.Round(p)).SetEase(Ease.Flash, 10);
    }
    
    public static Tweener Flash(TextMeshProUGUI text){
        return DOVirtual.Float(1, 0, 0.2f, p => text.alpha = Mathf.Round(p)).SetEase(Ease.Flash, 10);
    }
    
    public static Tweener RevealFromRandomChars(TextMeshProUGUI text, string value, float speed = 30){
        return DOVirtual.Float(0, value.Length, value.Length / speed, v => {
            text.text = value.Substring(0, (int)v) + CreateRandomString(value.Length - (int)v);
        });
    }

    private static string CreateRandomString(int len){
        char[] chars = new char[len];
        for(int i = 0;i < len;i ++){
        chars[i] = (char)Random.Range(0x20, 0x7f);
        }
        return new string(chars);
    }

}
