using UnityEngine;
using System.Collections;

public class C_FloatStone : MonoBehaviour {
    public float speed = 1;
    public GameObject stone;
    public GameObject player;
    Vector3 player_tra;
    Vector3 stone_tra;
    [Header("Switches")]
    public bool up = false; 
    public bool right = false; 
    public bool touch = false;  
    [Header("Distance Settings")]
    public float f_distance=0;
    float vertical_time; //垂直計時器
    float horizontal_time; //水平計時器
   
    void Start () {
        stone_tra = stone.transform.position;
        horizontal_time = 10;
    }
   
    void Update () {

        player_tra = player.transform.position;
        #region 移動

        if (up)
        {
            transform.position = new Vector3(transform.position.x, PingPong(vertical_time * speed, stone_tra.y, 29), transform.position.z);
        }
        else if (right)
        {
            horizontal_time += Time.deltaTime; //水平計時器
            transform.position = new Vector3(PingPong(horizontal_time * speed, 91, 101), transform.position.y, transform.position.z);
        }

        #endregion
    }

    float PingPong(float t, float minLength, float maxLength) 
    {
        return Mathf.PingPong(t, maxLength - minLength) + minLength;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (player_tra.y < this.transform.position.y && touch)
        {
            up = true;
            vertical_time += Time.deltaTime; 
            if (Input.GetKey(KeyCode.Space) )
            {

            }
            else
            {
                player.transform.position = new Vector3(player.transform.position.x, this.transform.position.y + f_distance, player.transform.position.z);
            }

        }
        else if (player_tra.y > this.transform.position.y && touch)
        {
            right = true;
        }
        
    }
    private void ResetPosition()
    {
        stone.transform.position = stone_tra;
        vertical_time = 0;
        horizontal_time = 10;
    }
    #region 觸發條件
    private void OnTriggerExit2D(Collider2D collision)
    {
        up = false;
        right = false;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        touch = true;
    }
    private void CollisionExit2D(Collider2D collision)
    {
        touch = false;      
    }
    #endregion
}
