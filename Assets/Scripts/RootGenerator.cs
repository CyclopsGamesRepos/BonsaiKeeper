using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class RootGenerator : MonoBehaviour
{
    // enumerated values
    public enum DirtFillTypes
    {
        Dirt,
        Root,
        Stone
    }

    public enum RootTypes
    {
        Vertical,               // can transition to Vertical, Double or VerticalEnd
        Double,                 // can transition to Horizontal, Elbow, LeftEnd or RightEnd
        HorizontalLeft,         // can transitions to Horizontal, Elbow, LeftEnd of RightEnd
        HorizontalRight,        // can transitions to Horizontal, Elbow, LeftEnd of RightEnd
        ElbowLeft,              // can transistion to Vertical, Double or VerticalEnd
        ElbowRight,             // can transistion to Vertical, Double or VerticalEnd
        VerticalEnd,            // no transision, root is stopped here
        LeftEnd,                // no transision, root is stopped here
        RightEnd                // no transision, root is stopped here
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
        //public int row;
        //public int col;
        public T type;
    }

    // constant values
    public static float MIN_TIME = 0.5f;
    public static float MAX_TIME = 2.5f;
    public static int GROUND_WIDTH = 36;
    public static int GROUND_HEIGHT = 15;
    public static int ROOT_START_COL = 17;

    // serialized variables for this script

    // public variables used by this script and available to others

    // private variables used by this script only
    private RootNode<RootTypes>[,] groundArea;

    /// <summary>
    /// Start is called before the first frame update to set up the root system to grow
    /// </summary>
    void Start()
    {
        groundArea = new RootNode<RootTypes>[GROUND_HEIGHT, GROUND_WIDTH];

        // set up the starting node and timer
        RootNode<RootTypes> beginningRoot = new RootNode<RootTypes>();
        beginningRoot.type = RootTypes.Vertical;
        //beginningRoot.row = 0;
        //beginningRoot.col = ROOT_START_COL;

        // randomize the timer so starts to grow at different rates
        beginningRoot.timeToGrow = Random.Range(MIN_TIME, MAX_TIME);

        groundArea[0, ROOT_START_COL] = beginningRoot;

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

                if ( (groundArea[row, col] != null) && !groundArea[row, col].grown)
                {
                    groundArea[row, col].timeToGrow -= timeDelta;

                    if (groundArea[row, col].timeToGrow <= 0)
                    {
                        switch (groundArea[row, col].type)
                        {
                            // can transition to Vertical, Double or VerticalEnd
                            case RootTypes.Vertical:
                            case RootTypes.ElbowLeft:
                            case RootTypes.ElbowRight:
                                GrowVerticalRoot(groundArea[row, col], row, col);
                                break;

                            // can transition to Horizontal, Elbow, LeftEnd or RightEnd
                            case RootTypes.Double:
                                GrowHorizontalRoot(groundArea[row, col], row, col, true);
                                GrowHorizontalRoot(groundArea[row, col], row, col, false);
                                break;

                            case RootTypes.HorizontalLeft:
                                GrowHorizontalRoot(groundArea[row, col], row, col, true);
                                break;

                            case RootTypes.HorizontalRight:
                                GrowHorizontalRoot(groundArea[row, col], row, col, false);
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

        // Need to check to see if there is a node below and change this one to a vertical stub if so
        if ((row == (GROUND_WIDTH - 1)) || (groundArea[row + 1, col] != null))
        {
            if (rootToGrow.type == RootTypes.Vertical)
            {
                rootToGrow.type = RootTypes.VerticalEnd;
            }
            else if (rootToGrow.type == RootTypes.ElbowLeft)
            {
                rootToGrow.type = RootTypes.LeftEnd;
            }
            else
            {
                rootToGrow.type = RootTypes.RightEnd;
            }

            // TODO: make sure to update the graphics
        }
        else
        {
            // randomize wether we do a vertical or double
            if (Random.Range(0f, 1f) < 0.5f)
            {
                newRoot.type = RootTypes.Vertical;
                newRoot.timeToGrow = growTimer;

                // if we are at the bottom of the area, then the vertical extension is an end
                if (row == GROUND_HEIGHT - 2)
                {
                    newRoot.type = RootTypes.VerticalEnd;
                    newRoot.grown = true;
                }
            }
            else
            {
                newRoot.type = RootTypes.Double;
                newRoot.timeToGrow = growTimer;
            }

            rootToGrow.Down = newRoot;
            groundArea[row + 1, col] = newRoot;
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

        // Need to check to see if there is a node below and change this one to a vertical stub if so
        if ((row == (GROUND_HEIGHT - 1)) || (groundArea[row + 1, col] != null))
        {
            if (rootToGrow.type == RootTypes.Vertical)
            {
                rootToGrow.type = RootTypes.VerticalEnd;
            }
            else if (rootToGrow.type == RootTypes.ElbowLeft)
            {
                rootToGrow.type = RootTypes.LeftEnd;
            }
            else
            {
                rootToGrow.type = RootTypes.RightEnd;
            }

            // TODO: make sure to update the graphics
        }
        else
        {
            // randomize wether we do a vertical or double
            if (Random.Range(0f, 1f) < 0.5f)
            {
                newRoot.type = RootTypes.Vertical;
                newRoot.timeToGrow = growTimer;

                // if we are at the bottom of the area, then the vertical extension is an end
                if (row == GROUND_HEIGHT - 2)
                {
                    newRoot.type = RootTypes.VerticalEnd;
                    newRoot.grown = true;
                }
            }
            else
            {
                newRoot.type = RootTypes.Double;
                newRoot.timeToGrow = growTimer;
            }

            rootToGrow.Down = newRoot;
            groundArea[row + 1, col] = newRoot;
        }

    } // end GrowVerticalRoot

}
