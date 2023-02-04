using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class WeevilMovement : MonoBehaviour
{
    public RootGenerator rootGenerator;
    [SerializeField] float speed = 0.5f;

    //Sound FX for eating root;
    AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    bool audioPlayed;
    float deathOffset = 0.35f;
    float pan;
    float pitch;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start() 
    {
        // Get audio source component and apply Sound Variables
        audioSource = GetComponent<AudioSource>();
        audioClip = GetComponent<AudioClip>();
        audioPlayed = false;
        pitch = Random.Range(0.8f, 1.1f);

    } // Start

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // keep moving up until we get to the top of the soil
        if (transform.position.y < 0)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }

    } // Update

    /// <summary>
    /// When the weevil collides with somthing, poison it if it is a root
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // if we collide with a root, do attack animation and poison that root
        if (other.gameObject.CompareTag("Root") )
        {
            //---------------
            // Apply Sound FX
            pan = Mathf.Clamp(transform.position.x, -1, 1);
            audioSource.panStereo = pan;
            audioSource.pitch = pitch;

            //Play Audio
            audioSource.Play();
            audioPlayed = true;

            //-----------------------------------------------------------------------------
            // add delay to prevent game object being destroyed before auid has been played
            if (audioPlayed)
            {
                // now destroy the game object
                RootEventHandler rootEventData = other.gameObject.GetComponent<RootEventHandler>();
                rootGenerator.PoisonRoot(rootEventData.arrayRowPos, rootEventData.arrayColPos);
                Destroy(gameObject, deathOffset);
            }

        }

    } // OnTriggerEnter
}
