using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RootEventHandler : MonoBehaviour
{
    // public variables used by this and other scripts
    public RootGenerator rootGenerator;
    public bool isMainRoot;
    public int arrayRowPos;
    public int arrayColPos;

    // private variables for this script
    GameManager gameManager;

    /// <summary>
    /// Sets up bacic variables for this creature
    /// </summary>
    private void Start()
    {
        // get access to the game manager so we can pause
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        isMainRoot = false;

    } // end Start

    /// <summary>
    /// Gets the click for this root
    /// </summary>
    void OnMouseDown()
    {
        if (gameManager.gameRunning && !gameManager.gamePaused)
        {
            // print out the position in the array so we can verify in game
            //Debug.Log("root array pos is (" + arrayRowPos + ", " + arrayColPos + ")");

            // Don't prume the main root
            if (!isMainRoot)
            {
                rootGenerator.DoPrune(arrayRowPos, arrayColPos);
                gameManager.ShowIcon(true);
            }
        }

    } // end OnMouseDown

}
