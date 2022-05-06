using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MatchInfoDisplay : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI textPlayers;
    [SerializeField] private TextMeshProUGUI textKills;

    public void SetPlayers(int alive, int total, bool flash){
        textPlayers.text = $"{alive.ToString()} / {total.ToString()} ALIVE";
        if(flash) TextAnimation.Flash(textPlayers);
    }

    public void SetKills(int kills, bool flash){
        textKills.text = $"{kills.ToString()} KILLS";
        if(flash) TextAnimation.Flash(textKills);
    }

}
