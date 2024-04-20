using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExManager : MonoBehaviour
{
    [SerializeField] private float MaxHp = 0f;
    private float CurHp = 0f;
    SpriteRenderer sr;
    Sprite sprDefault;

    private void Awake()
    {
        CurHp = MaxHp;
        sr = GetComponent<SpriteRenderer>();
        sprDefault = sr.sprite;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PBox")
        {
            hit(1);
        }
    }
    private void hit(float _damage)
    {
        CurHp -= _damage;

        sr.color = new Color(1f, 0f, 0f, 1f);
        Invoke("offHit", 0.3f);

        if(CurHp <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void offHit()
    {
        sr.color = Color.white;
    }
}
