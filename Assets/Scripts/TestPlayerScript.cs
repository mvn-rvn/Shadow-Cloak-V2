using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerScript : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 movement;
    bool jump;
    float velocity = 0f;
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
            RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
            if(ground.collider != null) {
                jump = true;
                Debug.Log("JUMP YOU HECKER JUMP");
            } else {
                jump = false;
            }
        } else {
            jump = false;
        }
    }

    void FixedUpdate() {
        //movement
        rb.MovePosition((Vector2)transform.position + (movement * Time.deltaTime * 10));
        //gravity
        RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));
        if(ground.collider == null) {
            velocity -= 1 * Time.deltaTime;
            rb.MovePosition((Vector2)transform.position + new Vector2(0, velocity));
        } else {
            velocity = 0;
        }
        if(jump == true) {
            velocity = 20;
        }
    }
}
