using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BallState
{
    //半球状态
    HalfBall,
    //满球状态
    FallingBall,
    //滚动状态
    RollBall
}

public class BallControl : MonoBehaviour
{


    private int MaxHp = 2;
    private int hp = 0;

    private float intervalTime = 2f;

    private float tickTime = 0f;


    public GameObject EnemyPerfeb;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    //是否滚动中
    private bool isRolling = false;
    //满球状态倒计时 2*tick
    private int fillingTicks = 2;
    //移动方向
    private Vector2 moveDir;

    private float rollSpeed = 5;

    private BallState currentState = BallState.HalfBall;
    public GameObject[] rewards;
    void Start()
    {
        this.animator= GetComponent<Animator>();
        tickTime = Time.time;
        this.gameObject.layer = LayerMask.NameToLayer("HalfBall");
        boxCollider2D = GetComponent<BoxCollider2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case BallState.HalfBall:
            case BallState.FallingBall:
                if (Time.time - tickTime > intervalTime)
                {
                    this.tickTime = Time.time;
                    this.tick();
                }
                break;
            case BallState.RollBall:
                if (moveDir != null)
                {
                    this.transform.Translate(moveDir * rollSpeed * Time.deltaTime);
                }
                break;
        }
        
    }

    private void tick()
    {
        switch (currentState)
        {
            case BallState.HalfBall:
                this.gameObject.layer = LayerMask.NameToLayer("HalfBall");
                this.hp -= 1;
                this.animator.SetInteger("hp", this.hp);
                if (this.hp < 0)
                {
                    createEnemy();
                    Destroy(this.gameObject);
                }
                break;
            case BallState.FallingBall:
                if (fillingTicks > 0)
                {
                    fillingTicks--;
                    this.gameObject.layer = LayerMask.NameToLayer("FallingBall");
                }
                else
                {
                    this.hp = 1;
                    this.animator.SetInteger("hp", this.hp);
                    currentState = BallState.HalfBall;
                    this.gameObject.layer = LayerMask.NameToLayer("HalfBall");
                }
                
                break;
            case BallState.RollBall:

                break;

        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (currentState)
        {
            case BallState.HalfBall:
                if (collision.gameObject.tag == Constants.Tag.Bullet)
                {
                    
                    if (this.hp < MaxHp)
                    {
                        this.hp += 1;
                        this.animator.SetInteger("hp", this.hp);
                        this.tickTime = Time.time;
                    }
                    if (this.hp >= MaxHp)
                    {
                        this.hp = MaxHp;
                        fillingTicks = 2;
                        currentState = BallState.FallingBall;
                        this.gameObject.layer = LayerMask.NameToLayer("FallingBall");
                    }
                }
                
                break;
            case BallState.RollBall:
                if (collision.gameObject.tag == Constants.Tag.Boss)
                {
                    Destroy(this.gameObject);
                }

                if(collision.gameObject.tag == Constants.Tag.GhostWall)
                {
                    Debug.Log($"撞到南墙，反转 {this.moveDir}=> {-this.moveDir} ");
                    if(this.moveDir != null)
                    {
                        this.moveDir = -moveDir;
                    }
                }

                break;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (currentState)
        {
            case BallState.HalfBall:
                Debug.Log("半球状态,被撞到" + collision.gameObject.tag);
                if (collision.gameObject.tag == Constants.Tag.RollBall)
                {
                    CreateRandPotition();
                    Destroy(this.gameObject);
                }
                break;
            case BallState.FallingBall:
                break;
            case BallState.RollBall:
                
                if (collision.gameObject.tag == Constants.Tag.GhostWall)
                {
                    Debug.Log($"撞到南墙，反转 {this.moveDir}=> {-this.moveDir} ");
                    if (this.moveDir != null)
                    {
                        this.moveDir = -moveDir;
                    }
                }
                break;

        }
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == Constants.Tag.RollBall && this.currentState == BallState.FallingBall)
        {
            //当前处于满球状态，并且被滚动的球撞到
            BallControl ballControl = collision.gameObject.GetComponent<BallControl>();
            this.firePush(ballControl.moveDir);
            ballControl.moveDir = -ballControl.moveDir;
        }
        if(collision.gameObject.tag == Constants.Tag.GhostWall && collision.gameObject.name == "LeftWall")
        {
            if (this.moveDir.x < 0)
            {
                this.moveDir = -this.moveDir;
            }
        }
        if (collision.gameObject.tag == Constants.Tag.GhostWall && collision.gameObject.name == "RightWall")
        {
            if (this.moveDir.x > 0)
            {
                this.moveDir = -this.moveDir;
            }
        }
        
        if(currentState == BallState.RollBall && collision.gameObject.tag == Constants.Tag.Boss)
        {
            Destroy(this.gameObject);
        }
    }

    public void pushBall()
    {
        if(this.hp >= MaxHp)
        {
            this.isRolling = true;
        }
       
    }
    public void releaseBall()
    {
        this.isRolling = false;
    }

    public void firePush(Vector2 vector)
    {   
        if(this.currentState == BallState.FallingBall)
        {
            this.animator.SetTrigger("Rolling");
            //rigidbody2D.AddForce(vector * 500f);
            this.gameObject.tag = Constants.Tag.RollBall;
            this.gameObject.layer = LayerMask.NameToLayer("RollBall");
            this.currentState = BallState.RollBall;
            this.moveDir = new Vector2(vector.x, 0);
        }
       
    }

    private void createEnemy()
    {
        GameObject enemy = Instantiate(this.EnemyPerfeb, this.transform.parent);
        enemy.transform.localPosition = this.transform.localPosition;
    }


    private void CreateRandPotition()
    {
        GameObject reward = this.rewards[Random.Range(0, this.rewards.Length)];
        GameObject newReward = Instantiate(reward, this.transform.parent);
        newReward.transform.localPosition = this.transform.localPosition;
    }
}
