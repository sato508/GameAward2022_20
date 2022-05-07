using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreatePlayerManagement : MonoBehaviour
{

    public MatchManager matchManager;
    public GameObject[] PlayerPrefub;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var v in PlayerPrefub)
        {
            var input = CreatePlayer(v);
            input.gameObject.GetComponent<FirstPersonController>().matchManager = matchManager;
            StartCoroutine(StayInputSystem(input));//アニメーション中入力出来ないようにする
        }
    }

    private PlayerInput CreatePlayer(GameObject player)
    {
        return PlayerInput.Instantiate(player);
    }

    private IEnumerator StayInputSystem(UnityEngine.InputSystem.PlayerInput input)
    {
        input.enabled = false;
        yield return new WaitForSeconds(5.0f);
        input.enabled = true;
    }
}
