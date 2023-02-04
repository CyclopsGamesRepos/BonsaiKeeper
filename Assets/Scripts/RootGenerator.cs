using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI.Table;

public class RootGenerator : MonoBehaviour
{
    // enumerated values
    public enum DirtFillTypes       // not currently used
    {
        Dirt,
        Root,
        Stone
    }

    public enum RootTypes
    {
        Vertical,               // can transition to Vertical, Split or VerticalEnd
        Split,                  // can transition to Horizontals, Elbow, LeftEnd or RightEnd (does it twice)
        HorizontalLeft,         // can transitions to HorizontalLeft, Elbow or LeftEnd
        HorizontalRight,        // can transitions to HorizontalRight, Elbow or RightEnd
        ElbowLeft,              // can transistion to Vertical, Split or VerticalEnd
        ElbowRight,             // can transistion to Vertical, Split or VerticalEnd
        VerticalEnd,            // no transision, root is stopped here
        LeftEnd,                // no transision, root is stopped here
        RightEnd,               // no transision, root is stopped here
        SplitLeftEnd,           // no transision, root is stopped here
        SplitRightEnd,          // no transision, root is stopped here
    }

    class RootNode<T>
    {
        public RootNode<T> Parent;
        public RootNode<T> Left;
        public RootNode<T> Down;
        public RootNode<T> Right;
        public float timeToGrow;
        public bool grown;
        public bool infected;
        public T type;
    }

    // constant values for creating roots
    private static float MIN_TIME = 0.5f;
    private static float MAX_TIME = 2.5f;
    private static float HORIZONTAL_CHANCE = 0.65f;
    private static int ROOT_START_COL = 17;

    // constant values for setting up the ground sprites
    private static int GROUND_START_PIXEL_X = 208;
    private static int GROUND_START_PIXEL_Y = 360;
    private static int GROUND_Z_OFFSET = 5;
    private static int GROUND_WIDTH = 36;
    private static int GROUND_HEIGHT = 15;
    private static int GROUND_SPRITE_SIZE = 24;          // ground sprites size in pixels (24x24)

    // serialized variables for this script
    [Header("Ground Area Data")]
    [SerializeField] GameObject soilSprite;
    [SerializeField] GameObject[] rootSprites;

    // public variables used by this script and available to others

    // private variables used by this script only
    private RootNode<RootTypes>[,] groundRoots;
    private GameObject[,] groundSprites;

    /// <summary>
    /// Start is called before the first frame update to set up the root system to grow
    /// </summary>
    void Start()
    {
        InitializeGround();

        groundRoots = new RootNode<RootTypes>[GROUND_HEIGHT, GROUND_WIDTH];

        // set up the starting node and timer
        RootNode<RootTypes> beginningRoot = new RootNode<RootTypes>();
        beginningRoot.type = RootTypes.Vertical;

        // randomize the timer so starts to grow at different rates
        beginningRoot.timeToGrow = Random.Range(MIN_TIME, MAX_TIME);

        groundRoots[0, ROOT_START_COL] = beginningRoot;

        // set up the root sprite at that location
        ChangeRootSprite((int)beginningRoot.type, 0, ROOT_START_COL);

    } // end Start

    /// <summary>
    /// Update is called once per frame - go through all roots to grow and do so if needed
    /// </summary>
    void Update()
    {
        // get the change in time so we don't have to do it for all the roots
        float timeDelta = Time.deltaTime;

        // go through the list of roots to grow and see if it is time to grow them
        for (int row = 0; row < GROUND_HEIGHT; row++)
        {
            for (int col = 0; col < GROUND_WIDTH; col++) {

                if ( (groundRoots[row, col] != null) && !groundRoots[row, col].grown)
                {
                    groundRoots[row, col].timeToGrow -= timeDelta;

                    if (groundRoots[row, col].timeToGrow <= 0)
                    {
                        switch (groundRoots[row, col].type)
                        {
                            // can transition to Vertical, Split or VerticalEnd
                            case RootTypes.Vertical:
                            case RootTypes.ElbowLeft:
                            case RootTypes.ElbowRight:
                                GrowVerticalRoot(groundRoots[row, col], row, col);
                                break;

                            // can transition to Horizontal, Elbow, LeftEnd or RightEnd
                            case RootTypes.Split:
                                GrowHorizontalRoot(groundRoots[row, col], row, col, true);
                                GrowHorizontalRoot(groundRoots[row, col], row, col, false);
                                break;

                            case RootTypes.HorizontalLeft:
                                GrowHorizontalRoot(groundRoots[row, col], row, col, true);
                                break;

                            case RootTypes.HorizontalRight:
                                GrowHorizontalRoot(groundRoots[row, col], row, col, false);
                                break;

                            // these should not grow!
                            default:
                                break;
                        }
                    }
                }
            }

        }
        
    } // end Update

    /// <summary>
    /// Prunes the roots at the given location in the ground array
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void DoPrune(int row, int col)
    {
        // set parent node to growable again
        groundRoots[row, col].Parent.grown = false;
        groundRoots[row, col].Parent.timeToGrow = Random.Range(MIN_TIME, MAX_TIME);

        // cut this node and all following nodes (replacing them with the ground soil sprite)
        PruneRoot(row, col);

    } // end DoPrune

    private void PruneRoot(int row, int col)
    {
        // gram the sprite and root object so we can prune it and its children
        GameObject rootToPrune = groundSprites[row, col];
        RootNode<RootTypes> rootNode = groundRoots[row, col];

        if (rootNode != null)
        {
            // check child left
            if (rootNode.Left != null)
            {
                PruneRoot(row, col - 1);
            }

            // check child below
            if (rootNode.Down != null)
            {
                PruneRoot(row + 1, col);
            }

            // check child right
            if (rootNode.Right != null)
            {
                PruneRoot(row, col + 1);
            }
        }

        // replace this root with soil
        groundRoots[row, col] = null;
        groundSprites[row, col] = Instantiate(soilSprite, rootToPrune.transform.position, Quaternion.identity);
        Destroy(rootToPrune);

    } // end PruneRoot

    /// <summary>
    /// grows the current root horizontally based on the grow criteria
    /// </summary>
    /// <param name="rootToGrow">the current root node to be grown</param>
    /// <param name="row">the row where this root is in the ground array</param>
    /// <param name="col">the col where this root is in the ground array</param>
    private void GrowHorizontalRoot(RootNode<RootTypes> rootToGrow, int row, int col, bool growLeft)
    {
        // mark this as grown
        rootToGrow.grown = true;

        // grab a random time in case we need to make the root grow
        float growTimer = Random.Range(MIN_TIME, MAX_TIME);

        RootNode<RootTypes> newRoot = new RootNode<RootTypes>();

        // set up column and new root based on which way we are growing
        float randomChance = Random.Range(0f, 1f);
        RootTypes newRootType;
        int colToCheck;

        if (growLeft)
        {
            colToCheck = col - 1;

            // randomize wether we do a horizontal left or an elbow
            if (randomChance < HORIZONTAL_CHANCE) {
                newRootType = RootTypes.HorizontalLeft;
            }
            else
            {
                newRootType = RootTypes.ElbowLeft;
            }
        }
        else
        {
            colToCheck = col + 1;

            // randomize wether we do a horizontal left or an elbow
            if (randomChance < HORIZONTAL_CHANCE)
            {
                newRootType = RootTypes.HorizontalRight;
            }
            else
            {
                newRootType = RootTypes.ElbowRight;
            }
        }

        // Need to check to see if there is a node left if we are growing left and change this one to a horizontalLeft stub if so
        if (growLeft && ( (col == 0) || (groundRoots[row, colToCheck] != null) ) )
        {
            // if we are a double coming in, then does it need to change to a vertical end? Or do we need multiple doubles here?
            if (rootToGrow.type == RootTypes.Split)
            {
                rootToGrow.type = RootTypes.SplitLeftEnd;
            }
            else
            {
                rootToGrow.type = RootTypes.LeftEnd;
            }

            // make sure to update the graphics
            ChangeRootSprite((int)rootToGrow.type, row, col);
            groundRoots[row, col].type = rootToGrow.type;
        }
        // Need to check to see if there is a node right if we are growing right and change this one to a horizontalRight stub if so
        else if (!growLeft && ( (col == GROUND_WIDTH - 1) || (groundRoots[row, colToCheck] != null) ))
        {
            // if we are a double coming in, then does it need to change to a vertical end? Or do we need multiple doubles here?
            if (rootToGrow.type == RootTypes.SplitLeftEnd)
            {
                rootToGrow.type = RootTypes.VerticalEnd;
            }
            else if (rootToGrow.type == RootTypes.Split)
            {
                rootToGrow.type = RootTypes.SplitRightEnd;
            }
            else
            {
                rootToGrow.type = RootTypes.RightEnd;
            }

            // make sure to update the graphics
            ChangeRootSprite((int)rootToGrow.type, row, col);
            groundRoots[row, col].type = rootToGrow.type;
        }
        else
        {
            // if we aren't at the edges we can just let the node go
            newRoot.type = newRootType;
            newRoot.timeToGrow = growTimer;

            newRoot.Parent = rootToGrow;
            groundRoots[row, colToCheck] = newRoot;

            // set up the root paths for pruning and infecting
            if (growLeft)
            {
                rootToGrow.Left = newRoot;
            }
            else
            {
                rootToGrow.Right = newRoot;
            }

            // make sure to update the graphics
            ChangeRootSprite((int)newRoot.type, row, colToCheck);
        }

    } // end GrowRoot

    /// <summary>
    /// grows the current root vertically based on the grow criteria
    /// </summary>
    /// <param name="rootToGrow">the current root node to be grown</param>
    /// <param name="row">the row where this root is in the ground array</param>
    /// <param name="col">the col where this root is in the ground array</param>
    private void GrowVerticalRoot(RootNode<RootTypes> rootToGrow, int row, int col)
    {
        // mark this as grown
        rootToGrow.grown = true;

        // grab a random time in case we need to make the root grow
        float growTimer = Random.Range(MIN_TIME, MAX_TIME);

        RootNode<RootTypes> newRoot = new RootNode<RootTypes>();

        // Need to check to see if there is a node below and change this depending on the its type
        if ((row == (GROUND_HEIGHT - 1)) || (groundRoots[row + 1, col] != null))
        {
            // if it was vertical, then make it a vertical stub
            if (rootToGrow.type == RootTypes.Vertical)
            {
                rootToGrow.type = RootTypes.VerticalEnd;
            }
            // if it was an elbow left, just make it end as a horizontal left end
            else if (rootToGrow.type == RootTypes.ElbowLeft)
            {
                rootToGrow.type = RootTypes.LeftEnd;
            }
            // otherwise it must be a right elbow, so make it a horizontal right end
            else
            {
                rootToGrow.type = RootTypes.RightEnd;
            }

            // make sure to update the graphics
            ChangeRootSprite((int)rootToGrow.type, row, col);
        }
        else
        {
            // randomize wether we do a vertical or double
            if (Random.Range(0f, 1f) < HORIZONTAL_CHANCE)
            {
                newRoot.type = RootTypes.Split;
                newRoot.timeToGrow = growTimer;
            }
            else
            {
                newRoot.type = RootTypes.Vertical;
                newRoot.timeToGrow = growTimer;
            }

            newRoot.Parent = rootToGrow;
            rootToGrow.Down = newRoot;
            groundRoots[row + 1, col] = newRoot;

            // make sure to update the graphics
            ChangeRootSprite((int)newRoot.type, row + 1, col);
        }

    } // end GrowVerticalRoot

    /// <summary>
    /// Sets up the ground sprites to be all soil to begin with - a one to one of rootArea
    /// </summary>
    private void InitializeGround()
    {
        Camera cam = Camera.main;

        // first get the camera screen position for where we want the ground sprites to start
        Vector3 screenStartPosition = new Vector3(GROUND_START_PIXEL_X, GROUND_START_PIXEL_Y, GROUND_Z_OFFSET);
        Vector3 groundStartWorldPos = cam.ScreenToWorldPoint(screenStartPosition);

        // move this sprite to that position
        transform.position = groundStartWorldPos;

        // now make all the sprites in the ground area soil to begin with
        groundSprites = new GameObject[GROUND_HEIGHT, GROUND_WIDTH];

        for (int row = 0; row < GROUND_HEIGHT; row++)
        {
            for (int col = 0; col < GROUND_WIDTH; col++)
            {
                Vector3 screenPosition = new Vector3( (screenStartPosition.x + (col * GROUND_SPRITE_SIZE) ),
                                                      (screenStartPosition.y - (row * GROUND_SPRITE_SIZE) ),
                                                       screenStartPosition.z);

                Vector3 spritePos = cam.ScreenToWorldPoint(screenPosition);

                groundSprites[row, col] = Instantiate(soilSprite, spritePos, Quaternion.identity);
            }
        }

    } // end InitializeGround

    /// <summary>
    /// Changes the ground sprite at the given location to the correct root or soil sprite
    /// </summary>
    /// <param name="rootToChange">the integer representation of the root sprite in the araray</param>
    /// <param name="row">the row where the sprite needs to change in the ground sprite array</param>
    /// <param name="col">the col where the sprite needs to change in the ground sprite array</param>
    public void ChangeRootSprite(int rootToChange, int row, int col)
    {
        GameObject oldGroundSprite = groundSprites[row, col];
        GameObject newGroundSprite = soilSprite;

        bool isRoot = (rootToChange >= 0) && (rootToChange < rootSprites.Length);
        // if the value of the root change is not in range, we must leave it as groundSoil
        if (isRoot)
        {
            newGroundSprite = rootSprites[rootToChange];
        }

        groundSprites[row, col] = Instantiate(newGroundSprite, oldGroundSprite.transform.position, Quaternion.identity);
        
        // if it is a root - there is some additional setup for the click events
        if (isRoot)
        {
            groundSprites[row, col].GetComponent<RootEventHandler>().rootGenerator = this;
            groundSprites[row, col].GetComponent<RootEventHandler>().arrayRowPos = row;
            groundSprites[row, col].GetComponent<RootEventHandler>().arrayColPos = col;


            //StartCoroutine(Coroutine_Refresh(groundSprites[row, col]));
        }

        // clean up the old sprite
        Destroy(oldGroundSprite, .05f);

    } // end ChangeRootSprite

    /*IEnumerator Coroutine_Refresh(GameObject obj)
    {
        BoxCollider2D box = obj.GetComponent<BoxCollider2D>();
        box.enabled = false;
        yield return new WaitForSeconds(1f);
        box.enabled = true;
    }*/

}
