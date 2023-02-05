using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum TreeState
    {
        ROOT,           // 0
        SEEDLING,       // 1
        SAPLING,        // 2    
        SMALL_TREE,     // 3
        MEDIUM_TREE,    // 4
        OPTIMAL_TREE,   // 5
        LARGE_TREE,     // 6
        OVERGROWN,      // 7
        TOO_BIG,        // 8
    }

    // constant values for the game manager
    private static int[] GROWTH_ROOT_VALUES = { 8, 13, 21, 34, 55, 89, 144, 233, 377 };
    private static float START_WATER_LEVEL = 0.5f;
    private static float START_SUN_LEVEL = 0.5f;
    private static float SUN_DECREASE_MULT = 0.2f;

    private static float GAME_OVER_DELAY = 2.0f;

    // serialized variables for use in this script
    [Header("Data for animations")]
    [SerializeField] Animator treeAnim;
    [SerializeField] Slider waterAnim;
    [SerializeField] Slider sunAnim;
    [SerializeField] TMP_Text pauseScoreText;
    [SerializeField] TMP_Text gameOverScoreText;
    [SerializeField] TMP_Text highScoreText;

    [Header("UI Elements for updating")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameOverScreen;

    // public variables used by other scripts
    public bool gameRunning;
    public bool gamePaused;
    public int currentNumRoots;

    // private variables used by this script
    private TreeState treeState;
    private float waterLevel;
    private float sunLevel;
    private float score;

    /// <summary>
    /// Start is called before the first frame update - basic setup
    /// </summary>
    void Start()
    {
        treeState = TreeState.ROOT;
        waterLevel = START_WATER_LEVEL;
        sunLevel = START_SUN_LEVEL;
        currentNumRoots = 0;
        score = 0;
        gameRunning = false;
        gamePaused = false;

    } // end Start

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // only update the game systems if we are running and the pause button hasn't been pressed
        if (gameRunning && !gamePaused) 
        { 
            // decrease water based on Sun (and number of roots?)
            waterLevel -= (sunLevel * Time.deltaTime * SUN_DECREASE_MULT);

            if (waterLevel < 0)
            {
                waterLevel = 0;
            }

            // update water level sprite variable
            waterAnim.value= waterLevel;

            // TODO: Change root growth based on water levels? sun levels?

            // Change Tree state
            UpdateTreeState();

            // update the score based on time and level of tree (optimal is best)
            score += Time.deltaTime * ((int)TreeState.OPTIMAL_TREE - Mathf.Abs((int)TreeState.OPTIMAL_TREE - (int)treeState) );

            // TODO: Add sun changes (cycle?)
        }

    } // Update

    /// <summary>
    /// Starts the game for real from menu
    /// </summary>
    public void StartGame()
    {
        gameRunning = true;

    } // StartGame

    /// <summary>
    /// Restarts the game by reloading the scene
    /// </summary>
    public void ReStartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    } // ReStartGame

    /// <summary>
    /// Pauses/unpauses the game
    /// </summary>
    public void TogglePauseGame()
    {
        gamePaused = !gamePaused;

    } // end TogglePauseGame

    /// <summary>
    /// Ends the game
    /// </summary>
    public void EndGame()
    {
        // disable the pause button
        pauseScreen.SetActive(false);
        gamePaused = true;

        // mark game as not running
        gameRunning = false;

        // start tree death animation
        treeAnim.SetBool("isDead", true);

        // invoke the game over screen after a delay
        Invoke("ShowGameOver", GAME_OVER_DELAY);

    } // EndGame

    // Starts the game over screen after a delay
    private void ShowGameOver()
    {
        gameOverScreen.SetActive(true);
    }

    /// <summary>
    /// updates the current tree state based on the variables roots, water and sun
    /// </summary>
    private void UpdateTreeState()
    {
        // based on how many roots there are, water level and sun levels
        // for now to get it working base it loosely on num roots (may keep this)
        // using fionacci sequence for now (reverse really)
        if ( (treeState != TreeState.ROOT) && (currentNumRoots < GROWTH_ROOT_VALUES[(int)treeState]) )
        {
            treeState--;
        }
        else if ( (treeState != TreeState.TOO_BIG) && (currentNumRoots > GROWTH_ROOT_VALUES[(int)treeState + 1]) )
        {
            treeState++;
        }

        // set the animation state for the tree
        treeAnim.SetInteger("tree state", (int)treeState);

    } // end UpdateTreeState

}
