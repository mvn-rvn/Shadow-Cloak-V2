using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSpikeCol : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col) {
        Debug.Log("qwdww");
        if(col.tag == "Spikes") {
            SceneManager.LoadScene("SampleScene");
        }
    }
}