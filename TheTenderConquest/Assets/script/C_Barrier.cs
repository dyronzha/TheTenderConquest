using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Barrier : MonoBehaviour {
    Transform[] barrier = new Transform[13];
    public AudioClip crumble_sound;
    public AudioSource audio_source;
    // Use this for initialization
    void Awake () {
        for (int i = 0; i < 13; i++) {
            barrier[i] = transform.GetChild(i);
        }
        audio_source = transform.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Collapse() {
        audio_source.PlayOneShot(crumble_sound); 
        for (int i = 0; i < 13; i++)
        {
            Rigidbody2D body;
            body = barrier[i].GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.velocity = new Vector2(3.0f,0.0f);
        }
    }
}
