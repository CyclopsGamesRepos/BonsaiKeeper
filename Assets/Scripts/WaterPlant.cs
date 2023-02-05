using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlant : MonoBehaviour
{
    // field to adjust water level
    [Range(0f, 1f)]
    [SerializeField] float waterMultiplier = .05f;

    // private variables for this script
    private GameManager gameManager;
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
            if (Input.GetMouseButton(0))
            {
                // adjust the water in some way and reset the timer
                gameManager.waterLevel += waterMultiplier * Time.deltaTime;

                if (gameManager.waterLevel > 1)
                {
                    gameManager.waterLevel = 1;
                }
            }
            else
            {
                watering = false;
                gameManager.HideWaterCan();
            }
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
            gameManager.ShowIcon(false);
        }

    } // end OnMouseDown
}
