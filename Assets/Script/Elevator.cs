using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public enum Type
    {
        Type1,
        Type2,
        Type3,
    }
    public Type type;
    Vector2 movePostion;
    [SerializeField]bool isUp;
    [SerializeField]bool isDown;
    float timer = 0.0f;

    
    void Start()
    {
        //transform.position = new Vector2(7, -3.5f);
        if(type == Type.Type1 || type == Type.Type2)
        {
            isUp = true;
        }
        if(type == Type.Type3)
        {
            isDown = true;
        }
    }

    void Update()
    {
        moving();
    }

    private void moving()
    {
        if(type == Type.Type1)
        {
            movePostion = transform.position;
            if (isUp == true)
            {
                movePostion += Vector2.up * Time.deltaTime;
                if (movePostion.y >= 3.4f)
                {
                    movePostion.y = 3.4f;
                    timer += Time.deltaTime;
                    if (timer >= 2.5f)
                    {
                        timer = 0.0f;
                        isUp = false;
                        isDown = true;
                    }
                }
            }
            if (isDown == true)
            {
                movePostion += Vector2.down * Time.deltaTime;
                if (movePostion.y <= -3.5f)
                {
                    movePostion.y = -3.5f;
                    timer += Time.deltaTime;
                    if (timer >= 2.5f)
                    {
                        timer = 0.0f;
                        isDown = false;
                        isUp = true;
                    }
                }
            }
            transform.position = movePostion;
        }

        if(type == Type.Type2)
        {
            movePostion = transform.position;
            if (isUp == true)
            {
                movePostion += Vector2.up * Time.deltaTime;
                if (movePostion.y >= -5.5f)
                {
                    movePostion.y = -5.5f;
                    timer += Time.deltaTime;
                    if (timer >= 2.5f)
                    {
                        timer = 0.0f;
                        isUp = false;
                        isDown = true;
                    }
                }
            }
            if (isDown == true)
            {
                movePostion += Vector2.down * Time.deltaTime;
                if (movePostion.y <= -12.5f)
                {
                    movePostion.y = -12.5f;
                    timer += Time.deltaTime;
                    if (timer >= 2.5f)
                    {
                        timer = 0.0f;
                        isDown = false;
                        isUp = true;
                    }
                }
            }
            transform.position = movePostion;
        }

        if(type == Type.Type3)
        {
            movePostion = transform.position;
            if (isUp == true)
            {
                movePostion += Vector2.up * Time.deltaTime;
                if (movePostion.y >= 3.5f)
                {
                    movePostion.y = 3.5f;
                    timer += Time.deltaTime;
                    if (timer >= 2.5f)
                    {
                        timer = 0.0f;
                        isUp = false;
                        isDown = true;
                    }
                }
            }
            if (isDown == true)
            {
                movePostion += Vector2.down * Time.deltaTime;
                if (movePostion.y <= -7f)
                {
                    movePostion.y = -7f;
                    timer += Time.deltaTime;
                    if (timer >= 2.5f)
                    {
                        timer = 0.0f;
                        isDown = false;
                        isUp = true;
                    }
                }
            }
            transform.position = movePostion;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.SetParent(this.transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.gameObject.transform.SetParent(null);
    }
}
