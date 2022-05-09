/*------------------------------------------------------------
 * 
 *  [TextBreakEvent.cs]
 *  Author : 出合翔太
 *  タイトルシーンのイベント情報を管理する  
 * 
 * -----------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

[RequireComponent(typeof(TMP_Text))]
public class TitleTextEvent : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    private Action[] _TitleEvent;

    /// <summary> 
    /// クリック時のイベントハンドラー 
    /// </summary>
    [SerializeField]
    private UnityEvent<string, string, int, int> _OnClickLink;

    /// <summary> 
    /// TextMeshProのテキスト 
    /// </summary>
    private TMP_Text _Text;

    /// <summary>
    /// ゲームモード選択用画面のゲームオブジェクト
    /// </summary>
    private GameObject _GameModeImage;

    /// <summary>
    /// イベントの状態
    /// </summary>
    private int _EventState = 0;

    // Start is called before the first frame update
    void Start()
    {
        _Text = GetComponent<TMP_Text>();
        
        // ゲームモード選択用背景の基準位置を示すゲームオブジェクトを取得する
        _GameModeImage = GameObject.Find("GameMode_Image");

        // イベントを発生させる順番に関数を登録する        
        _TitleEvent = new Action[] { GameModeSelectReset, PlayGameStartAnimation, GameModeSelect };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 次に進む
    /// </summary>
    public void NextEvent()
    {
        _EventState++;
        //　要素を超えたときの処理
        if (_TitleEvent.Length -1 < _EventState)
        {            
            _EventState = _TitleEvent.Length - 1;
            return;
        }
        _TitleEvent[_EventState]();

    }

    /// <summary>
    /// 戻る
    /// </summary>
    public void RedoEvent()
    {
        _EventState--;
        //　要素を超えたときの処理
        if (_EventState < 0)
        {
            _EventState = 0;
            return;
        }
        _TitleEvent[_EventState]();
    }

    /// <summary>
    /// GameStart時に文字が割れるアニメーションを再生
    /// </summary>
    private void PlayGameStartAnimation()
    {
        TMP_LinkInfo linkInfo = _Text.textInfo.linkInfo[0];
        // アニメーションを再生する
        _OnClickLink.Invoke(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkInfo.linkTextfirstCharacterIndex, 0);
    }

    /// <summary>
    /// ゲームモード選択中の画面を表示する
    /// </summary>
    private void GameModeSelect()
    {        
        _GameModeImage.transform.position = new Vector3(960.0f, 540.0f, 0.0f);        
    }

    private void GameModeSelectReset()
    {
        _GameModeImage.transform.position = new Vector3(0.0f, 1080.0f, 0.0f);
    }
}
