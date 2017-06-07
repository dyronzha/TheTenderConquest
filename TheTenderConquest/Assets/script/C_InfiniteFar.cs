using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_InfiniteFar : MonoBehaviour {
    GameObject player, O_camera;
    C_Player c_player;
    C_CameraFollow c_camera;
	// Use this for initialization
	void Awake () {
        player = GameObject.Find("Player");
        c_player = player.GetComponent<C_Player>();
        transform.position = O_camera.transform.position + new Vector3(4.8f, 1.9f, 0.0f);
        O_camera = GameObject.Find("Main Camera");
        c_camera = O_camera.GetComponent<C_CameraFollow>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(O_camera.transform.position.x + 4.8f, transform.position.y, transform.position.z); 
        if(c_camera.y_axis_change) transform.position = new Vector3(transform.position.x, O_camera.transform.position.y+1.9f, transform.position.z);
    }
}
