using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerAniEvent : MonoBehaviour {
    C_Player player;
    public AudioClip[] step_sound = new AudioClip[5];
    public AudioClip[] stick_swing_sound = new AudioClip[3];
    public AudioClip both_attack_sound;
    public AudioClip fall_sound, jump_sound;
    public AudioSource audio_source;
    int run_index;
    // Use this for initialization
    void Awake() {
        player = transform.GetComponentInParent<C_Player>();
        audio_source = transform.GetComponentInParent<AudioSource>();
        run_index = 0;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) {
            if (player.b_isground) {
                //audio_source.Stop();
                audio_source.PlayOneShot(step_sound[run_index]);
                if (run_index + 1 > 4) run_index = 0;
                else run_index++;
            }
        }
    }
    void StickHitOver() {
        //transform.parent.SendMessage("NormalAttackOver");
        transform.GetComponentInParent<C_Player>().NormalAttackOver();
    }
    void StickDetect() {
        transform.GetComponentInParent<C_Player>().NormalAtkDetect();
    }

    void VerticleTeleport() {
        transform.GetComponentInParent<C_Player>().Teleport();
    }

    void JumpOver() {
        //transform.parent.SendMessage("JumpEnd");
    }
    void StepSound() {
        //audio_source.pitch = Random.Range(1.7f, 2.0f);
        //audio_source.volume = Random.Range(0.5f,1.0f);
        audio_source.PlayOneShot(step_sound[run_index]);
        if (run_index + 1 > 4) run_index = 0;
        else run_index++;
    }

    void JumpFall() {
        audio_source.PlayOneShot(fall_sound);
    }

    public void JumpBegin() {
        //Debug.Log("jump sound");
        Debug.Log("jump");
        audio_source.PlayOneShot(jump_sound);
    }
    public void BothAttackSound() {
        audio_source.PlayOneShot(both_attack_sound);
    }
    public void StickSwingSound(int num) {
        switch (num)
        {
            case 0:
                //audio_source.pitch = Random.Range(1.7f, 2.0f);
                //audio_source.volume = Random.Range(0.5f,1.0f);
                audio_source.PlayOneShot(stick_swing_sound[0]);
                break;
            case 1:
                audio_source.PlayOneShot(stick_swing_sound[1]);
                break;
            case 2:
                audio_source.PlayOneShot(stick_swing_sound[2]);
                break;
        }
    }
}
