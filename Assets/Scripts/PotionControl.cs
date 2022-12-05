using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Potion
{
    BluePotion,
    RedPotion,
    YellowPotion,
    GreenPotion,
    Candy1,
    Candy2,
    Candy3,
    Candy4,
    Candy5,
    Candy6
}
public class PotionControl : MonoBehaviour
{
    public Potion PotionType;
    //添加的分数
    public int score = 0;

    public GameObject flyScore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == Constants.Tag.Player)
        {

            PlayerControl playerControl=  collision.gameObject.GetComponent<PlayerControl>();
            int showScore = 0;
            switch (this.PotionType)
            {
                
                case Potion.BluePotion:
                    //蓝色: 增加子弹效果，效果增加一倍
                    showScore = playerControl.powerUp(this.score);
                    break;
                case Potion.RedPotion:
                    //红色：玩家速度提升一倍
                    showScore = playerControl.speedUp(this.score);
                    break; 
                case Potion.YellowPotion:
                    //黄色:延长子弹飞行时间，增加50%
                    showScore = playerControl.bulletFlyUp(this.score);
                    break;
                case Potion.GreenPotion:
                    //绿色药水+10s无敌时间
                    playerControl.invincible(10f);
                    break;
                case Potion.Candy1:
                case Potion.Candy2:
                case Potion.Candy3:
                case Potion.Candy4:
                case Potion.Candy5:
                case Potion.Candy6:
                    showScore = playerControl.scoreUp(this.score);
                    break;
            }
            if(showScore > 0)
            {
                showFlyScore();
            }
            Destroy(this.gameObject); 
        }
    }


    private void showFlyScore()
    {
        GameObject gameObject = Instantiate(flyScore, transform.parent);
        gameObject.transform.position = this.transform.position;
    }

}
