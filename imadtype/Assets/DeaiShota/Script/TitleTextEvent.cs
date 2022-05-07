/*------------------------------------------------------------
 * 
 *  [TextBreakEvent.cs]
 *  Author : 出合翔太
 *  
 *  <Link>機能を実装
 *  <Link>タグで囲った文字をクリックするとアニメーションが再生される
 * 
 * -----------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TMP_Text))]
//[RequireComponent(typeof(PlayerInput))]
public class TitleTextEvent : MonoBehaviour
{
    /// <summary> 
    /// クリック時のイベントハンドラー 
    /// </summary>
    [SerializeField]
    private UnityEvent<string, string, int, int> _OnClickLink;

    /// <summary> 
    /// TextMeshProのテキスト 
    /// </summary>
    private TMP_Text _Text;
    

    int _count = 0;

    private void Awake()
    {
        _Text = GetComponent<TMP_Text>();       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _count++;
        // クリック判定を行う
        if (_count > 500)
        {                          
            TMP_LinkInfo linkInfo = _Text.textInfo.linkInfo[0];
               
            // アニメーションを再生する
            _OnClickLink.Invoke(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkInfo.linkTextfirstCharacterIndex, 0);    
        }
    }
}
