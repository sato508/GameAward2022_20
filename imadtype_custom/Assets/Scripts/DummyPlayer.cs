using System.Collections;
using UnityEngine;
using StarterAssets;
using Random = UnityEngine.Random;

[RequireComponent(typeof(StarterAssetsInputs))]
public class DummyPlayer : MonoBehaviour
{
    
    private StarterAssetsInputs _input;

    private void Start() {
        _input = GetComponent<StarterAssetsInputs>();
        StartCoroutine(Run());
    }

    private IEnumerator Run(){
        while(true){
            yield return new WaitForSeconds(Random.Range(4, 10));
            _input.AttackInput(true);
        }
    }

}
