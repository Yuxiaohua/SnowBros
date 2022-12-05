using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebornControl : MonoBehaviour
{

    public GameObject NickPrefeb;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void reborn()
    {
        GameObject gameObject =  Instantiate(NickPrefeb, this.transform.parent);
        gameObject.transform.position = new Vector3(-1.765443f, -3.638216f,0);
    }

}
