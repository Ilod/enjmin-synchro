using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAI : MonoBehaviour {
    public PlayerAI player;

	// Use this for initialization
	void Start ()
    {
        gameObject.layer = LayerMask.NameToLayer($"Arrow{player.index}");
        GetComponent<SpriteRenderer>().color = player.color;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
