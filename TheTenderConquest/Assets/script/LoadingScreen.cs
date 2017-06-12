using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {
    public string levelToLoad;
    public GameObject background;
    public GameObject loadingtext;
    public GameObject progressbar;
    public GameObject StartButton;

    private int loadProgress = 0;

    void Start () {

        background.SetActive(false);
        loadingtext.SetActive(false);
        progressbar.SetActive(false);
        StartButton.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(DisplayLoadingScreen(levelToLoad));
        }
	}
    public void ChangeScene()
    {
        StartButton.SetActive(false);
        StartCoroutine(load());
        StartCoroutine(DisplayLoadingScreen(levelToLoad));     
    }
    IEnumerator load()
    {
        yield return new WaitForSeconds(1.5f); 
    }

    IEnumerator DisplayLoadingScreen(string level)
    {
        background.SetActive(true);
        loadingtext.SetActive(true);
        progressbar.SetActive(true);

        progressbar.transform.localScale = new Vector3(loadProgress, progressbar.transform.position.y, progressbar.transform.position.z);
        loadingtext.GetComponent<GUIText>().text = "Loading" + loadProgress + "%";

        AsyncOperation async = Application.LoadLevelAsync(level);
        while (!async.isDone)
        {
            loadProgress = (int)(async.progress * 100);
            loadingtext.GetComponent<GUIText>().text = "Loading" + loadProgress + "%";
            progressbar.transform.localScale = new Vector3(async.progress*4, 0.05f, progressbar.transform.localScale.z);
            yield return null;
        }
        
    }
}
