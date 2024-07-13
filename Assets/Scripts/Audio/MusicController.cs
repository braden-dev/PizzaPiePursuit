using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] initialMusic; // Array to hold the initial music clips
    public AudioClip inMissionMusic; // Music clip for the mission
    public MissionController missionController;

    private bool inMission = false;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (missionController == null)
        {
            Debug.LogError("MissionController object not set for MusicController.");
        }

        audioSource.volume = 0.04f;

        StartCoroutine(PlayInitialMusicLoop());
    }

    private void Update()
    {
        bool isInMission = missionController.GetIsInMission();

        if (isInMission != inMission)
        {
            inMission = isInMission;

            if (inMission)
            {
                StartMissionMusic();
            }
            else
            {
                StartCoroutine(PlayInitialMusicLoop());
            }
        }
    }

    private IEnumerator PlayInitialMusicLoop()
    {
        while (!inMission)
        {
            foreach (AudioClip clip in initialMusic)
            {
                audioSource.clip = clip;
                audioSource.Play();
                yield return new WaitForSeconds(clip.length);

                if (inMission)
                {
                    break;
                }
            }
        }
    }

    public void StartMissionMusic()
    {
        StopAllCoroutines();
        audioSource.clip = inMissionMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
