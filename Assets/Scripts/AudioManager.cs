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
    bool mainTheme, dangerTheme, zenTheme;
    bool mainPlayed, dangerPlayed, zenPlayed;

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
        if (mainTheme)
        {
            if(!mainPlayed)
            {
                Play("MainTheme");
                mainPlayed = true;
            }

        }
        else
        {
            mainPlayed = false;
            Stop("MainTheme");
        }

        //---------------------------------
        if (dangerTheme)
        {
            if (!dangerPlayed)
            {
                Play("DangerTheme");
                dangerPlayed = true;
            }
        }
        else
        {
            Stop("DangerTheme");
            dangerPlayed = false;
        }

        //---------------------------------
        if (zenTheme)
        {
            if (!zenPlayed)
            {
                Play("DefeatEnemy");
                Play("ZenTheme");
                zenPlayed = true;
            }
        }
        else
        {
            Stop("ZenTheme");
            zenPlayed = false;
        }

     
        //------Loops
        // play main theme
        if (Input.GetKeyDown("3"))
        {
            mainTheme = true;

            dangerTheme = false;
            zenTheme = false;
        }

        // play danger theme
        if (Input.GetKeyDown("4"))
        {
            dangerTheme = true;

            mainTheme = false;
            zenTheme = false;
        }

        // play zen theme
        if (Input.GetKeyDown("5"))
        {
            zenTheme = true;

            dangerTheme = false;
            mainTheme = false;
        }

        // stop all loops
        if (Input.GetKeyDown("6"))
        {
            mainTheme = false;
            dangerTheme = false;
            zenTheme = false;
        }




    }

    //--------------------------
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
