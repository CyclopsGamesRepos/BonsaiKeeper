using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    bool normalTheme, dangerTheme, zenTheme;
    bool normalPlayed, dangerPlayed, zenPlayed;

    // instructions:
    // key 1 - 2 audio hits
    // keys 3 - 5 audio loop themes


    //create an audio source and add to an array
    //---------
    void Awake()
    {
        // prevent duplicate game objects being created;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // prevent audio from restarting if new scene loaded
        DontDestroyOnLoad(gameObject);

        // Loop through sounds and set variables;
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    //---------
    void Start()
    {
        Play("Atmosphere");
    }

    //----------
    void Update()
    {
        //-------------------------
        // grow sound hit
        if (Input.GetKeyDown("1"))
        {
            Play("Grow");
        }

        // defeat enemy hit
        if (Input.GetKeyDown("2"))
        {
            Play("DefeatEnemy");
        }


        //---------------------------------
        if (normalTheme)
        {
            if(!normalPlayed)
            {
                Play("normalTheme");
                normalPlayed = true;
            }

        }
        else
        {
            normalPlayed = false;
            Stop("normalTheme");
        }

        //---------------------------------
        if (dangerTheme)
        {
            if (!dangerPlayed)
            {
                Play("dangerTheme");
                dangerPlayed = true;
            }
        }
        else
        {
            Stop("dangerTheme");
            dangerPlayed = false;
        }

        //---------------------------------
        if (zenTheme)
        {
            if (!zenPlayed)
            {
                Play("zenTheme");
                zenPlayed = true;
            }
        }
        else
        {
            Stop("zenTheme");
            zenPlayed = false;
        }


        //------Loops
        // play normal theme
        if (Input.GetKeyDown("3"))
        {
            normalTheme = true;

            dangerTheme = false;
            zenTheme = false;
        }

        // play danger theme
        if (Input.GetKeyDown("4"))
        {
            dangerTheme = true;

            normalTheme = false;
            zenTheme = false;
        }

        // play zen theme
        if (Input.GetKeyDown("5"))
        {
            zenTheme = true;

            dangerTheme = false;
            normalTheme = false;
        }




    }

    //--------------------------
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        // if no sound found return to prevent error
        if (s == null) { return; }
     
        // play the audio
        s.source.Play();
    }

    //--------------------------
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        // if no sound found return to prevent error
        if (s == null) { return; }

        // play the audio
        s.source.Stop();
    }

    //--------------------------
    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            return;
        }

        s.source.volume = volume;
    }




}
