using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GifScript : MonoBehaviour
{
    VideoClip temp;

    VideoPlayer gif;
    RawImage display;

    // Use this for initialization
    void Awake()
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

    public void SwitchGif(VideoClip newGif)
    {
        temp = gif.clip;
        gif.clip = newGif;
        newGif = temp;
    }
}
