using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 movement;
    bool jump;
    float velocity = 0f;
    //public vars
    public float gravity_mod = 1f;
    public float jump_height = 2f;
    public float speed_mod = 10f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //ROTATION SCRIPT. DO NOT USE FOR PLAYER, USE FOR OTHER STUFF
        //Vector2 world_to_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 direction = (world_to_mouse - (Vector2)transform.position).normalized;
        //transform.up = direction;

        //movement
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if(movement.x != 0) {
            Debug.Log("Moving");
        }
        //jump
        if(Input.GetButton("Jump")) {
            RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, 1.02f, LayerMask.GetMask("Ground"));
            if(ground.collider != null) {
                jump = true;
                Debug.Log("JUMP YOU HECKER JUMP");
            }
        }
    }

    void FixedUpdate() {
        //gravity
        RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, 1.02f, LayerMask.GetMask("Ground"));
        if(ground.collider == null) {
            Debug.Log("On ground");
            velocity -= gravity_mod * Time.deltaTime;
        } else {
            velocity = 0f;
        }
        //jumping
        if(jump == true) {
            velocity = 2f;
            jump = false;
        }
        //movement
        movement.y += velocity;
        rb.MovePosition((Vector2)transform.position + (movement * Time.deltaTime * 10));
    }
}
