using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    Vector2 movePostion;
    [SerializeField]bool isUp;
    [SerializeField]bool isDown;
    float timer = 0.0f;
    void Start()
    {
        //transform.position = new Vector2(7, -3.5f);
        isUp = true;
    }

    void Update()
    {
        moving();
    }

    private void moving()
    {
        movePostion = transform.position;
        if(isUp == true)
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
        if(isDown == true)
        {
            movePostion += Vector2.down * Time.deltaTime;
            if(movePostion.y <= -3.5f)
            {
                movePostion.y = -3.5f;
                timer += Time.deltaTime;
                if(timer >= 2.5f)
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
