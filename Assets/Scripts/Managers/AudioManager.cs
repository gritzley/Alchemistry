using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class AudioManager : MonoBehaviour
{
    public static void PlaySoundAtLocationOnBeat(AudioClip clip, Vector3 location)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, location);
        }
    }
}
