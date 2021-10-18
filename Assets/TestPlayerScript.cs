using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerScript : MonoBehaviour
{
    Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 world_to_mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (world_to_mouse - (Vector2)transform.position).normalized;
        transform.up = direction;

        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        body.MovePosition((Vector2)transform.position + movement * Time.deltaTime * 100);
    }
}
