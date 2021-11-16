using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempRestart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) {
            transform.position = new Vector3(0, 5, 0);
        }
    }
}
