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
    [SerializeField, Tooltip("닿았을때 튕겨나가는 힘")] float enterForce = 0.0f;
    [SerializeField] float MaxHp = 0f;
    [SerializeField] float CurHp = 0f;
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] Collider2D Range;
    [SerializeField] LayerMask layerEnemy;
    [SerializeField] LayerMask layerGround;
    [SerializeField] LayerMask layerPlayer;
    [SerializeField] Collider2D AtkBox1;
    [SerializeField] Collider2D AtkBox2;
    Animator anim;
    [SerializeField]bool isDeath = false;
    bool isHurt = false;
    bool isEnter = false;
    bool Attack1 = false;
    bool Attack2 = false;
    float timer = 0.0f;
    float moveTimer = 0.0f;
    float attackTimer = 0.0f;
    [SerializeField]int attackNum = 0;
    private Vector2 scale;
    [SerializeField,Tooltip("드롭할 아이템")] GameObject dropItem;
    int playerAtk = 0;


    public enum EnemyType
    {
        MushRoom,
        Skeleton,
        Goblin,
        Flyingeye,
        M_Boss,
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
            Hit(playerAtk);
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
        AtkBox1.enabled = false;
        AtkBox2.enabled = false;
        if(scale.x == -1)
        {
            moveSpeed *= -1;
        }
    }
    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if (isDeath == true || isHurt == true) return;
        doAni();
        enterCooltime();
        move();
        atkNum();
        playerAtk = player.RePlayerAtkDamage();
    }

    private void FixedUpdate()
    {
        if (isDeath == true) return;

        if (Range.IsTouchingLayers(layerEnemy) == true)
        {
            turn();
        }
        if(Range.IsTouchingLayers(layerGround) == false)
        {
            turn();
        }

        if (Range.IsTouchingLayers(layerPlayer) == true && attackNum == 0 && type != EnemyType.M_Boss)
        {
            rigid.velocity = Vector3.zero;
            Attack1 = true;
            Invoke("onBox1", 0.5f);
            Invoke("offBox1", 0.7f);
        }
        else if (Range.IsTouchingLayers(layerPlayer) == false && type != EnemyType.M_Boss)
        {
            Attack1 = false;
        }
        if (Range.IsTouchingLayers(layerPlayer) == true && attackNum == 1 && type != EnemyType.M_Boss)
        {
            rigid.velocity = Vector2.zero;
            Attack2 = true;
            Invoke("onBox1", 0.5f);
            Invoke("offBox1", 0.7f);
        }
        else if (Range.IsTouchingLayers(layerPlayer) == false && type != EnemyType.M_Boss)
        {
            Attack2 = false;
        }

        if (Range.IsTouchingLayers(layerPlayer) == true && attackNum == 0 && type == EnemyType.M_Boss)
        {
            rigid.velocity = Vector3.zero;
            Attack1 = true;
            Invoke("onBox1", 0.4f);
            Invoke("offBox1", 0.5f);
        }
        else if (Range.IsTouchingLayers(layerPlayer) == false && type == EnemyType.M_Boss)
        {
            Attack1 = false;
        }
        if (Range.IsTouchingLayers(layerPlayer) == true && attackNum == 1 && type == EnemyType.M_Boss)
        {
            rigid.velocity = Vector2.zero;
            Attack2 = true;
            Invoke("onBox1", 0.4f);
            Invoke("offBox1", 0.5f);
        }
        else if (Range.IsTouchingLayers(layerPlayer) == false && type == EnemyType.M_Boss)
        {
            Attack2 = false;
        }
    }

    private void onBox1()
    {
        AtkBox1.enabled = true;
    }
    private void onBox2()
    {
        AtkBox2.enabled = true;
    }
    private void offBox1()
    {
        AtkBox1.enabled = false;
    }
    private void offBox2()
    {
        AtkBox2.enabled = false;
    }

    private void doAni()
    {
        anim.SetInteger("Move", (int)moveSpeed);
        anim.SetBool("Attack1", Attack1);
        anim.SetBool("Attack2", Attack2);
    }

    private void hitDamaged(Vector2 _target)
    {
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

        sr.color = new Color(1f, 0f, 0f, 1f);
        Invoke("offHit", 0.3f);

        if(CurHp <= 0f)//체력이 0 이하라면
        {
            isDeath = true;
            anim.Play("Death");//사망애니메이션 실행
            //죽은 자리에 아이템 코인 드랍
            CreatItem(transform.position);
        }
    }

    private void CreatItem(Vector2 _pos)
    {
        Instantiate(dropItem, _pos, Quaternion.identity);
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
        rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
        moveTimer += Time.deltaTime;
        if (moveTimer > 4.0f)
        {
            turn();
            moveTimer = 0.0f;
        }
    }

    private void atkNum()
    {
        attackTimer += Time.deltaTime;
        if(attackTimer >= 10.0f)
        {
            attackTimer = 0.0f;
            attackNum = Random.Range(0, 2);
        }
    }

    private void turn()
    {
        scale.x *= -1;
        transform.localScale = scale;
        moveSpeed *= -1;
    }
}
