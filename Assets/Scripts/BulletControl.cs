using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{   //开始时间
    private float startTime = 0f;
    //存活时间
    private float liveTime = 0.8f;

    private float flyTime = 0.5f;
    [HideInInspector]
    public bool isDestory = false;
    private float speed = 5f;

    //子弹方向
    [HideInInspector]
    public Vector3 direction;
    public bool isBigBullet = false;
    public bool isFlyUp = false;
    [HideInInspector]
    private Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.isKinematic = false;
        if(this.isFlyUp)
        {
            this.speed = this.speed * 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime < flyTime)
        {
            //飞行一会
            rigidbody.isKinematic = true;
            this.transform.Translate(new Vector2(-direction.x, 0) * Time.deltaTime * speed);
        }
        else
        {
            rigidbody.isKinematic = false;
            this.transform.Translate(new Vector2(-direction.x, 0) * Time.deltaTime * speed * 0.9f);
        }


        if (Time.time - startTime > liveTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);
    }

}
