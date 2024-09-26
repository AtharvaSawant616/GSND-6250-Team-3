using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTensionMusic : MonoBehaviour
{
    public AudioSource playBGM;
    // Start is called before the first frame update
    void Start()
    {
        playBGM = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
