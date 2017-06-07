using UnityEngine;
using System.Collections;

public class C_UIHP : MonoBehaviour {
    Transform[] hp_imgs = new Transform[5];

	// Use this for initialization
	void Awake () {
        for (int i = 0; i<5; i++) {
            hp_imgs[i] = transform.GetChild(i);
        }
	}
	
	// Update is called once per frame
	void Update () {

	}
    public void PresentHp(int hp) {
        if (hp < 0) return;
        for (int i=0; i<5; i++) {
            if (i <= hp-1) hp_imgs[i].gameObject.SetActive(true);
            else hp_imgs[i].gameObject.SetActive(false);
        }
    }
}
