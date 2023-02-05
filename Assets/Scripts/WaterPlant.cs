using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlant : MonoBehaviour
{
    // field to adjust water level
    [Range(0f, 1f)]
    [SerializeField] float waterMultiplier = .01f;

    // private variables for this script
    private GameManager gameManager;
    private float timeWatered;
    private bool watering;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // get access to the game manager so we can pause
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();

    } // end Start

    /// <summary>
    /// Updates the water timer
    /// </summary>
    private void Update()
    {
        if (gameManager.gameRunning && watering)
        {
            timeWatered += Time.deltaTime;
        }
        
    } // end Update

    /// <summary>
    /// Gets the click entry to start water process
    /// </summary>
    void OnMouseDown()
    {
        if (gameManager.gameRunning && !gameManager.gamePaused)
        {
            watering = true;
        }

    } // end OnMouseDown

    /// <summary>
    /// When the user releases the mouse on this object stop watering
    /// </summary>
    private void OnMouseUp()
    {
        if (gameManager.gameRunning && !gameManager.gamePaused)
        {
            watering = false;

            // adjust the water in some way and reset the timer
            gameManager.waterLevel += waterMultiplier * timeWatered;
            timeWatered = 0f;
        }
    }
}
