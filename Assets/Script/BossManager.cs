
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
    bool Attack2 = false;
    bool isMeet = false;
    float AttackTimer = 0.0f;
    [SerializeField]int AttackNum = 0;

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
        AtkBox.enabled = false;
        curHp = maxHp;
        moveSpeed *= -1;
        BossUi.SetActive(false);
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
        move();
        doAni();
        ranAtk();
        onOffUi();
    }

    private void move()
    {
        if(isHurt == true)return;
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
        if(AttackTimer >= 6.0f)
        {
            AttackNum = Random.Range(0, 2);
            AttackTimer = 0.0f;
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
        if (isDeath == true) return;

        if (meetRange.IsTouchingLayers(layerPlayer) == true)
        {
            isMeet = true;
        }

        if(Range.IsTouchingLayers(layerPlayer) == true && AttackNum == 0)
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

        if (meetRange.IsTouchingLayers(layerPlayer) == true && Range.IsTouchingLayers(layerPlayer) == false && AttackNum == 1)
        {
            rigid.velocity = Vector2.zero;
            Attack2 = true;
            posSpell();
            if (SpellCount >= SpellLimit)
            {
                SpellCount = 0;
                Attack2 = false;
                AttackNum = Random.Range(0, 2);
            }
        }
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
        //createSpell(objSpell, playerPos + new Vector3(-3,3,0), Vector3.zero);
        createSpell(objSpell, playerPos + new Vector3(0,3,0), Vector3.zero);
        //createSpell(objSpell, playerPos + new Vector3(3,3,0), Vector3.zero);
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
