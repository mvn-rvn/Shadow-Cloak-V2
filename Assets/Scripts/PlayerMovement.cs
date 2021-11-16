using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: BUILD THE HECKING LEVEL

public class PlayerMovement : MonoBehaviour
{
    //normal/private vars
        //rigidbody
            Rigidbody2D rb;
        //sprite renderer
            SpriteRenderer sprite_rend;
        //movement vector
            Vector2 movement = new Vector2(0, 0);
        //gravity velocity
            float velocity = 0f;
        //jump stuffs
            bool dash_jump = false;
            bool wall_jump = false;
            bool double_jump = false;
        //original mods
            float original_speed_mod;
            float original_gravity_mod;
            float original_fall_speed_limit;
            float wall_grip_time = 0f;
        //dash
            float dash_duration = 0f;
            float dash_direction = 0f;
            float dash_cooldown = 0f;
            bool dash = false;
            float dash_trail_spawn = 0f;
    
    //public vars
        //movement mods
            [Header("Movement Mods")]
            public float gravity_mod = 1f;
            public float jump_height = 2f;
            public float speed_mod = 10f;
        //dash mods
            [Header("Dash Mods")]
            public float dash_multiplier = 2f;
            public float max_dash_duration = 0.2f;
            public float max_dash_cooldown = 0.3f;
            public float dash_spawn_rate = 0.05f;
        //fastfall
            [Header("Fastfall")]
            public float fastfall_multiplier = 1.5f;
            public float fall_speed_limit = 1f;
        //wall gripping
            [Header("Wall Gripping")]
            public float wall_grip = 0.25f;
            public float wall_grip_limit = 1.5f;
        //sprites
            [Header("Sprites")]
            public Sprite dash_sprite;
            public Sprite walk_sprite;
            public Sprite dash_trail_sprite;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite_rend = GetComponent<SpriteRenderer>();
        original_speed_mod = speed_mod;
        original_gravity_mod = gravity_mod;
        original_fall_speed_limit = fall_speed_limit;
    }

    // Update is called once per frame
    void Update()
    {
        //ROTATION SCRIPT. DO NOT USE FOR PLAYER, USE FOR OTHER STUFF
        //Vector2 world_to_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector2 direction = (world_to_mouse - (Vector2)transform.position).normalized;
        //transform.up = direction;
        
        //ground detection
        RaycastHit2D ground = Physics2D.CircleCast(
            transform.position,
            0.4f, 
            Vector2.down, 
            0.62f, 
            LayerMask.GetMask("Ground")
        );

        //left wall detection
        RaycastHit2D left_wall = Physics2D.BoxCast(
            transform.position,
            new Vector2(0.5f, 1.9f),
            0f,
            Vector2.left,
            0.27f,
            LayerMask.GetMask("Ground")
        );

        //right wall detection
        RaycastHit2D right_wall = Physics2D.BoxCast(
            transform.position,
            new Vector2(0.5f, 1.9f),
            0f,
            Vector2.right,
            0.27f,
            LayerMask.GetMask("Ground")
        );

        //gripping to wall
        if((left_wall.collider != null || right_wall.collider != null)
            && wall_grip_time < wall_grip_limit
            && Input.GetAxisRaw("Horizontal") != 0
            && !dash)
        {
            fall_speed_limit = wall_grip;
            wall_jump = true;
            double_jump = true;

            if(velocity < 0) {
                wall_grip_time += Time.deltaTime;
            }

        } else {
            fall_speed_limit = original_fall_speed_limit;
            wall_jump = false;
        }

        //replenish grip time when on ground
        if(ground.collider != null) {
            wall_grip_time = 0f;
        }

        //movement
        movement.y = 0f;
        if(ground.collider != null) {
            movement.x = Input.GetAxisRaw("Horizontal") * speed_mod;
            
        } else {
            //air drift
            if((movement.x <= original_speed_mod && Input.GetAxisRaw("Horizontal") == 1) 
                || (movement.x >= -original_speed_mod && Input.GetAxisRaw("Horizontal") == -1))
            {
                movement.x += Input.GetAxisRaw("Horizontal") * Time.deltaTime * 5 * speed_mod;
            }

            if(Input.GetAxisRaw("Horizontal") == 0) {
                //if movement is close enough to zero, clamp to zero
                if(movement.x < Time.deltaTime * 3 * speed_mod
                    && movement.x > -Time.deltaTime * 3 * speed_mod) 
                {
                    movement.x = 0f;

                } else if(movement.x > 0) {
                    movement.x -= Time.deltaTime * 3 * speed_mod;

                } else if(movement.x < 0) {
                    movement.x += Time.deltaTime * 3 * speed_mod;
                }
            }
        }

        //reset movement.x to zero when against wall
        if(left_wall.collider != null && movement.x < 0) {
            movement.x = 0f;

        } else if(right_wall.collider != null && movement.x > 0) {
            movement.x = 0f;
        }

        //jump/double-jump/wall-jump
        if(Input.GetButtonDown("Jump")) {
            if(ground.collider != null) {
                velocity = jump_height;
            
            } else if(wall_jump) {
                wall_grip_time = 0f;
                velocity = jump_height;
                wall_jump = false;

                if(Input.GetAxisRaw("Horizontal") == -1) {
                    movement.x = original_speed_mod;

                } else {
                    movement.x = -original_speed_mod;
                }
            
            } else if(double_jump && !dash) {
                velocity = jump_height;
                double_jump = false;
            }
        }

        //fastfall
        if(Input.GetButton("Fastfall")) {
            gravity_mod = original_gravity_mod * fastfall_multiplier;

        } else {
            gravity_mod = original_gravity_mod;
        }

        //dash
        if(Input.GetButtonDown("Dash") && Input.GetAxisRaw("Horizontal") != 0 && dash_cooldown == max_dash_cooldown) {
            dash_direction = Input.GetAxisRaw("Horizontal");
            dash_duration = 0f;
            dash = true;
            sprite_rend.sprite = dash_sprite;
        }

        if(dash && dash_duration < max_dash_duration) {
            speed_mod = original_speed_mod * dash_multiplier;
            dash_duration += Time.deltaTime;
            movement.x = dash_direction * speed_mod;
            dash_cooldown = 0f;

            //dash-jump
            if(Input.GetButtonDown("Jump") && ground.collider != null) {
                dash = false;
                dash_jump = true;
                dash_duration = 0f;
                velocity = jump_height * 0.9f;
                movement.x = dash_direction * speed_mod;
                sprite_rend.sprite = walk_sprite;
            }

        } else if(dash && dash_duration >= max_dash_duration && !dash_jump) {
            //essentially post-dash cleanup. Skipped if dash-jumping
            dash_duration = 0f;
            dash = false;
            speed_mod = original_speed_mod;
            movement.x = Mathf.Clamp(movement.x, -original_speed_mod, original_speed_mod);
            sprite_rend.sprite = walk_sprite;

        } else {
            dash_cooldown += Time.deltaTime;
            dash_cooldown = Mathf.Clamp(dash_cooldown, 0f, max_dash_cooldown);
        }

        //ceiling detection
        RaycastHit2D ceiling = Physics2D.CircleCast(
            transform.position, 
            0.4f, 
            Vector2.up, 
            0.62f, 
            LayerMask.GetMask("Ground")
        );
        if(ceiling.collider != null) {
            velocity = 0f;
        }

        //flip sprite depending on move direction
        if(Input.GetAxisRaw("Horizontal") > 0) {
            sprite_rend.flipX = true;
        } else if(Input.GetAxisRaw("Horizontal") < 0) {
            sprite_rend.flipX = false;
        }

        //dash trail
        if(dash) {

            if(dash_trail_spawn >= dash_spawn_rate) {
                GameObject dash_trail;
                dash_trail = new GameObject("Dash Trail");
                dash_trail.AddComponent<SpriteRenderer>();
                dash_trail.AddComponent<DashTrailFade>();
                dash_trail.GetComponent<SpriteRenderer>().sprite = dash_trail_sprite;
                dash_trail.GetComponent<SpriteRenderer>().flipX = sprite_rend.flipX;
                dash_trail.transform.position = new Vector3(transform.position.x, transform.position.y, 1);
                dash_trail_spawn = 0f;

            } else {
                dash_trail_spawn += Time.deltaTime;
            }

        } else {
            dash_trail_spawn = dash_spawn_rate;
        }
    }

    // Physics update
    void FixedUpdate() {
        //gravity
        RaycastHit2D ground = Physics2D.CircleCast(
            transform.position, 
            0.4f, 
            Vector2.down, 
            0.62f, 
            LayerMask.GetMask("Ground")
        );

        if(ground.collider == null && !dash) {
            velocity -= gravity_mod * Time.fixedDeltaTime;

        } else if(dash) {
            velocity = 0f;

        } else {
            velocity = Mathf.Clamp(velocity, 0, Mathf.Infinity);
            double_jump = true;
        }

        velocity = Mathf.Clamp(velocity, -fall_speed_limit, Mathf.Infinity);
        
        //movement
        movement.y += velocity;
        rb.MovePosition((Vector2)rb.position + new Vector2(movement.x * Time.fixedDeltaTime, movement.y));
        //essentially forces the program to wait until dash-jump is completed before resetting movement speed
        if(dash_jump) {
            dash_jump = false;
            speed_mod = original_speed_mod;
        }
    }
}