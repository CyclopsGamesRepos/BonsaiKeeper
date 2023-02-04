using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeevilMovement : MonoBehaviour
{
    public RootGenerator rootGenerator;
    [SerializeField] float speed = 2.0f;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {

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
            RootEventHandler rootEventData = other.gameObject.GetComponent<RootEventHandler>();
            rootGenerator.PoisonRoot(rootEventData.arrayRowPos, rootEventData.arrayColPos);
            Destroy(gameObject);
        }

    } // OnTriggerEnter
}