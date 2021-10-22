using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    PlayerMovement player_mov;
    float dash_time = 0f;
    bool is_dashing = false;
    float dash_dir;
    //public vars
    public float dash_cooldown = 0.5f;
    public float max_dash_time = 0.2f;
    public float dash_speed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        player_mov = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Dash") && !is_dashing && Input.GetAxisRaw("Horizontal") != 0) {
            Debug.Log("GAS GAS GAS IM GONNA STEP ON THE GAS TONIGHT ILL FLY");
            dash_dir = Input.GetAxisRaw("Horizontal");
            is_dashing = true;
            while(dash_time < max_dash_time) {
                dash_time += Time.deltaTime;
                player_mov.velocity = 0f;
                player_mov.movement.x = dash_speed * dash_dir;
            }
            dash_time = 0f;
            is_dashing = false;
        }
    }
}
