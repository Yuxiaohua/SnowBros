using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIControl : MonoBehaviour
{

    public TMP_Text ScoreText;
    public TMP_Text HpText;
    public TMP_Text BossText;

    public static UIControl instance;

    public int PlayerHp = 10;
    public int ScoreValue = 0;


    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addScore(int score)
    {
        Debug.Log($"totalScore:{ScoreValue} + {score} = {ScoreValue + score}");
        this.ScoreValue += score;
        this.ScoreText.text = $"Score: {this.ScoreValue}";
    }

    public void reducePlayerHp()
    {
        this.HpText.text = $"x {--this.PlayerHp}";
    }

    public void drawBossHp(int hp,int maxHp)
    {
        this.BossText.text = $"Boss: {hp}/{maxHp}";

    }

}
