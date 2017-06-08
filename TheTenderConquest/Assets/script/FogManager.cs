using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogManager : MonoBehaviour {

    public GameObject fog;
    public GameObject FogSystem;
    public GameObject copyfog;
    bool upperFog;
    // Use this for initialization
    void Start () {
        InvokeRepeating("FogMaker",1.5f,6);
        upperFog = true;
    }
    void FogMaker()
    {   
        if (upperFog)
        {
            copyfog = Instantiate(fog, new Vector3(0, Random.Range(8f, 20f), 0), Quaternion.identity);
            copyfog.transform.parent = FogSystem.transform;
            copyfog.transform.localScale = new Vector3(fog.transform.localScale.x, fog.transform.localScale.y, fog.transform.localScale.z);
            upperFog = false;
        }
        else
        {
            copyfog = Instantiate(fog, transform.position, Quaternion.identity);
            copyfog.transform.parent = FogSystem.transform;
            copyfog.transform.localScale = new Vector3(fog.transform.localScale.x, fog.transform.localScale.y, fog.transform.localScale.z);
            upperFog = true;
        } 
    }
    
}
