using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Vector2 movement = new Vector2(0, 0);
    bool jump;
    float velocity = 0f;
    bool double_jump = false;
    float original_speed_mod;
    float original_gravity_mod;
    //public vars
    public float gravity_mod = 1f;
    public float jump_height = 2f;
    public float speed_mod = 10f;
    public float dash_multiplier = 2f;
    public float fastfall_multiplier = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        original_speed_mod = speed_mod;
        original_gravity_mod = gravity_mod;
    }

    // Update is called once per frame
    void Update()
    {
        //ROTATION SCRIPT. DO NOT USE FOR PLAYER, USE FOR OTHER STUFF
        //Vector2 world_to_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 direction = (world_to_mouse - (Vector2)transform.position).normalized;
        //transform.up = direction;
        
        //ground detection
        RaycastHit2D ground = Physics2D.CircleCast(transform.position, 0.4f, Vector2.down, 0.62f, LayerMask.GetMask("Ground"));
        //wall detection
        RaycastHit2D left_wall = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 0.5f), 0f, Vector2.left, 0.27f, LayerMask.GetMask("Ground"));
        RaycastHit2D right_wall = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 0.5f), 0f, Vector2.right, 0.27f, LayerMask.GetMask("Ground"));

        //placeholder sprint
        if(Input.GetKey(KeyCode.LeftShift)) {
            speed_mod = original_speed_mod * dash_multiplier;
        } else {
            speed_mod = original_speed_mod;
        }

        //movement
        movement.y = 0f;
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if(ground.collider != null) {
            movement.x = Input.GetAxisRaw("Horizontal") * speed_mod;
        } else {
            //air drift
            movement.x += Input.GetAxisRaw("Horizontal") * Time.deltaTime * 5 * speed_mod;
            movement.x = Mathf.Clamp(movement.x, -speed_mod, speed_mod);
            if(Input.GetAxisRaw("Horizontal") == 0) {
                if(movement.x > 0) {
                    movement.x -= Time.deltaTime * 3 * speed_mod;
                } else if(movement.x < 0) {
                    movement.x += Time.deltaTime * 3 * speed_mod;
                }
            }
        }

        //reset movement.x to zero when against wall
        if(left_wall.collider != null && movement.x < 0) {
            Debug.Log("Next to left wall");
            movement.x = 0f;
        } else if(right_wall.collider != null && movement.x > 0) {
            Debug.Log("Next to right wall");
            movement.x = 0f;
        }

        //jump
        if(Input.GetButtonDown("Jump")) {
            if(ground.collider != null) {
                jump = true;
            } else if(double_jump) {
                jump = true;
                double_jump = false;
            }
        }

        //fastfall
        if(Input.GetButton("Fastfall")) {
            gravity_mod = original_gravity_mod * fastfall_multiplier;
        } else {
            gravity_mod = original_gravity_mod;
        }

        //ceiling detection
        RaycastHit2D ceiling = Physics2D.CircleCast(transform.position, 0.4f, Vector2.up, 0.62f, LayerMask.GetMask("Ground"));
        if(ceiling.collider != null) {
            velocity = 0f;
        }

        //flip sprite depending on move direction
        if(Input.GetAxisRaw("Horizontal") > 0) {
            sprite.flipX = true;
        } else if(Input.GetAxisRaw("Horizontal") < 0) {
            sprite.flipX = false;
        }
    }

    void FixedUpdate() {
        //gravity
        RaycastHit2D ground = Physics2D.CircleCast(transform.position, 0.4f, Vector2.down, 0.62f, LayerMask.GetMask("Ground"));
        if(ground.collider == null) {
            velocity -= gravity_mod * Time.fixedDeltaTime;
        } else {
            velocity = 0f;
            double_jump = true;
        }
        velocity = Mathf.Clamp(velocity, -original_gravity_mod, Mathf.Infinity);
        //jumping
        if(jump == true) {
            velocity = jump_height;
            jump = false;
        }
        //movement
        movement.y += velocity;
        rb.MovePosition((Vector2)rb.position + new Vector2(movement.x * Time.fixedDeltaTime, movement.y));
    }
}