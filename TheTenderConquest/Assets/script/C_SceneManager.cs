using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SceneManager : MonoBehaviour {
    public static C_SceneManager SceneManger;
    C_CameraFollow camera_follow;
    public int i_save_point;
    public GameObject  O_show_plate21, O_enemy,O_plate10, O_show_plate11,O_record3,O_step_ice,O_step_enemy,O_barrier;
    public GameObject O_camera, O_player;
    C_Player c_player;
    bool b_enemy_switch, b_look_up, b_look_down;
    public bool b_times_up, b_camera_busy, b_player_controll, b_look;
    int i_camera_move;
    float f_look_updown_time;
    Vector3 up_vec3, down_vec3, temp_vec3;
    // Use this for initialization
    void Awake () {
        SceneManger = this;
        i_save_point = i_camera_move = 0;
        O_camera = GameObject.Find("Main Camera");
        O_player = GameObject.Find("Player");
        camera_follow = O_camera.GetComponent<C_CameraFollow>();
        c_player = O_player.GetComponent<C_Player>();
        b_enemy_switch = b_times_up = b_camera_busy = b_look_up = b_look_down = b_look =   false;
        b_player_controll = true;
        f_look_updown_time = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
        LookUpDown();
        //if (O_enemy == null&&!b_enemy_switch) {
        //    O_plate10.SetActive(true);
        //    O_show_plate21.SetActive(true);
        //    O_show_plate11.SetActive(true);
        //    O_record3.SetActive(true);
        //    b_enemy_switch = true;
        //} 
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
        if (i_camera_move == 0)
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
        while (camera_follow.SetScreen2(target)) {
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
    IEnumerator CameraMoveTwoCoro(Vector3 target, bool reset)
    {
        if (reset)
        {
            Debug.Log("reset ");
            while (camera_follow.reset())
            {
                Debug.Log("reset " + target);
                yield return null;
            }
            UnFreezePlayer();
            b_camera_busy = false;
            Debug.Log("reset end " + b_camera_busy);
        }
        else {
            while (camera_follow.SetScreen2(target) && (b_look_up || b_look_down))
            {
                Debug.Log("move " + target);
                yield return null;
            }
            Debug.Log("move over " + (b_look_up || b_look_down) + " target " + target);
        }
        
    }


    void step_enemy() {
        //O_step_enemy.SendMessage("change",new Vector3(88.56f,11.28f,0.0f));
        //O_step_ice.SendMessage("change", new Vector3(80.0f,13.0f,0.5f));
        //O_camera.GetComponent<C_CameraFollow>().Invoke("reset",1.0f);
    }

    void LookUpDown() {
        //b_camera_busy 避免重複協程
        if (b_camera_busy) return;

        //分上看與下看紀錄，要都沒按的前提再按才會記錄
        if (!b_look_up && !b_look_down)
        {
            if (Input.GetKeyDown(KeyCode.W)) b_look_up = true;
            if (Input.GetKeyDown(KeyCode.S)) b_look_down = true;
            temp_vec3 = O_camera.transform.position;
            up_vec3 = temp_vec3 + new Vector3(0.0f, 5.0f, 0.0f);
            down_vec3 = temp_vec3 + new Vector3(0.0f,-5.0f,0.0f);
        }
        //放開紀錄        
        if (Input.GetKeyUp(KeyCode.W)) b_look_up = false;
        if (Input.GetKeyUp(KeyCode.S)) b_look_down = false;

        //紀錄長按時間超過一秒，開啟flag，else if裡是按下之後放開會reset的(static感覺會有bug，不知道誰給直，但是好好的)
        if ((b_look_up || b_look_down) && !b_look)
        {
            f_look_updown_time += Time.deltaTime;
            Debug.Log("push look " + f_look_updown_time);
        }
        else if((!b_look_up && !b_look_down)&& b_look) {
            Debug.Log("begin reset" + b_look_down  + " " + b_look_up);
            b_look = false;
            b_camera_busy = true;
            StartCoroutine(CameraMoveTwoCoro(temp_vec3, true));
        }
        //大於1秒之後開時往上或往下
        if (f_look_updown_time > 1.0f) {
            b_look = true;
            FreezePlayer();
            f_look_updown_time = 0.0f;
            Debug.Log("down + up " + b_look_down + " " + b_look_up);
            Vector3 temp = new Vector3(0,0,0);
            if (b_look_up) temp = up_vec3;
            if (b_look_down) temp = down_vec3;
            StartCoroutine(CameraMoveTwoCoro(temp, false));
        }
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
