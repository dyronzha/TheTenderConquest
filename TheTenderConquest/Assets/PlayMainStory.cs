using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMainStory : MonoBehaviour {

    public MovieTexture movTexture; //電影材質
    private AudioSource movAudio; //影片音軌
    GameObject loadScece;

    void Start()
    {
        GetComponent<Renderer>().material.mainTexture = movTexture;
        loadScece = GameObject.Find("LoadingScreen");
        movTexture.loop = false;
        movAudio = GetComponent<AudioSource>();
    }
    void Update()
    {
        movTexture.Play();
        movAudio.Play();
        if (Input.GetKeyDown(KeyCode.P))
        {
            //GetComponent<AudioSource>().Pause();
            movTexture.Pause();
            movAudio.Pause();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            movTexture.Stop();
            movAudio.Stop();
            loadScece.SendMessage("ChangeScene");
        }
        if (movTexture.isPlaying == false && Input.anyKeyDown)
        {
            loadScece.SendMessage("ChangeScene");
        }
    }
}
    
    


