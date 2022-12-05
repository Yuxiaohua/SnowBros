using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BossState
{   
    DOWN_IDLE,
    UP_IDLE,
    DEAD
}

public class BossControl : MonoBehaviour
{
    // Start is called before the first frame update


    private Animator animator;
    [HideInInspector]
    private Rigidbody2D rigidbody;
    private float startTime = 0f;
    //时间间隔2s
    private float intervalTime = 2f;

    //最大跳跃次数
    private const int MAX_JUMP_TIMES = 3;
    private BossState currentState = BossState.DOWN_IDLE;
    private int jumpTimes = 0;

    public GameObject EnemyPrefeb;
    private Transform FirePoint;

    private float lastAttackTime = 0f;
    private int MaxHp = 10000;
    private int hp = 10000;
    private RenderInvincible renderInvincible;
    void Start()
    {
        startTime = Time.time;
        animator = this.GetComponent<Animator>();
        rigidbody = this.GetComponent<Rigidbody2D>();
        FirePoint = this.transform.Find("FirePoint");
        renderInvincible = this.GetComponent<RenderInvincible>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - startTime > intervalTime)
        {
            tick();
            startTime = Time.time;
        }
        if (UIControl.instance != null)
        {
            UIControl.instance.drawBossHp(this.hp, MaxHp);
        }
    }

    private void tick()
    {
        Debug.Log("Boss-Tick");
        switch (currentState)
        {
            case BossState.DOWN_IDLE:
                idle();
                break;
            case BossState.UP_IDLE:
                upIdle();
                break;
            case BossState.DEAD:
                break;
        }
    }

    private void idle()
    {
        //跳跃3次之后，跳到上面去
        if (jumpTimes >= MAX_JUMP_TIMES)
        {
            animator.SetTrigger("BigJump");
            currentState = BossState.UP_IDLE;
            jumpTimes = 0;
            rigidbody.AddForce(Vector2.up * 500f);
        }
        else
        {
            animator.SetTrigger("Jump");
            rigidbody.AddForce(Vector2.up * 100f);
            jumpTimes += 1;
        }
    }

    private void upIdle()
    {
        //跳跃3次之后，跳到下面去
        if (jumpTimes >= MAX_JUMP_TIMES)
        {
            animator.SetTrigger("BigJump");
            currentState = BossState.DOWN_IDLE;
            jumpTimes = 0;
            rigidbody.AddForce(Vector2.up * 100f);
            this.gameObject.layer = LayerMask.NameToLayer("BossDown");
        }
        else
        {
            animator.SetTrigger("Jump");
            rigidbody.AddForce(Vector2.up * 100f);
            jumpTimes += 1;
        }
    }

    public void attack()
    {
        //每秒最多出一个儿子
        if(Time.time - lastAttackTime > 0.5f)
        {
            lastAttackTime = Time.time;
            GameObject enemy = Instantiate(EnemyPrefeb, this.transform.parent);
            enemy.transform.position = this.FirePoint.position;
            enemy.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 300f);
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == Constants.Tag.Bullet)
        {
            if (collision.gameObject.GetComponent<BulletControl>().isBigBullet)
            {
                Debug.Log("Boss血量-10");
                this.hp -= 10;
            }
            else
            {
                Debug.Log("Boss血量-1");
                this.hp -= 1;
            }
            if (this.hp <= 0)
            {
                this.hp = 0;
                this.animator.SetBool("Dead", true);
                currentState = BossState.DEAD;
                this.gameObject.tag = Constants.Tag.DeadBoss;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //向下撞到物体
        if (collision.gameObject.tag == Constants.Tag.Ground && this.rigidbody.velocity.y <= 0)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Boss");
            this.animator.SetBool("IsGround", true);
        }
        

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //被雪球撞到
        if (collision.gameObject.tag == Constants.Tag.RollBall)
        {
            Debug.Log("Boss血量-200");
            this.hp -= 200;
            if (this.hp <= 0)
            {
                this.hp = 0;
                this.animator.SetBool("Dead", true);
                currentState = BossState.DEAD;
                this.gameObject.tag = Constants.Tag.DeadBoss;

            }
            else
            {
                renderInvincible.open();
                Invoke("closeRender", 1f);
            }
            Destroy(collision.gameObject);
        }
    }

    private void closeRender()
    {
        this.renderInvincible.close();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //向上离开物体
        if (collision.gameObject.tag == Constants.Tag.Ground && this.rigidbody.velocity.y >= 0)
        {
            this.animator.SetBool("IsGround", false);
        }
    }


    public void destroyBoss()
    {
        Destroy(this.gameObject);
    }

}
