using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Barrier : MonoBehaviour {
    Transform[] barrier = new Transform[13];
	// Use this for initialization
	void Awake () {
        for (int i = 0; i < 13; i++) {
            barrier[i] = transform.GetChild(i);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Collapse() {
        for (int i = 0; i < 13; i++)
        {
            Rigidbody2D body;
            body = barrier[i].GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.velocity = new Vector2(3.0f,0.0f);
        }
    }
}
