using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_VirtualPilar : MonoBehaviour {
    public GameObject virtual_player;
    GameObject O_vplayer;
    Vector3 virtual_position;
	// Use this for initialization
	void Awake () {
        virtual_position = this.transform.GetChild(0).transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (O_vplayer != null) return;
            O_vplayer = Instantiate(virtual_player, virtual_position, Quaternion.Euler(0f, 0f, 0f));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(O_vplayer);
    }
}
