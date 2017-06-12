using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_EnemyAniEvent : MonoBehaviour {

    Animator enemy_animator;

	// Use this for initialization
	void Awake () {
        enemy_animator = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeType() {
        enemy_animator.SetBool("attack_type", true);
        transform.GetComponentInParent<C_Enemy>().Howl();
    }
    void ChangeTypeOver() {
        transform.GetComponentInParent<C_Enemy>().PreAttack();
    }

    void AttackDetect() {
        transform.GetComponentInParent<C_Enemy>().Attackarea();
    }

    void AttackOver() {
        transform.GetComponentInParent<C_Enemy>().AttackOver();
    }

    void Die() {
        Destroy(transform.parent.gameObject);
    }

}
