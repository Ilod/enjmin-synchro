using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAI : MonoBehaviour {
    public float speed = 3;
    public PlayerAI player;

	// Use this for initialization
	void Start ()
    {
        gameObject.layer = LayerMask.NameToLayer($"Arrow{player.index}");
    }
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (!player.bounds.Contains(gameObject.transform.position))
        {
            Destroy(gameObject);
        }
    }
}
