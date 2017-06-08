using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogMovement : MonoBehaviour {
    float FogSpeed = 0.03f;
    public GameObject Fog;
	void Update () {
        this.transform.position += new Vector3(FogSpeed, 0,0);
        //if (this.transform.position.x >= 230f)
        //{
        //    Destroy(this);
        // }
        InvokeRepeating("Destroy",45f,0);
    }
    private void Destroy()
    {
        Destroy(Fog);
    }

}

