﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Enemy : MonoBehaviour {
    Rigidbody2D enemy_body;
    public LayerMask mask;
    Vector3 respawn_location_vec3;
    Animator enemy_animator;
    Transform t_detect;
    RaycastHit2D ray_seeplayer, ray_detect;
    GameObject player;
    CircleCollider2D attack_area;
    bool b_toofar, b_to_right, b_is_hurt, b_attack, b_both_attack, b_pre_attack;
    public int i_HP, i_mode;
    float f_distance, f_ramble_left, f_ramble_right, f_ramble_wait, f_face_way, f_atk_blank, f_away_dir, f_hurt_time, f_die_time;
    public float f_ramble_dis, f_run_speed, f_walk_speed, f_trace_dis, f_sight_dis, f_player_dis, f_player_away;
    bool b_see_it, b_ramble_return, b_hit_away;
    AudioSource audio_source;
    public AudioClip[] hurt_sound = new AudioClip[3];
    public AudioClip attack_sound, howl_sound;
    public bool b_attacking, b_ground_uncheck;
    Vector3 vplayer_pos;
    // Use this for initialization
    void Awake()
    {
        enemy_body = gameObject.GetComponent<Rigidbody2D>();
        respawn_location_vec3 = transform.position;
        enemy_animator = gameObject.GetComponentInChildren<Animator>();
        t_detect = gameObject.transform.GetChild(2);
        player = GameObject.Find("player");
        attack_area = gameObject.transform.GetChild(1).GetComponent<CircleCollider2D>();
        f_ramble_left = respawn_location_vec3.x - f_ramble_dis;
        f_ramble_right = respawn_location_vec3.x + f_ramble_dis;
        b_see_it = b_ramble_return = false;
        b_toofar = b_attack = b_to_right = b_is_hurt = b_hit_away = b_both_attack = b_attacking = b_pre_attack =  false;
        f_face_way = transform.localScale.x;
        f_ramble_wait = f_atk_blank = f_away_dir = f_hurt_time = f_die_time =  0.0f;
        audio_source = GetComponent<AudioSource>();
        vplayer_pos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //碰到玩家的raycast
        if (!b_see_it) {
            b_see_it = ray_seeplayer = Physics2D.Raycast(transform.position, (new Vector3(transform.localScale.x, 0, 0) + transform.up * 0.2f), f_sight_dis, mask);
            b_ground_uncheck = false;
            if(b_see_it)vplayer_pos = ray_seeplayer.transform.position;
        } 
        else {
            if (ray_seeplayer.transform == null) {
                b_see_it = false;
                b_ground_uncheck = true;
            } 
        }
        Debug.DrawLine(transform.position, transform.position + (new Vector3(transform.localScale.x, 0, 0) + transform.up * 0.2f).normalized * f_sight_dis);
        if (!IsDie())
        {
            if (b_is_hurt)
            {
                HurtTime();
                return;
            }
            //沒看到玩家進入待機
            if (!seePlay())
            {
                behaviorMode();
            }
        }
        else
        {
        }

    }

    //追逐視野內玩家
    bool seePlay()
    {
        Vector3 walkto_vec3 = new Vector3(0,0,0);
        //判斷有無碰到地板
        ray_detect = Physics2D.Linecast(transform.position, t_detect.transform.position, 1 << LayerMask.NameToLayer("ground"));
        f_distance = Mathf.Abs(transform.position.x - respawn_location_vec3.x);
        if (f_distance > f_trace_dis) b_toofar = true;
        else b_toofar = false;
        if (b_attack && !Wait(ref f_atk_blank, 2.0f)) return true;
        else
        {
            //Debug.Log("after_wait" + f_atk_blank);
            f_atk_blank = 0;
            b_attack = false;
        }

        if (!b_ground_uncheck)
        {
            if (b_see_it && ray_detect )
            {
                f_player_dis = Vector2.Distance(transform.position, ray_seeplayer.transform.position);
                if (!b_toofar && f_player_dis < f_player_away)
                {
                    enemy_animator.SetBool("see_player", true);
                    if (!b_pre_attack) return true;
                    walkto_vec3 = new Vector3(ray_seeplayer.transform.position.x - transform.position.x, 0, 0);
                    if (!b_attack) transform.localScale = new Vector3(-1.0f * Mathf.Sign(transform.position.x - ray_seeplayer.transform.position.x), 1, 1);
                    if (Mathf.Abs((ray_seeplayer.transform.position.x - transform.position.x)) < 2.0f && !b_attack)
                    {
                        Debug.Log("enemy attack");
                        if (b_is_hurt) return true;
                        enemy_body.velocity = new Vector3(0, 0, 0);
                        b_attack = true;
                        enemy_animator.SetBool("walk", false);
                        enemy_animator.SetBool("attack_over", false);
                        //audio_source.PlayOneShot(attack_sound);
                        enemy_animator.Play("EnemyAttack");
                        return (true);
                    }
                    enemy_animator.SetBool("walk", true);
                    enemy_body.velocity = new Vector3(walkto_vec3.normalized.x * f_run_speed * 1.4f, enemy_body.velocity.y, 0);
                    return true;
                }
                else
                {
                    Debug.Log("too far");
                    //transform.localScale = new Vector3(-1.0f * Mathf.Sign(transform.position.x - respawn_location_vec3.x), 1, 1);
                    b_see_it = false;
                    return false;
                }
            }
            else
            {
                //Debug.Log("ground undetext");
                b_see_it = false;
                return false;
            }
        }
        else {
            if (b_see_it&&(ray_detect || b_ground_uncheck))
            {
                f_player_dis = Vector2.Distance(transform.position, vplayer_pos);
                if (!b_toofar && f_player_dis < f_player_away)
                {
                    enemy_animator.SetBool("see_player", true);
                    if (!b_pre_attack) return true;
                    walkto_vec3 = new Vector3(vplayer_pos.x - transform.position.x, 0, 0);
                    if (!b_attack) transform.localScale = new Vector3(-1.0f * Mathf.Sign(transform.position.x - vplayer_pos.x), 1, 1);
                    if (Mathf.Abs((vplayer_pos.x - transform.position.x)) < 2.0f && !b_attack)
                    {
                        Debug.Log("enemy attack");
                        if (b_is_hurt) return true;
                        enemy_body.velocity = new Vector3(0, 0, 0);
                        b_attack = true;
                        enemy_animator.SetBool("walk", false);
                        enemy_animator.SetBool("attack_over", false);
                       // audio_source.PlayOneShot(attack_sound);
                        enemy_animator.Play("EnemyAttack");
                        return (true);
                    }
                    enemy_animator.SetBool("walk", true);
                    enemy_body.velocity = new Vector3(walkto_vec3.normalized.x * f_run_speed * 1.4f, enemy_body.velocity.y, 0);
                    return true;
                }
                else
                {
                    Debug.Log("too far");
                    //transform.localScale = new Vector3(-1.0f * Mathf.Sign(transform.position.x - respawn_location_vec3.x), 1, 1);
                    b_see_it = false;
                    return false;
                }
            }
            else
            {
                //Debug.Log("ground undetext");
                b_see_it = false;
                return false;
            }
        }

    }

    void behaviorMode()
    {
        
        Vector3 walkto_vec3;
        Vector3 pos_vec3 = transform.position;
        if (b_pre_attack) {
            Debug.Log("to normale mode");
            b_pre_attack = false;
            enemy_animator.SetBool("attack_type", false);
            enemy_animator.SetBool("see_player", false);
            f_ramble_wait = 0.0f;
        }
        switch (i_mode)
        {
            case 0:
                if (Mathf.Abs(respawn_location_vec3.x - pos_vec3.x) > 0.5f)
                {
                    if (!Wait(ref f_ramble_wait, 1.2f))
                    {
                        enemy_animator.SetBool("walk",false);
                        enemy_body.velocity = new Vector3(0, enemy_body.velocity.y, 0);
                        return;
                    }
                    enemy_animator.SetBool("walk", true);
                    transform.localScale = new Vector3(-1.0f*Mathf.Sign(pos_vec3.x - respawn_location_vec3.x), 1, 1);
                    walkto_vec3 = new Vector3((respawn_location_vec3 - pos_vec3).x, 0, 0);
                }
                else
                {
                    f_ramble_wait = 0.0f;
                    transform.localScale = new Vector3(f_face_way, 1, 1);
                    walkto_vec3 = Vector3.zero;
                    enemy_animator.SetBool("walk", false);
                }
                enemy_body.velocity = new Vector3(walkto_vec3.normalized.x * f_walk_speed, enemy_body.velocity.y, 0);
                break;

            case 1:
                if (pos_vec3.x < f_ramble_left)
                {
                    if (!(Wait(ref f_ramble_wait, 1.5f)))
                    {
                        enemy_animator.SetBool("walk", false);
                        enemy_body.velocity = new Vector3(0, enemy_body.velocity.y, 0);
                        return;
                    }
                    enemy_animator.SetBool("walk", true);
                    b_ramble_return = true;
                    transform.localScale = new Vector3(-f_face_way, 1, 1);
                    walkto_vec3 = new Vector3(-f_face_way, 0, 0);
                    b_to_right = true;
                }
                else if (pos_vec3.x > f_ramble_right)
                {
                    if ((!Wait(ref f_ramble_wait, 1.5f)))
                    {
                        enemy_body.velocity = new Vector3(0, enemy_body.velocity.y, 0);
                        enemy_animator.SetBool("walk", false);
                        return;
                    }
                    enemy_animator.SetBool("walk", true);
                    transform.localScale = new Vector3(f_face_way, 1, 1);
                    walkto_vec3 = new Vector3(f_face_way, 0, 0);
                    b_to_right = false;
                }
                else
                {
                    f_ramble_wait = 0.0f;
                    if (b_to_right)
                    {
                        walkto_vec3 = new Vector3(-f_face_way, 0, 0);
                    }
                    else
                    {
                        walkto_vec3 = new Vector3(f_face_way, 0, 0);
                    }
                    enemy_animator.SetBool("walk", true);
                }
                enemy_body.velocity = new Vector3(walkto_vec3.normalized.x * f_walk_speed, enemy_body.velocity.y, 0);
                break;
        }
    }

    bool Wait(ref float f_current_time, float f_total_time)
    {
        if (f_current_time < f_total_time)
        {
            f_current_time += Time.deltaTime;
            return false;
        }
        else
        {
            return true;
        }

    }

    public void Howl() {
        audio_source.PlayOneShot(howl_sound);
    }

    public void Attackarea()
    {
        Debug.Log("enemy attack detect");
        audio_source.PlayOneShot(attack_sound);
        if (b_is_hurt) return;
        b_attacking = true;
        attack_area.enabled = true;
    }

    public void AttackOver()
    {
        attack_area.enabled = false;
        b_attacking = false;
        enemy_animator.SetBool("attack_over", true);
        //b_attack = false;
    }

    public void BothAttack()
    {
        b_both_attack = false;
        attack_area.enabled = false;
        b_attacking = false;
        enemy_animator.SetBool("attack_over", true);
        f_atk_blank = 1.7f;
    }

    public void GetHurt(bool hit_away, float dir)
    {
        Debug.Log("enemy get hurt");
        if (i_HP <= 0) return;
        enemy_animator.Play("Hurt");
        int random = Random.Range(0, 2);
        audio_source.PlayOneShot(hurt_sound[random]);
        i_HP--;
        b_is_hurt = true;
        b_attacking = false;
        b_hit_away = hit_away;
        attack_area.enabled = false;
        enemy_animator.SetBool("attack_over", true);
        //enemy_animator.Play("Emepty");
        f_atk_blank = 1.0f;
        if (!b_see_it) transform.localScale = new Vector3(-dir,1,1);
        if (hit_away)
        {
            f_away_dir = dir;
            enemy_body.velocity = new Vector3(10.0f * f_away_dir, 0.0f, 0.0f);
        }
        //Debug.Log("enemy ouch");
    }

    public void PreAttack() {
        b_pre_attack = true;
        //enemy_animator.SetBool("attack_type",true);
    }

    void HurtTime()
    {
        if (!b_hit_away)
        {
            if (!Wait(ref f_hurt_time, 0.5f)) return;
            else
            {
                b_is_hurt = false;
                f_hurt_time = 0.0f;
                enemy_animator.SetBool("attack_over", false);
                return;
            }
        }
        if (Mathf.Abs(enemy_body.velocity.x) > 0.3f) enemy_body.velocity += new Vector2(-f_away_dir * 10.0f * Time.deltaTime, 0.0f);
        else
        {
            b_is_hurt = false;
            enemy_animator.SetBool("attack_over", false);
        }
    }

    bool IsDie()
    {
        float die_time = 0.3f;
        if (i_HP > 0) return false;
        else {
            if (b_hit_away) {
                die_time = 0.5f;
                if (Mathf.Abs(enemy_body.velocity.x) > 0.3f) enemy_body.velocity += new Vector2(-f_away_dir * 10.0f * Time.deltaTime, 0.0f);
            } 
            if (!Wait(ref f_die_time, die_time)) return true;
            else enemy_animator.Play("Die");
            return true;
        } 
    }

    //四邊形面積
    private float multiarea(Vector3 point, Vector3 ver1, Vector3 ver2)
    {
        Vector3 tri1, tri2;
        float area;
        tri1 = ver1 - point;
        tri2 = ver2 - point;
        area = (tri1.x * tri2.y - tri1.y * tri2.x);
        return (area);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("enemy trigger");
        if (collision.tag == "Player" && b_attacking)
        {
            //Debug.Log("enemy attack act");
            attack_area.enabled = false;
            if (b_both_attack)
            {
                b_both_attack = false;
                attack_area.enabled = false;
                //Debug.Log("enemy both attack");
                return;
            }

            collision.gameObject.SendMessage("GetHurt", -transform.localScale.x);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
    }
}
