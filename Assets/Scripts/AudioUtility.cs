using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtility : MonoBehaviour
{
    public static AudioUtility instance;

    public static List<AudioClip> useSounds;
    public static List<AudioClip> walkSounds;
    public static List<AudioClip> talkSounds;

    [SerializeField]
    List<AudioClip> useClips;
    [SerializeField]
    List<AudioClip> walkClips;
    [SerializeField]
    List<AudioClip> talkClips;

    void Awake()
    {
        instance = this;
        useSounds = useClips;
        walkSounds = walkClips;
        talkSounds = talkClips;
    }

    void Start()
    {
        useSounds = useClips;
        walkSounds = walkClips;
        talkSounds = talkClips;
    }
}
