using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_FarBackground : MonoBehaviour {

    [System.Serializable]
    public struct child_far_scene {
        public bool b_infinite_far;
        public GameObject O_far_scene;
        public float limit_r, limit_l, limit_t, limit_b;
        [System.NonSerialized]
        public float bk_f_x, bk_f_y,trans_value_x, trans_value_y;
    }
    public child_far_scene[] far_scenes;

    float bk_f_x,bk_f_y;
    GameObject camera;
    C_CameraFollow c_camera;
    float camera_r, camera_l;
    float camera_t, camera_b;
    Vector2 temp_vec2;
    float temp_x, temp_y;
    // Use this for initialization
    void Awake () {
        camera = GameObject.Find("Main Camera");
        c_camera = camera.GetComponent<C_CameraFollow>();
        //limit_l = 37.4f; limit_r = 64.5f;
        camera_l = 20.4f; camera_r = 92.2f;
        //limit_t = 25.2f; limit_b = 17.6f;
        camera_t = 27.0f; camera_b = 16.0f;

        for (int i = 0; i< far_scenes.Length; i++) {
            far_scenes[i].trans_value_x = (far_scenes[i].limit_r - far_scenes[i].limit_l) / (camera_r - camera_l);
            far_scenes[i].trans_value_y = (far_scenes[i].limit_t - far_scenes[i].limit_b) / (camera_t - camera_b);
            far_scenes[i].bk_f_x = far_scenes[i].limit_l;
            far_scenes[i].bk_f_y = far_scenes[i].limit_b;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (C_SceneManager.SceneManger.b_camera_busy || C_SceneManager.SceneManger.b_look) FarBackgroundMove();
    }
    void FixedUpdate () {
        if (!C_SceneManager.SceneManger.b_camera_busy && !C_SceneManager.SceneManger.b_look) FarBackgroundMove();
	}

    void FarBackgroundMove()
    {
        for (int i = 0; i< far_scenes.Length; i++) {
            if (camera.transform.position.x <= camera_l) far_scenes[i].bk_f_x = far_scenes[i].limit_l;
            else if (camera.transform.position.x >= camera_r) far_scenes[i].bk_f_x = far_scenes[i].limit_r;
            else
            {
                if (!far_scenes[i].b_infinite_far) far_scenes[i].bk_f_x = far_scenes[i].limit_l + (camera.transform.position.x - camera_l) * far_scenes[i].trans_value_x;
                else {
                    if (c_camera._b_right && !C_SceneManager.SceneManger.b_camera_busy)
                    {
                        Debug.Log("follow far background");
                        temp_x = far_scenes[i].O_far_scene.transform.position.x- C_SceneManager.SceneManger.O_player.transform.position.x;
                        far_scenes[i].bk_f_x = far_scenes[i].limit_l + (camera.transform.position.x - camera_l) * far_scenes[i].trans_value_x;
                    }
                    else {
                        Debug.Log("static far background");
                        far_scenes[i].bk_f_x = C_SceneManager.SceneManger.O_player.transform.position.x + temp_x - transform.position.x;
                    }
                }
                //Debug.Log(this.gameObject.name + "  " + (camera.transform.position.x - camera_l) * trans_value);
            }
            if (camera.transform.position.y <= camera_b) far_scenes[i].bk_f_y = far_scenes[i].limit_b;
            else if (camera.transform.position.y >= camera_t) far_scenes[i].bk_f_y = far_scenes[i].limit_t;
            else
            {
                if (!far_scenes[i].b_infinite_far) far_scenes[i].bk_f_y = far_scenes[i].limit_b + (camera.transform.position.y - camera_b) * far_scenes[i].trans_value_y;
                else {
                    if(!C_SceneManager.SceneManger.b_camera_busy && !C_SceneManager.SceneManger.b_look)
                    {
                        temp_y = far_scenes[i].O_far_scene.transform.position.y - C_SceneManager.SceneManger.O_player.transform.position.y;
                        far_scenes[i].bk_f_y = far_scenes[i].limit_b + (camera.transform.position.y - camera_b) * far_scenes[i].trans_value_y;
                    }
                    else {
                        far_scenes[i].bk_f_y = C_SceneManager.SceneManger.O_player.transform.position.y + temp_y - transform.position.y;
                    }
                }
                //Debug.Log(this.gameObject.name + "  " + (camera.transform.position.x - camera_l) * trans_value);
                //if (camera.transform.localScale.y > 0) bk_f_y = limit_b + (camera.transform.position.y - camera_b) * trans_value_y;
            }
            far_scenes[i].O_far_scene.transform.localPosition = new Vector3(far_scenes[i].bk_f_x, far_scenes[i].bk_f_y, transform.position.z);
        }
    }

}
