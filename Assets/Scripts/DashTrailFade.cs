using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTrailFade : MonoBehaviour
{
    //normal/private vars
        SpriteRenderer sprite_rend;
        Color tmp;

    // Start is called before the first frame update
    void Start()
    {
        sprite_rend = GetComponent<SpriteRenderer>();
        tmp = sprite_rend.color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tmp.a -= Time.fixedDeltaTime * 10;
        sprite_rend.color = tmp;
    }
}
