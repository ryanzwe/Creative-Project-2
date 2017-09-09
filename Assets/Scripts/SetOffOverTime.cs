using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOffOverTime : MonoBehaviour {

	// Use this for initialization
	

    private IEnumerator OnBecameVisible()
    {
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration);
        gameObject.SetActive(false);
    }
}
