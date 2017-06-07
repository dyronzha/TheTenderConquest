using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SceneManager : MonoBehaviour {
    public static C_SceneManager SceneManger;
    C_CameraFollow camera_follow;
    public int i_save_point;
    public GameObject  O_show_plate21, O_enemy,O_plate10, O_show_plate11,O_record3,O_step_ice,O_step_enemy,O_barrier;
    GameObject O_camera, O_player;
    C_Player c_player;
    bool b_enemy_switch;
    public bool b_times_up, b_camera_busy, b_player_controll;
    int i_camera_move;
    // Use this for initialization
    void Awake () {
        SceneManger = this;
        i_save_point = i_camera_move = 0;
        O_camera = GameObject.Find("Main Camera");
        O_player = GameObject.Find("Player");
        camera_follow = O_camera.GetComponent<C_CameraFollow>();
        c_player = O_player.GetComponent<C_Player>();
        b_enemy_switch = b_times_up = b_camera_busy = false;
        b_player_controll = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (O_enemy == null&&!b_enemy_switch) {
            O_plate10.SetActive(true);
            O_show_plate21.SetActive(true);
            O_show_plate11.SetActive(true);
            O_record3.SetActive(true);
            b_enemy_switch = true;
        } 
	}

    public void ChangeSavePoint() {
        i_save_point++;
        if (i_save_point == 1)
        {
            FreezePlayer();
            if (!b_camera_busy) StartCoroutine(CameraMoveCoro(new Vector3(51.2f, 20.4f, -10.0f), 2.0f));
            i_camera_move++;
            O_barrier.GetComponent<C_Barrier>().Invoke("Collapse",2.5f);
        }
        else if (i_save_point == 3) {
           
        }
    }

    public void OnDetect() {
       // O_camera.SendMessage("SetScreen", new Vector3(89.0f, 16.0f, -10.0f));
        //camera_follow.SetScreen(new Vector3(89.0f, 16.0f, -10.0f),1.0f);
       this.Invoke("step_enemy", 1.0f);
        if (i_camera_move == 1)
        {
            if (!b_camera_busy) StartCoroutine(CameraMoveCoro(new Vector3(89.0f, 16.0f, -10.0f), 2.0f));
            i_camera_move++;
        }
        else if (i_camera_move==2) {
            
        }
        
    }

    IEnumerator CameraMoveCoro(Vector3 target, float wait_time)
    {
        b_camera_busy = true;
        while (camera_follow.SetScreen2(target, wait_time)) {
            yield return null;
        }
        yield return new WaitForSeconds(wait_time);
        O_camera.GetComponent<C_CameraFollow>().ChMod();
        while (camera_follow.reset()) {
            yield return null;
        }
        b_camera_busy = false;
        UnFreezePlayer();
    }


    void step_enemy() {
        //O_step_enemy.SendMessage("change",new Vector3(88.56f,11.28f,0.0f));
        //O_step_ice.SendMessage("change", new Vector3(80.0f,13.0f,0.5f));
        //O_camera.GetComponent<C_CameraFollow>().Invoke("reset",1.0f);
    }

    void FreezePlayer() {
        c_player.FreezeControl();
    }
    void UnFreezePlayer() {
        c_player.UnFreezeControl();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

}
