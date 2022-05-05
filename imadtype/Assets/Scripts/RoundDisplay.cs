using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundDisplay : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI textRound;
    [SerializeField] private TextMeshProUGUI textTime;

    public void SetTime(float secondsRemaining){
        textTime.text = $"{Mathf.FloorToInt(secondsRemaining).ToString("D2")}:{Mathf.FloorToInt((secondsRemaining % 1) * 100).ToString("D2")}";
    }

}
