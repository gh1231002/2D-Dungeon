using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject G;
    [SerializeField] GameObject ShopPanel;
    bool isEnter;
    bool shoponoff;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            G.SetActive(true);
            isEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            G.SetActive(false);
            isEnter = false;
        }
    }

    private void Awake()
    {
        G.SetActive(false);
        ShopPanel.SetActive(false);
    }

    private void Update()
    {
        if(isEnter == true)
        {
            //���� ��� ������... �������� ���׷��̵� ������... (hp ��������)
            if(Input.GetKeyDown(KeyCode.G) && shoponoff == false)
            {
                ShopPanel.SetActive(true);
                Time.timeScale = 0.0f;
                shoponoff = true;
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && shoponoff == true)
            {
                ShopPanel.SetActive(false);
                Time.timeScale = 1.0f;
                shoponoff = false;
            }
        }
    }
}
