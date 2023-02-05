using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.ComponentModel.Design.Serialization;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    bool mainTheme, dangerTheme, zenTheme, largeTheme;
    bool mainPlayed, dangerPlayed, zenPlayed, largePlayed;

    //---------------
    [Header("Reference")]
    [SerializeField] GameObject gameManager;
    GameManager gameManagerScript;
    string currentState;


    //public enum TreeState
    //{
    //    ROOT,           // 0
    //    SEEDLING,       // 1
    //    SAPLING,        // 2    
    //    SMALL_TREE,     // 3
    //    MEDIUM_TREE,    // 4
    //    OPTIMAL_TREE,   // 5
    //    LARGE_TREE,     // 6
    //    OVERGROWN,      // 7
    //    TOO_BIG,        // 8
    //}


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
        gameManagerScript = gameManager.GetComponent<GameManager>();
        currentState = gameManagerScript.treeState.ToString();


        Play("Atmosphere");
    }

    //----------
    void Update()
    {
        currentState = gameManagerScript.treeState.ToString();


        //-------------------------
        //-------------------------
        // Play Root State Theme
        if (currentState == "ROOT"
            || currentState == "SEEDLING")
        {
            if (!dangerPlayed)
            {
                Play("DangerTheme");
                Play("DangerHit");
                dangerPlayed = true;
            }
        }

        if (currentState != "ROOT" && currentState != "SEEDLING")
        {
            dangerPlayed = false;
            Stop("DangerTheme");
        }



        //-------------------------
        //-------------------------
        // Play Main State Theme
        if (currentState == "SAPLING"
            || currentState == "SMALL_TREE"
            || currentState == "MEDIUM_TREE")
        {
            if (!mainPlayed)
            {
                Play("MainTheme");
                Play("MainHit");
                mainPlayed = true;
            }
        }

        if (currentState != "SAPLING"
            && currentState != "SMALL_TREE"
            && currentState != "MEDIUM_TREE")
        {
            mainPlayed = false;
            Stop("MainTheme");
        }


        //-------------------------
        //-------------------------
        // Play Zen State Theme
        if (currentState == "OPTIMAL_TREE"
            || currentState == "LARGE_TREE")
        {
            if (!zenPlayed)
            {
                Play("ZenTheme");
                Play("ZenHit");
                zenPlayed = true;
            }
        }

        if (currentState != "OPTIMAL_TREE"
            && currentState != "LARGE_TREE")
        {
            zenPlayed = false;
            Stop("ZenTheme");
        }


        //-------------------------
        //-------------------------
        // Play large State Theme
        if (currentState == "OVERGROWN"
            || currentState == "TOO_BIG")
        {
            if (!largePlayed)
            {
                Play("DangerHit");
                Play("LargeTheme");
                largePlayed = true;
            }
        }

        if (currentState != "OVERGROWN"
            && currentState != "TOO_BIG")
        {
            largePlayed = false;
            Stop("LargeTheme");
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
