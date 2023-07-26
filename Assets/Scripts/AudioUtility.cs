using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtility : MonoBehaviour
{
    public static List<AudioClip> sounds;
    [SerializeField]
    List<AudioClip> clips;
    void Start()
    {
        sounds = clips;
    }
}
