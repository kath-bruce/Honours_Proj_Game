using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GifScript : MonoBehaviour
{
    VideoPlayer gif;
    RawImage display;

    // Use this for initialization
    void Start()
    {
        gif = GetComponent<VideoPlayer>();
        display = GetComponent<RawImage>();

        gif.Prepare();
    }

    void Update()
    {
        if (gif.isPrepared)
            display.texture = gif.texture;

        if (!gif.isPlaying)
            gif.Play();
    }
}
