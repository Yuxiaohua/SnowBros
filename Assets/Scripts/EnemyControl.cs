using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private Animator animator;

    [HideInInspector]
    public Vector3 scaleRight = new Vector3(-1, 1, 1);
    [HideInInspector]
    public Vector3 scaleLeft = new Vector3(1, 1, 1);

    private bool isLeft = false;
    private float speed = 1f;
    public GameObject BallPrefeb;

    public GameObject[] rewards;

    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponent<Animator>();
        randDirection();
    }

    // Update is called once per frame
    void Update()
    {
        move();   
    }
    
    private void move()
    {
        bool isGround = this.animator.GetBool("IsGround");
        if (isGround)
        {
            this.animator.SetBool("Move", true);
            this.animator.SetFloat("Speed", 1);
            //移动
            if (isLeft)
            {
                //往左移动
                this.transform.Translate(Vector3.left * speed * Time.deltaTime);
            }
            else
            {
                //往由移动
                this.transform.Translate(Vector3.right * speed * Time.deltaTime);
            }
        }
        else
        {
            this.animator.SetBool("Move", false);
            this.animator.SetFloat("Speed", 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == Constants.Tag.Ground)
        {
            //落到地面
            this.animator.SetBool("IsGround",true);
            //随机一个方向
            randDirection();
        }
        if (collision.gameObject.tag == Constants.Tag.DeathWall)
        {
            //死亡
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == Constants.Tag.GhostWall)
        {
            //反向
            isLeft = !isLeft;
            if (this.transform.localScale == this.scaleLeft)
            {
                this.transform.localScale = this.scaleRight;
            }
            else
            {
                this.transform.localScale = this.scaleLeft;
            }
        }

        if (collision.gameObject.tag == Constants.Tag.RollBall)
        {
            Debug.Log("被雪球撞到");
            //被雪球撞到,随机一个药水出来
            CreateRandPotition();
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Constants.Tag.Ground)
        {
            //落到地面
            this.animator.SetBool("IsGround", false);
        }
        
    }


    private void CreateRandPotition()
    {
        GameObject reward = this.rewards[Random.Range(0, this.rewards.Length)];
        GameObject newReward = Instantiate(reward, this.transform.parent);
        newReward.transform.localPosition = this.transform.localPosition;
    }
    

    private void randDirection()
    {
        //随机一个方向
        float randValue = Random.RandomRange(0, 1f);
        if (randValue > 0.5)
        {
            this.transform.localScale = scaleLeft;
            isLeft = true;
        }
        else
        {
            this.transform.localScale = scaleRight;
            isLeft = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == Constants.Tag.Bullet)
        {
            createBall();
            Destroy(this.gameObject);
        }
    }

    private void createBall()
    {
        GameObject ball =  Instantiate(this.BallPrefeb,this.transform.parent);
        ball.transform.localPosition = this.transform.localPosition;
    }



}

