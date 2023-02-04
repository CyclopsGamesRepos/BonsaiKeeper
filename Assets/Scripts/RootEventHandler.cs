using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RootEventHandler : MonoBehaviour
{
    // public variables used by this and other scripts
    public RootGenerator rootGenerator;
    public int arrayRowPos;
    public int arrayColPos;

    
    void OnMouseDown()
    {
        // print out the position in the array so we can verify in game
        Debug.Log("root array pos is (" + arrayRowPos + ", " + arrayColPos + ")");

        rootGenerator.DoPrune(arrayRowPos, arrayColPos);

    } // end OnMouseDown

}
