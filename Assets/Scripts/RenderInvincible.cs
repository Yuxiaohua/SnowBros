using UnityEngine;

public class RenderInvincible : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    private bool isOpen = false;

    private Color originColor;

    private int currentFrame = 0;

    private Color[] colors = {Color.yellow,Color.green,Color.blue};

    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        originColor = this.spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isOpen)
        {
            drawInvincible(); 
        }
    }


    //绘制无敌效果
    private void drawInvincible()
    {
        //Debug.Log($"当前时间：{Time.time*10}");
        int frame = (int)(Time.time * 10) % colors.Length;
        if(currentFrame != frame)
        {
            this.currentFrame = frame;
            spriteRenderer.color = colors[currentFrame]; 
        }
        
    }

    public bool isOpened()
    {
        return this.isOpen;
    }
    
    public void open()
    {
        this.isOpen = true;
    }

    public void close()
    {
        this.isOpen = false;
        this.spriteRenderer.color = originColor;
    }




}
