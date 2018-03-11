using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController INSTANCE { get; private set; }

    [SerializeField]
    List<AudioClip> audioClips;

    Dictionary<string, AudioClip> name_to_clips = new Dictionary<string, AudioClip>();

    AudioSource src;

    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Debug.LogError("MORE THAN ONE AUDIO CONTROLLER!!!!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        src = GetComponent<AudioSource>();

        foreach (AudioClip clip in audioClips)
        {
            name_to_clips.Add(clip.name, clip);
        }
    }

    public void PlayButtonSelectAudio()
    {
        src.clip = name_to_clips["button_select"];
        src.Play();
    }

    public void PlayTaskGenerationAudio()
    {
        src.clip = name_to_clips["task_generation"];
        src.Play();
    }

    public void PlayTaskCompletionAudio()
    {
        src.clip = name_to_clips["task_completion"];
        src.Play();
    }

    public void PlayWinAudio()
    {
        src.clip = name_to_clips["win"];
        src.Play();
    }

    public void PlayLoseAudio()
    {
        src.clip = name_to_clips["lose"];
        src.Play();
    }

    public void PlayEventAudio()
    {
        src.clip = name_to_clips["event"];
        src.Play();
    }
}
