/*------------------------------------------------------------
 * 
 *  [TextBreakEvent.cs]
 *  Author : �o���đ�
 *  �^�C�g���V�[���̃C�x���g�����Ǘ�����  
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
    /// �N���b�N���̃C�x���g�n���h���[ 
    /// </summary>
    [SerializeField]
    private UnityEvent<string, string, int, int> _OnClickLink;

    /// <summary> 
    /// TextMeshPro�̃e�L�X�g 
    /// </summary>
    private TMP_Text _Text;

    /// <summary>
    /// �Q�[�����[�h�I��p��ʂ̃Q�[���I�u�W�F�N�g
    /// </summary>
    private GameObject _GameModeImage;

    /// <summary>
    /// �C�x���g�̏��
    /// </summary>
    private int _EventState = 0;

    // Start is called before the first frame update
    void Start()
    {
        _Text = GetComponent<TMP_Text>();
        
        // �Q�[�����[�h�I��p�w�i�̊�ʒu�������Q�[���I�u�W�F�N�g���擾����
        _GameModeImage = GameObject.Find("GameMode_Image");

        // �C�x���g�𔭐������鏇�ԂɊ֐���o�^����        
        _TitleEvent = new Action[] { GameModeSelectReset, PlayGameStartAnimation, GameModeSelect };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ���ɐi��
    /// </summary>
    public void NextEvent()
    {
        _EventState++;
        //�@�v�f�𒴂����Ƃ��̏���
        if (_TitleEvent.Length -1 < _EventState)
        {            
            _EventState = _TitleEvent.Length - 1;
            return;
        }
        _TitleEvent[_EventState]();

    }

    /// <summary>
    /// �߂�
    /// </summary>
    public void RedoEvent()
    {
        _EventState--;
        //�@�v�f�𒴂����Ƃ��̏���
        if (_EventState < 0)
        {
            _EventState = 0;
            return;
        }
        _TitleEvent[_EventState]();
    }

    /// <summary>
    /// GameStart���ɕ����������A�j���[�V�������Đ�
    /// </summary>
    private void PlayGameStartAnimation()
    {
        TMP_LinkInfo linkInfo = _Text.textInfo.linkInfo[0];
        // �A�j���[�V�������Đ�����
        _OnClickLink.Invoke(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkInfo.linkTextfirstCharacterIndex, 0);
    }

    /// <summary>
    /// �Q�[�����[�h�I�𒆂̉�ʂ�\������
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
