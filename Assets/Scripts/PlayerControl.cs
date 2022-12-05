using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Animator animator;
    //横向移动速度
    public static float MoveSpeed = 2f;
    [HideInInspector]
    private Rigidbody2D rigidbody2D;

    [HideInInspector]
    public Vector3 scaleRight = new Vector3(-1, 1, 1);
    [HideInInspector]
    public Vector3 scaleLeft = new Vector3(1, 1, 1);


    private Transform FirePoint;

    public GameObject BulletPrefeb;

    public GameObject BulletBigPrefeb;

    private BallControl ballControl;

    //无敌时间为5秒
    private float invincibleTime = 5f;
    //是否攻击提升
    private bool isPowerUp = false;
    //是否速度提升
    private bool isSpeedUp = false;
    //是否子弹飞行速度提升
    private bool isFlyUp = false;
    private RenderInvincible renderInvincible;
    //角色是否可用
    private bool PlayerEnable = false;

    private Transform checkBallObject;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        FirePoint = this.transform.Find("FirePoint");
        renderInvincible = GetComponent<RenderInvincible>();
        checkBallObject = this.transform.Find("CheckBall");

    }

    // Update is called once per frame
    void Update()
    {
        if (!this.PlayerEnable)
        {
            return;
        }
        refresh();
        move();
        fire();
        checkBall();
    }

    private void checkBall()
    {
        if (!this.animator.GetBool("IsGround"))
        {
            //只有站在地面才检测
            return;
        }
        Vector3 dir = new Vector3(-this.transform.localScale.x,0,0);
        
        Debug.DrawLine(FirePoint.position, checkBallObject.position+dir, Color.red);
        //只检测这个一层
        int FallingBallLayer = LayerMask.GetMask("FallingBall");
        RaycastHit2D hit = Physics2D.Raycast(FirePoint.position, dir, 0.1f, FallingBallLayer);
        //0.1距离内的碰撞到了Ball
        if (hit.collider!=null && hit.collider.gameObject.tag == Constants.Tag.Ball)
        {
            Debug.Log("碰撞检测到了："+hit.collider.gameObject.tag);
            this.ballControl = hit.collider.gameObject.GetComponent<BallControl>();
            this.ballControl.pushBall();
            //撞到雪球
            this.animator.SetBool("Push", true);
        }
        else if(this.ballControl!=null)
        {
            //离开雪球
            this.animator.SetBool("Push", false);
            this.ballControl.releaseBall();
            this.ballControl = null;
        }
        else
        {
            //离开雪球
            this.animator.SetBool("Push", false);
        }

    }


    /**
     * 移动
     */
    private void move()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");
        animator.SetFloat("Speed", Mathf.Abs(Horizontal));
        if (Mathf.Abs(Horizontal) > 0)
        {
            //移动
            this.transform.Translate(new Vector3(MoveSpeed * Time.deltaTime * Horizontal, 0));
            if (Horizontal > 0)
            {
                this.transform.localScale = scaleRight;
            }
            else if (Horizontal < 0)
            {
                this.transform.localScale = scaleLeft;
            }

        }

        if (Input.GetKeyDown(KeyCode.Space) && this.animator.GetBool("IsGround"))
        {
            //跳跃
            rigidbody2D.AddForce(Vector2.up * 300f);
            this.animator.SetTrigger("Jump");
        }
    }
    
    //攻击
    private void fire()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (this.animator.GetBool("Push") && ballControl!=null)
            {
                //推雪球
                ballControl.firePush(new Vector2(-this.transform.localScale.x,0));
                this.animator.SetBool("Push", false);
                ballControl = null;
            }
            else
            {
                //发射子弹
                this.animator.SetTrigger("Fire");
                GameObject bullet;
                if (isPowerUp)
                {
                    bullet = Instantiate(BulletBigPrefeb, this.transform.parent);
                }
                else
                {
                    bullet = Instantiate(BulletPrefeb, this.transform.parent);
                }
                
                bullet.transform.position = FirePoint.position;
                bullet.transform.localScale = this.transform.localScale;
                
                bullet.GetComponent<BulletControl>().direction = this.transform.localScale;
                bullet.GetComponent<BulletControl>().isFlyUp = this.isFlyUp;
            }
           
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //碰撞到了地面，并且速度向下
        if ((collision.gameObject.tag == Constants.Tag.Ground) && this.rigidbody2D.velocity.y <= 0)
        {
            animator.SetBool("IsGround", true);
        }

        if(collision.gameObject.tag == Constants.Tag.Boss || collision.gameObject.tag == Constants.Tag.Enemy)
        {
            if(invincibleTime <= 0)
            {
                animator.SetTrigger("Dead");
            }
           
        }

        if(collision.gameObject.tag == Constants.Tag.Ball)
        {
            //this.ballControl = collision.gameObject.GetComponent<BallControl>();
            //this.ballControl.pushBall();
            ////撞到雪球
            //this.animator.SetBool("Push", true);
        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Constants.Tag.RollBall)
        {
            //被滚动的雪球撞到，增加5秒无敌时间
            this.invincible(5f);
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == Constants.Tag.Ground) && this.rigidbody2D.velocity.y >= 0)
        {
            animator.SetBool("IsGround", false);
        }

        if (collision.gameObject.tag == Constants.Tag.Ball)
        {
            //离开雪球
            //this.animator.SetBool("Push", false);
            //collision.gameObject.GetComponent<BallControl>().releaseBall();
            //this.ballControl = null;
        }
    }

    private void refresh()
    {
        
        if(invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
            if (!this.renderInvincible.isOpened())
            {
                this.renderInvincible.open();
                this.gameObject.layer = LayerMask.NameToLayer("InviciblePlayer");
            }
        }
        else
        {
            if (this.renderInvincible.isOpened())
            {
                this.renderInvincible.close();
                this.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }

        if(this.isSpeedUp)
        {
            MoveSpeed = 4f;
        }
        else
        {
            MoveSpeed = 2f;
        }

    }

    //进入无敌状态
    public void invincible(float addTime)
    {
        Debug.Log("进入无敌状态 + 10秒");
        //加10s的无敌时间
        this.invincibleTime += addTime;
    }


    public int speedUp(int addScore)
    {
        //速度提升,如果已经提升，则加分数
        if (!this.isSpeedUp)
        {
            this.isSpeedUp = true;
            this.animator.SetBool("SpeedUp", true);
        }
        else
        {
            return this.scoreUp(addScore);
        }
        return 0;
       
    }

    public int bulletFlyUp(int addScore)
    {
        if (!this.isFlyUp)
        {
            //攻击提升
            this.isFlyUp = true;
        }
        else
        {
            //如果已经提升，就+2000分
            return this.scoreUp(addScore);
        }
        return 0;
    }


    public int powerUp(int addScore)
    {
        if (!this.isPowerUp)
        {
            //攻击提升
            this.isPowerUp = true;
        }
        else
        {
            //如果已经提升，就+2000分
            return this.scoreUp(addScore);
        }
        return 0;
       
    }

    public int scoreUp(int addScore)
    {
        
        if (UIControl.instance != null)
        {
            UIControl.instance.addScore(addScore);
            return addScore;
        }
        return 0;
    }



    //死亡
    public void Dead()
    {
        Destroy(this.gameObject);
    }
    //复活
    public void reborn()
    {
        if(UIControl.instance != null && UIControl.instance.PlayerHp > 0)
        {
                UIControl.instance.reducePlayerHp();
                GameObject NickPrefeb = Resources.Load<GameObject>("Nick");
                GameObject gameObject = Instantiate(NickPrefeb, this.transform.parent);
                gameObject.transform.position = new Vector3(-1.765443f, -3.638216f, 0);
        }
        
    }


    //由动画事件调用
    public void activePlayer()
    {
        this.PlayerEnable = true;
    }
}
