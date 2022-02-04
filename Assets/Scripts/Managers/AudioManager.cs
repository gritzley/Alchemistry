using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class AudioManager : MonoBehaviour
{
    void Start()
    {
    }
    public static void PlaySoundAtLocationOnBeat(AudioClip clip, Vector3 location)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, location);
        }
    }

    IEnumerator WaitForBeatToPlay(AudioClip clip, Vector3 location)
    {
        int lastBeat = (int)Time.time;
        while (true)
        {
            yield return new WaitUntil(() =>
            {
                if (Time.time > lastBeat)
                {
                    lastBeat = (int)Time.time + 1;
                    return true;
                }
                return false; 
            });
            AudioSource.PlayClipAtPoint(clip, location);
        }
    }
}
