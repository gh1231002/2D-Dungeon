
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("보스 스텟")]
    [SerializeField] private float maxHp = 0.0f;
    [SerializeField] private float curHp = 0.0f;
    [SerializeField] private float moveSpeed = 0.0f;
    [SerializeField] private Collider2D Range;
    [SerializeField] private Collider2D AtkBox;
    [SerializeField] private Collider2D meetRange;
    [SerializeField] private LayerMask layerPlayer;
    [SerializeField] private GameObject objSpell;
    [SerializeField] private Transform trsSpell;
    [SerializeField] private int SpellLimit = 0;
    private int SpellCount = 0;

    Animator anim;
    Rigidbody2D rigid;

    bool isDeath = false;
    bool isHurt = false;
    bool Attack1 = false;
    [SerializeField]bool Attack2 = false;
    bool isMeet = false;
    float AttackTimer = 0.0f;
    float ReloadTimer = 0.0f;
    float moveTimer = 0.0f;
    [SerializeField]int AttackNum = 0;
    private Vector2 scale;

    [Header("보스Ui")]
    [SerializeField] Slider slider;
    [SerializeField] Image sliderFillImage;
    [SerializeField] GameObject BossUi;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isHurt = true;
            Hit(1);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isHurt = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PBox")
        {
            isHurt = true;
            Hit(1);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PBox")
        {
            isHurt = false;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        BossUi.SetActive(false);
        AtkBox.enabled = false;
        scale = transform.localScale;
        curHp = maxHp;
        moveSpeed *= -1;
    }

    public void Hit(float _damage)
    {
        curHp -= _damage;
        SetBossHp(curHp, maxHp);
        if (curHp <= 0f)
        {
            isDeath = true;
            anim.Play("Death");
            BossUi.SetActive(false);
        }
    }

    void Update()
    {
        if (isMeet != true) return;
        if (isDeath == true)return;
        //move();
        move2();
        doAni();
        ranAtk();
        onOffUi();
    }

    private void move()
    {
        if(isHurt == true || AttackNum == 1)return;
        GameObject objPlayer = GameObject.Find("Player");
        Player player = objPlayer.GetComponent<Player>();
        Vector3 playerPos = player.transform.position;
        float x = transform.position.x - playerPos.x;
        //rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
        float moveDir = 0;
        if (x > 0)
        {
            moveDir = 1;
        }
        else
        {
            moveDir = -1;
        }

        if(moveDir == -1)
        {
            Vector2 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        else if(moveDir == 1)
        {
            Vector2 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
        rigid.velocity = new Vector2(moveSpeed * moveDir, rigid.velocity.y);
    }

    private void move2()
    {
        if (isHurt == true) return;
        moveTimer += Time.deltaTime;
        rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
        if (moveTimer > 7.0f)
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
    private void doAni()
    {
        anim.SetBool("Hurt", isHurt);
        anim.SetInteger("Move", (int)moveSpeed);
        anim.SetBool("Attack1", Attack1);
        anim.SetBool("Attack2", Attack2);
    }

    private void ranAtk()
    {
        AttackTimer += Time.deltaTime;
        if(AttackTimer >= 10.0f)
        {
            AttackTimer = 0.0f;
            AttackNum = Random.Range(0, 2);
        }
    }

    private void onOffUi()
    {
        if(isMeet == true)
        {
            BossUi.SetActive(true);
            SetBossHp(curHp, maxHp);
        }
    }

    private void Atkspell()
    {
        ReloadTimer += Time.deltaTime;
        if(AttackNum == 1)
        {
            rigid.velocity = Vector2.zero;
            Attack2 = true;
            if (ReloadTimer >= 1.0f)
            {
                ReloadTimer = 0.0f;
                posSpell();
                if (SpellCount >= SpellLimit)
                {
                    SpellCount = 0;
                    Attack2 = false;
                }
            }
        }
    }

    private void SetBossHp(float _curHp, float _maxHp)
    {
        if(slider.maxValue != _maxHp)
        {
            slider.maxValue = _maxHp;
        }

        slider.value = _curHp;
    }

    private void FixedUpdate()
    {
        if (isDeath == true || isHurt == true) return;

        if (meetRange.IsTouchingLayers(layerPlayer) == true)
        {
            isMeet = true;
        }

        if(Range.IsTouchingLayers(layerPlayer) == true)
        {
            rigid.velocity = Vector2.zero;
            Attack1 = true;
            Invoke("onBox", 0.4f);
            Invoke("offBox", 0.7f);
        }
        else if (Range.IsTouchingLayers(layerPlayer) == false)
        {
            Attack1 = false;
        }
        Atkspell();
    }

    private void createSpell(GameObject _obj, Vector3 _pos, Vector3 _rot)
    {
        GameObject obj = Instantiate(_obj, _pos, Quaternion.Euler(_rot), trsSpell);
    }

    private void posSpell()
    {
        GameObject objPlayer = GameObject.Find("Player");
        Player player = objPlayer.GetComponent<Player>();
        Vector3 playerPos = player.transform.position;
        createSpell(objSpell, playerPos + new Vector3(0, 3, 0), Vector3.zero);
        SpellCount++;
    }

    private void onBox()
    {
        AtkBox.enabled = true;
    }
    private void offBox()
    {
        AtkBox.enabled = false;
    }
}
