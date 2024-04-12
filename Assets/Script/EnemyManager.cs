using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    Rigidbody2D rigid;
    Player player;
    SpriteRenderer sr;
    Sprite sprDefault;
    [SerializeField, Tooltip("´ê¾ÒÀ»¶§ Æ¨°Ü³ª°¡´Â Èû")] float enterForce = 0.0f;
    [SerializeField] float MaxHp = 0f;
    [SerializeField]float CurHp = 0f;

    public enum EnemyType
    {
        Ex,
        Type1,
        Type2,
        Type3,
    }

    public EnemyType type;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.gameObject.GetComponent<Player>();
            player.Hit(1);
            Hit(1);
            Vector2 target = collision.transform.position;
            hitDamaged(target);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PBox")
        {
            Hit(1);
            Vector2 target = collision.transform.position;
            hitDamaged(target);
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sprDefault = sr.sprite;
        CurHp = MaxHp;
    }

    private void hitDamaged(Vector2 _target)
    {
        if (type == EnemyType.Ex) return;
        Vector2 curPos = transform.position;
        Vector2 dir = curPos - _target;
        dir.Normalize();
        rigid.AddForce(dir * enterForce, ForceMode2D.Impulse);
    }

    public void Hit(float _damage)
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
