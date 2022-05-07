using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    
    [SerializeField] private float duration;
    [SerializeField] private int players;
    [SerializeField] private int playersAlive;
    [SerializeField] private RoundDisplay roundDisplay;
    [SerializeField] private MatchInfoDisplay matchInfoDisplay;
    [SerializeField] private PlayerKilledDisplay playerKilledDisplay;
    [SerializeField] private VictoryDisplay victoryDisplay;

    private bool isRunning;
    private float timeRemaining;

    private void Start() {
        roundDisplay.gameObject.SetActive(false);
        matchInfoDisplay.gameObject.SetActive(false);
    }

    public void StartMatch(){
        isRunning = true;
        timeRemaining = duration;
        roundDisplay.gameObject.SetActive(true);
        matchInfoDisplay.gameObject.SetActive(true);
        matchInfoDisplay.SetPlayers(playersAlive, players, false);
    }

    public void PlayerDied(){
        playersAlive --;
        matchInfoDisplay.SetPlayers(playersAlive, players, true);
        matchInfoDisplay.SetKills(players - playersAlive, true); // example display
        if(playersAlive == 1){
            roundDisplay.gameObject.SetActive(false);
            matchInfoDisplay.gameObject.SetActive(false);
            victoryDisplay.gameObject.SetActive(true);
            victoryDisplay.Display();
        }
        playerKilledDisplay.Display("DUMMY");
    }

    private void Update() {
        if(isRunning){
            timeRemaining -= Time.deltaTime;
            roundDisplay.SetTime(timeRemaining);
        }
    }

    
}
