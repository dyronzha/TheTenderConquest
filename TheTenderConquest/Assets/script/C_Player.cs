using UnityEngine;
using System.Collections;
using Spine.Unity;

public class C_Player : MonoBehaviour {

    //存檔變數
    public bool b_is_save = false;

     bool b_player_controll = true;

    //技能相關變數宣告
    public GameObject O_mirror = null;
    public GameObject O_virtualplayer = null;
    public bool b_magic = false;
    public bool b_upside = false;
    public bool b_use_skill = false;
    public float f_shoot = 0f;
    //GameObject O_tempmirror;
    GameObject O_tempvirtuall;
    float skill_time = 0.0f;
    bool skill_ani_use = false;
    RaycastHit2D hit_cilling_ray ;
    RaycastHit2D hit_ground_ray;
    Transform AOE_col,Stick_col;
    bool b_AOE_has, b_hit_away;
    float shoot_ani_time;
    

    //玩家物件相關變數
    GameObject O_camera;
    public GameObject O_bullet = null;
    Rigidbody2D player_rig = null;
    Animator player_spine_animator = null;
    Animator player_animator = null;
    public bool b_isground = true;
    private Transform t_ground_check,t_ground_check2;
    private Transform t_pic;
    private Collider2D player_coll;
    public GameObject O_dieline = null;
    public int i_hp,i_hp_tmp;
    protected C_UIHP HP_ui;
    public string s_name = "player";
    public Transform player_tra;
    private bool b_hurting,b_attack_enable,b_play_ani, b_is_attack, b_both_attack;
    private float f_hurting_time,f_hurt_dir,f_attack_time;
    private int i_hit_number;
    C_PlayerAniEvent player_ani;

    //玩家運動變數
    private float f_speed = 0.0f;
    private bool b_jump, b_jump_sound = false;
    private float f_jump_speed = 0.0f;
    Vector3 last_position_vec3;
    Vector2 jump_vec2;
    public Vector3 between_cilling_vec3;
    public Vector3 between_virtuall_vec3;
    bool b_airmove = false;
    public LayerMask mask_layer;
    public bool direction = true; //面相右邊為true 左邊false
    public float f_gravity;

    //重生變數
    public bool b_die = false;
    private Vector3 respawn_position_vec3;
    private float f_dietime = 0;
    SkeletonAnimation skeleton_animation;
    private GameObject FloatStone;
    // Use this for initialization
    void Awake()
    {
        respawn_position_vec3 = transform.position;
        O_camera = GameObject.Find("Main Camera");
        b_die = false;
        f_jump_speed = 8.5f;
        f_speed = 8.0f;
        player_rig = GetComponent<Rigidbody2D>();
        t_ground_check = transform.Find("Groundcheck");
        t_ground_check2 = transform.Find("Groundcheck2");
        t_pic = transform.Find("pic");
        player_spine_animator = transform.GetChild(0).GetComponent<Animator>();
        player_ani = transform.GetChild(0).GetComponent<C_PlayerAniEvent>();
        player_animator = gameObject.GetComponent<Animator>();
        player_tra = gameObject.GetComponent<Transform>();
        jump_vec2 = new Vector2(0, f_jump_speed);
        player_coll = GetComponent<Collider2D>();
        respawn_position_vec3 = transform.position;
        b_jump = false;
        i_hp = 5;i_hp_tmp = 5;
        HP_ui = GameObject.Find("UI_HP").GetComponent<C_UIHP>();
        AOE_col = transform.GetChild(3);
        AOE_col.gameObject.SetActive(false);
        Stick_col = transform.Find("Stick_col");
        Stick_col.gameObject.SetActive(false);
        b_AOE_has = false;
        b_hurting = b_play_ani = b_is_attack = b_both_attack = false;
        b_attack_enable = true;
        f_hurting_time = f_attack_time = 0;
        i_hit_number = 0;
        FloatStone = GameObject.Find("FloatStone");
    }

    void Start()
    {
        //開始前都就先讀檔
        //給ui顯示現在的血量
        HP_ui.PresentHp (i_hp);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (C_SceneManager.SceneManger.GetComponent<C_SceneManager>().b_times_up) return; //暫停
        player_rig.velocity = new Vector2(player_rig.velocity.x, player_rig.velocity.y - f_gravity * Time.deltaTime);
        if (!b_player_controll) return;//無法操作
        if (!b_die)  //沒死
        {
            IsDie();    //判斷是生是死
            Move();  //基本移動
            last_position_vec3 = transform.position; //記下最後位置
        }
        else//死了
        {
            transform.position = last_position_vec3;
            PlayerRespawn();//重生
        }
    }

    void Update()
    {
        if (C_SceneManager.SceneManger.GetComponent<C_SceneManager>().b_times_up) return; //暫停
        //判斷在地上
        b_isground = (Physics2D.Linecast(transform.position, t_ground_check.position, 1 << LayerMask.NameToLayer("ground"))) ||
            (Physics2D.Linecast(transform.position, t_ground_check2.position, 1 << LayerMask.NameToLayer("ground")));
        player_spine_animator.SetBool("isground", b_isground);
        //受擊
        if (b_hurting) HurtTime();
        if (!b_player_controll) return;//無法操作
        if (!b_die)  //沒死
        {
            NormalAttack();
            //AOE_skill(); //範圍技
            TeleportToAni(); //上下瞬移
            JumpDetect();     //連跳落地判斷
            JumpChange();  //跳躍最高點動畫變換
            if (Input.GetKey(KeyCode.Space) && b_isground)//&& !b_magic
            {
                if (b_jump) return;
                b_jump = true;
                player_spine_animator.SetBool("jump", b_jump);
                JumpAct();
            }
            
            //射擊
            ShootAni();
            //射擊間格時間
            if (f_shoot < 3) f_shoot += Time.deltaTime;
        }
        if (player_tra.localScale.x > 0)
        {
            direction = true;
        }
        else direction = false;
    }

     

    void TeleportToAni() {
        RaycastHit2D hit_cilling_ray = Physics2D.Raycast(transform.position, Vector2.up, 5.0f, mask_layer);
        RaycastHit2D hit_ground_ray = Physics2D.Raycast(transform.position, Vector2.up, -5.0f, mask_layer);
        Debug.DrawLine(transform.position, transform.position + (Vector3)Vector2.up * 5.0f);
        Debug.DrawLine(transform.position, transform.position + (Vector3)Vector2.up * -5.0f, Color.red);
        if (hit_cilling_ray && !b_upside)
        {
            //紀錄鏡子和虛像與玩家的距離
            between_cilling_vec3 = new Vector3(transform.position.x, (transform.position.y + hit_cilling_ray.point.y) / 2 + 0.3f, transform.position.z);
            between_virtuall_vec3 = new Vector3(transform.position.x, hit_cilling_ray.point.y - 0.5f, transform.position.z);
        }
       else  if (hit_ground_ray && b_upside)
        {
            between_cilling_vec3 = new Vector3(transform.position.x, (transform.position.y + hit_ground_ray.point.y) / 2 - 0.3f, transform.position.z);
            between_virtuall_vec3 = new Vector3(transform.position.x, hit_ground_ray.point.y + 0.5f, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.E))
         {
            if (!b_use_skill) {
                player_spine_animator.Play("mirror");
                b_use_skill = true;
            } 
            
         }
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (b_magic)
            {
                if (!b_upside)
                {
                    transform.localScale = new Vector3(1.0f, -1.0f, 1f);
                    transform.position = between_virtuall_vec3;
                    f_gravity *= -1;
                    b_upside = true;
                }
                else
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1f);
                    transform.position = between_virtuall_vec3;
                    f_gravity *= -1;
                    b_upside = false;
                }
            }
            b_magic = false;
            b_use_skill = false;
        }
        if ((!hit_cilling_ray || !hit_ground_ray) || !b_isground && b_use_skill)
        {
            Destroy(O_tempvirtuall, 0f);
            b_use_skill = false;
            b_magic = false;
        }
    }


    public void Teleport()
    {
        //Debug.Log(b_use_skill);
        if (!b_use_skill) return;
        if ((!hit_cilling_ray || !hit_ground_ray) && !b_isground) return;
            // //按鍵後產生鏡子和虛像，並紀錄用過技能
            if ( b_isground&& !b_upside)
          {
               // O_tempmirror = Instantiate(O_mirror, between_cilling_vec3, Quaternion.identity) as GameObject;
                O_tempvirtuall = Instantiate(O_virtualplayer, between_virtuall_vec3, Quaternion.Euler(180, 0, 0)) as GameObject;
                b_magic = true;
            }

            if ( b_isground&& b_upside)
            {
               // O_tempmirror = Instantiate(O_mirror, between_cilling_vec3, Quaternion.identity) as GameObject;
                O_tempvirtuall = Instantiate(O_virtualplayer, between_virtuall_vec3, Quaternion.identity) as GameObject;
                b_magic = true;
            }
    }

    //跳
    void JumpAct()
    {
        if (!b_upside && b_jump)
        {
            if(!b_jump_sound)player_ani.JumpBegin();
            b_jump_sound = true;
            player_rig.velocity = new Vector2(player_rig.velocity.x, f_jump_speed);
            //Debug.Log("jump " + player_rig.velocity.x);
            player_spine_animator.SetBool("jumpover", false);
        }
        else if (b_upside && b_jump)
        {
            player_ani.JumpBegin();
            player_rig.velocity = new Vector2(player_rig.velocity.x, -f_jump_speed);
            player_spine_animator.SetBool("jumpover", false);
        }
    }
    void JumpChange()
    {
        if (!b_upside)
        {
            if (player_rig.velocity.y <= 0) {
                player_spine_animator.SetBool("jumpchange", true);
                b_jump_sound = false;
            } 
        }
        else {
            if (player_rig.velocity.y >= 0)
            {
                player_spine_animator.SetBool("jumpchange", true);
                b_jump_sound = false;
            } 
        }
    }
    void JumpDetect()
    {
        if (!b_jump) return;
        if (b_isground) {
            b_jump = false;
            player_spine_animator.SetBool("jump",b_jump);
        }
    }

    //移動
    void Move()
    {
        //空中撞到牆速度為0
        if (b_airmove) f_speed = 0;
        else f_speed = 5.0f;
        //橫向移動
        if (!b_upside)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);//轉向用
                player_rig.velocity = new Vector2(-f_speed, player_rig.velocity.y); //速度等於speed
                player_spine_animator.SetBool("walk", true);  //動畫開關

            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1f);//轉向用
                player_rig.velocity = new Vector2(f_speed, player_rig.velocity.y);
                player_spine_animator.SetBool("walk", true);
            }
        }

        else if (b_upside)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localScale = new Vector3(-1.0f, -1.0f, 1.0f);//轉向用
                player_rig.velocity = new Vector2(-f_speed, player_rig.velocity.y);
                player_spine_animator.SetBool("walk", true);

            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);//轉向用
                player_rig.velocity = new Vector2(f_speed, player_rig.velocity.y);
                player_spine_animator.SetBool("walk", true);
            }
        }

        if (!(Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.A)))
        {

            float temp = player_rig.velocity.x;
            if (Mathf.Abs(temp) > 0.2f) temp += -30.0f*Mathf.Sign(temp) * Time.deltaTime*0.5f;
            else temp = 0.0f;
            //Debug.Log("temp  " + temp);
            player_rig.velocity = new Vector2(temp, player_rig.velocity.y);
            player_spine_animator.SetBool("walk", false);
            
        }
    }


    void ShootAni()
    {
        Vector3 v3, v3_position;
        Vector2 v2, input, v2_position;
        float angle;
        v3 = Camera.main.WorldToScreenPoint(transform.position);  //自己位置轉成螢幕座標
        v2 = new Vector2(v3.x, v3.y); //再轉乘二維向量
        if(!b_upside) v3_position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(transform.lossyScale.x, 0.7f, 0));
        else v3_position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(transform.lossyScale.x, -1.0f, 0));
        v2_position = new Vector2(v3_position.x, v3_position.y);
        input = new Vector2(Input.mousePosition.x, Input.mousePosition.y); //紀錄滑鼠位置
        Vector2 normalized = ((input - v2_position)).normalized;  //滑鼠與自己的向量差正規化
        angle = Mathf.Atan2(-(input - v2_position).x, (input - v2_position).y) * Mathf.Rad2Deg;
        if (shoot_ani_time==0 && (Input.GetMouseButtonDown(1) && f_shoot > 0.5f) || (Input.GetMouseButtonDown(1) && f_shoot == 0))//射子彈
        {
            shoot_ani_time = 0.01f;
            player_spine_animator.Play("shoot");
        }
        if(shoot_ani_time>0)shoot_ani_time += Time.deltaTime;
        if (shoot_ani_time > 0.7f) ShootAct(normalized,angle);
    }

    void NormalAttack() {
       if (b_use_skill) return;
       f_attack_time += Time.deltaTime;
        if (f_attack_time > 0.7f) {
            i_hit_number = 0;
            b_attack_enable = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (f_attack_time > 0.03f || i_hit_number==0) {
                b_play_ani = true;
            } 
        }
        else {
            if (f_attack_time > 0.3f && !b_play_ani) {
                player_spine_animator.SetBool("attackover", true);
                i_hit_number = 0;
            } 
        }
        if (b_attack_enable && b_play_ani)
        {
            //Debug.Log("continue " + i_hit_number);

            if (i_hit_number < 1)
            {
                player_spine_animator.Play("attack0", 1);
                f_attack_time = 0;
                i_hit_number++;
                b_attack_enable = false;
                b_play_ani = false;
                player_spine_animator.SetBool("attackover", false);
            }
            else if(i_hit_number <2)
            {
                //Debug.Log("hit1");
                player_spine_animator.Play("attack1", 1);
                f_attack_time = 0;
                i_hit_number++;
                b_attack_enable = false;
                b_play_ani = false;
                player_spine_animator.SetBool("attackover", false);
            }
            else if (i_hit_number < 3)
            {
                //Debug.Log("hit3");
                f_attack_time = 0;
                b_hit_away = true;
                player_spine_animator.Play("attack2", 1);
                i_hit_number = 0;
                b_attack_enable = false;
                b_play_ani = false;
                player_spine_animator.SetBool("attackover", false);
            }
        }
    }
    public void NormalAtkDetect() {
        //Debug.Log("player attack detect");
        if (b_hurting) return;
        player_ani.StickSwingSound(i_hit_number);
        b_is_attack = true;
        Stick_col.gameObject.SetActive(true);
    } 

    public void NormalAttackOver() {
        //Debug.Log("over");
        f_attack_time = 0.18f;
        if (b_hit_away) {
            //Debug.Log("end");
            f_attack_time = 0.5f; //因為是最後一個
            b_hit_away = false;
        } 
        b_attack_enable = true;
        b_is_attack = false;
        player_spine_animator.SetBool("attackover", true);
        Stick_col.gameObject.SetActive(false);
    }

    void ShootAct(Vector2 normalied, float angle)
    {
        GameObject vbullet;
        Rigidbody2D vrigidbody;
        //算向量差與x軸的夾角的餘角(因為是讓子彈原是90度開始轉)
           if(!b_upside) vbullet = Instantiate(O_bullet, transform.position + new Vector3(transform.lossyScale.x, 0.7f, 0), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
           else vbullet = Instantiate(O_bullet, transform.position + new Vector3(transform.lossyScale.x, -1.0f, 0), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
        vrigidbody = vbullet.GetComponent<Rigidbody2D>();
            vrigidbody.velocity = new Vector2(normalied.x * 25, normalied.y * 25);
            vbullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            i_hp--;
            HP_ui.PresentHp(i_hp);
            f_shoot = 0;
        shoot_ani_time = 0f;
    }

    //腳色重生
    void PlayerRespawn()
    {
        f_dietime += Time.deltaTime;
        if (f_dietime > 1.3f)
        {
            this.player_rig.velocity = new Vector2(0, 0);
            transform.position = respawn_position_vec3;
            i_hp = i_hp_tmp;
            b_magic = false;
            b_upside = false;
            b_use_skill = false;
            transform.localScale = new Vector3(1.0f,1.0f,1);
            b_die = false;
            f_dietime = 0;
            O_camera.SendMessage("ResetPos");
            if (f_gravity < 0) f_gravity *= -1;
            FloatStone.SendMessage("ResetPosition");
        }
    }

    void AOE_skill() {
        if (Input.GetMouseButtonDown(2)){
            player_animator.Play("AOE_skill");
            b_AOE_has = true;
            AOE_col.gameObject.SetActive(true);
            
        }
    }
    public void AOE_end() {
        b_AOE_has = false;
        AOE_col.gameObject.SetActive(false);
    }


    public void BothAttack() {
        if (b_both_attack) return;
        b_both_attack = true;
    }

    //受傷
    public void GetHurt(float hurt_dir)
    {
        Debug.Log("player get hurt");
        Stick_col.gameObject.SetActive(false);
        player_spine_animator.SetBool("attackover", true);
        b_player_controll = false;
            player_spine_animator.Play("hit2");
            i_hp--;
            HP_ui.PresentHp(i_hp);
            b_hurting = true;
            b_is_attack = false;
            f_hurt_dir = hurt_dir;
            Debug.Log("hurt");
        if (!b_upside)
        {
            player_rig.velocity = new Vector2(-5.0f * hurt_dir, 10.0f);
            transform.localScale = new Vector3(Mathf.Sign(hurt_dir), 1.0f, 1.0f);
        }
        else {
            player_rig.velocity = new Vector2(-5.0f * hurt_dir, -10.0f);
            transform.localScale = new Vector3(Mathf.Sign(hurt_dir), -1.0f, 1.0f);
        }
    }
    //受擊傷害時間
    public void HurtTime() {
        Debug.Log("hurt time");
        f_hurting_time += Time.deltaTime;
        if(!b_upside)player_rig.velocity += new Vector2(10.0f *f_hurt_dir* Time.deltaTime, -20.0f*Time.deltaTime);  //f_hurt_dir是被擊的反方向
        else player_rig.velocity += new Vector2(10.0f * f_hurt_dir * Time.deltaTime, 20.0f * Time.deltaTime);
        if (f_hurting_time > 0.5f) {
            b_hurting = false;
            b_player_controll = true;
            f_hurting_time = 0.0f;
            //player_rig.velocity = Vector2.zero;
        } 
        
    }

    //判斷掉落死亡
    void IsDie()
    {
        if (transform.position.y < O_dieline.transform.position.y || transform.position.y>35.0f)
        {
            b_die = true;
        }
    }

    public void FreezeControl() {
        b_player_controll = false;
        player_spine_animator.SetBool("walk",false);
    }
    public void UnFreezeControl()
    {
        b_player_controll = true;
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        //遍歷每一碰撞點，判斷
        foreach (ContactPoint2D con in coll.contacts)
        {
            if (!b_isground && Mathf.Sign(con.normal.x) == - (Mathf.Sign(transform.localScale.x)) && coll.gameObject.tag == "floor")
            {
                b_airmove = true;
            }
            else
            {
                b_airmove = false;
            }
        }
    }
    void OnCollisionExit2D(Collision2D coll) {
        b_airmove = false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "hp_props")
        {
            i_hp++;
            HP_ui.PresentHp(i_hp);
            Destroy(collider.gameObject);
        }
        else if (collider.tag == "save_point")
        {
            respawn_position_vec3 = collider.gameObject.transform.position;
            i_hp_tmp = i_hp;
            b_is_save = true;
            C_SceneManager.SceneManger.GetComponent<C_SceneManager>().ChangeSavePoint();
            //Debug.Log(b_is_save);
            Destroy(collider.gameObject);
        }
        else if (collider.tag == "enemy") {
            //if(b_AOE_has) collider.gameObject.SendMessage("GetHurt");
            if (b_is_attack) {
                if (!collider.GetComponent<C_Enemy>().b_attacking)
                {
                    //Debug.Log("player attack");
                    collider.gameObject.GetComponent<C_Enemy>().GetHurt(b_hit_away, transform.localScale.x);                }
                else {
                    //Debug.Log("player both attack");
                    b_both_attack = true;
                    Stick_col.gameObject.SetActive(false);
                    player_ani.BothAttackSound();
                    collider.gameObject.GetComponent<C_Enemy>().BothAttack();
                }
                b_is_attack = false;
            } 

        }
        else if (collider.tag == "debris")
        {
            if(b_AOE_has) Destroy(collider.gameObject);
        }
        else if (collider.tag =="scene_manager") {
            C_SceneManager.SceneManger.GetComponent<C_SceneManager>().OnDetect();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "scene_manager"&&C_SceneManager.SceneManger.GetComponent<C_SceneManager>().i_save_point==3) {
            O_camera.SendMessage("reset");
        }
    }

}
