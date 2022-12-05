using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreFly : MonoBehaviour
{

    private float flyTime = 2f;
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;   
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.up * 2f * Time.deltaTime);
        if(Time.time - startTime > this.flyTime)
        {
            Destroy(this.gameObject);
        }
    }


}
