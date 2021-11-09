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
    //public vars
    public float gravity_mod = 1f;
    public float jump_height = 2f;
    public float speed_mod = 10f;
    Text txt; //placeholder
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        txt = GameObject.Find("Placeholder").GetComponent<Text>();
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

        //placeholder sprint
        if(Input.GetKey(KeyCode.LeftShift)) {
            speed_mod = 13f * 2;
        } else {
            speed_mod = 13f;
        }

        //movement
        movement.y = 0f;
        Debug.DrawRay(transform.position, Vector2.down, Color.green);
        if(ground.collider != null) {
            movement.x = Input.GetAxisRaw("Horizontal");

        } else {
            movement.x += Input.GetAxisRaw("Horizontal") * Time.deltaTime * 5;
            movement.x = Mathf.Clamp(movement.x, -1f, 1f);
            if(Input.GetAxisRaw("Horizontal") == 0) {
                if(movement.x > 0) {
                    movement.x -= Time.deltaTime * 3;
                } else if(movement.x < 0) {
                    movement.x += Time.deltaTime * 3;
                }
            }
        }
        if(movement.x != 0) {
            Debug.Log("Moving");
        }
        //jump
        if(Input.GetButtonDown("Jump")) {
            if(ground.collider != null) {
                jump = true;
                Debug.Log("JUMP YOU HECKER JUMP");
            } else if(double_jump) {
                jump = true;
                double_jump = false;
                Debug.Log("Double jumping breaks the laws of physics.");
            }
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
            Debug.Log("On ground");
            velocity = 0f;
            double_jump = true;
        }
        //jumping
        if(jump == true) {
            velocity = jump_height;
            jump = false;
        }
        //movement
        movement.y += velocity;
        //txt.text = new Vector3(movement.x * speed_mod, movement.y, 0).ToString(); //debugging movement vector
        rb.MovePosition((Vector2)rb.position + new Vector2(movement.x * speed_mod * Time.fixedDeltaTime, movement.y));
    }
}