/*------------------------------------------------------------
 * 
 *  [TextBreakEvent.cs]
 *  Author : �o���đ�
 *  
 *  <Link>�@�\������
 *  <Link>�^�O�ň͂����������N���b�N����ƃA�j���[�V�������Đ������
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
    /// �N���b�N���̃C�x���g�n���h���[ 
    /// </summary>
    [SerializeField]
    private UnityEvent<string, string, int, int> _OnClickLink;

    /// <summary> 
    /// TextMeshPro�̃e�L�X�g 
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
        // �N���b�N������s��
        if (_count > 500)
        {                          
            TMP_LinkInfo linkInfo = _Text.textInfo.linkInfo[0];
               
            // �A�j���[�V�������Đ�����
            _OnClickLink.Invoke(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkInfo.linkTextfirstCharacterIndex, 0);    
        }
    }
}
