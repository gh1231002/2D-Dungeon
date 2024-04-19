using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    Rigidbody2D rigid;
    Player player;
    SpriteRenderer sr;
    Sprite sprDefault;
    [SerializeField, Tooltip("´ê¾ÒÀ»¶§ Æ¨°Ü³ª°¡´Â Èû")] float enterForce = 0.0f;
    [SerializeField] float MaxHp = 0f;
    [SerializeField] float CurHp = 0f;
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] Collider2D Range;
    [SerializeField] LayerMask layerEnemy;
    [SerializeField] LayerMask layerGround;
    [SerializeField] LayerMask layerPlayer;
    Animator anim;
    bool isDeath = false;
    bool isHurt = false;
    bool isEnter = false;
    bool isAttack = false;
    float timer = 0.0f;
    float moveTimer = 0.0f;
    float attackTimer = 0.0f;
    private Vector2 scale;


    public enum EnemyType
    {
        Ex,
        MushRoom,
        Skeleton,
        Goblin,
        Flyingeye,
        Boss,
    }

    public EnemyType type;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isEnter = true;
            isHurt = true;
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
            isEnter = true;
            isHurt = true;
            Hit(1);
            Vector2 target = collision.transform.position;
            hitDamaged(target);
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        sprDefault = sr.sprite;
        CurHp = MaxHp;
        scale = transform.localScale;
        if (type == EnemyType.Ex)
        {
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        if(scale.x == -1)
        {
            moveSpeed *= -1;
        }
    }

    private void Update()
    {
        if (type == EnemyType.Ex || isDeath == true) return;
        doAni();
        enterCooltime();
        move();
    }

    private void FixedUpdate()
    {
        if (type == EnemyType.Ex) return;
        if(Range.IsTouchingLayers(layerEnemy) == true && type != EnemyType.Boss)
        {
            turn();
        }
        if(Range.IsTouchingLayers(layerGround) == false && type != EnemyType.Boss)
        {
            turn();
        }
    }

    private void doAni()
    {
        anim.SetBool("Hurt", isHurt);
        anim.SetInteger("Move", (int)moveSpeed);
    }

    private void hitDamaged(Vector2 _target)
    {
        if (type == EnemyType.Ex) return;

        Vector2 curPos = transform.position;
        Vector2 dir = curPos - _target;
        dir.Normalize();
        rigid.AddForce(dir * enterForce, ForceMode2D.Impulse);

        Invoke("offHurt", 0.5f);
    }

    private void offHurt()
    {
        isHurt = false;
    }

    public void Hit(float _damage)
    {
        CurHp -= _damage;
        if (type == EnemyType.Ex)
        {
            sr.color = new Color(1f, 0f, 0f, 1f);
            Invoke("offHit", 0.3f);
        } 

        if(CurHp <= 0f)
        {
            if(type != EnemyType.Ex)
            {
                isDeath = true;
                anim.Play("Death");
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void offHit()
    {
        sr.color = Color.white;
    }

    private void enterCooltime()
    {
        if (isEnter == true)
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                timer = 0.0f;
                isEnter = false;
            }
        }
    }

    private void move()
    {
        if (isHurt == true) return;
        rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
        moveTimer += Time.deltaTime;
        if (moveTimer > 5.0f)
        {
            turn();
            moveTimer = 0.0f;
        }
    }

    private void turn()
    {
        scale.x *= -1;
        transform.localScale = scale;
        moveSpeed *= -1;
    }
}
