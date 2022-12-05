using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWallControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != Constants.Tag.Player)
        {
            //进来的任何物体，都杀掉
            Destroy(collision.gameObject);
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != Constants.Tag.Player)
        {
            //进来的任何物体，都杀掉


            Destroy(collision.gameObject);
        }
    }



}
