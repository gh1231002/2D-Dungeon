
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField] private float maxHp = 0.0f;
    [SerializeField] private float curHp = 0.0f;
    [SerializeField] private float moveSpeed = 0.0f;

    Animator anim;
    Rigidbody2D rigid;
    private Vector2 scale;
    Player player;

    [Header("���� ���� ����")]
    [SerializeField] private Collider2D Range;
    [SerializeField] private Collider2D AtkBox;
    [SerializeField] private Collider2D turnRange;
    [SerializeField] private Collider2D meetRange;
    [SerializeField] private LayerMask layerPlayer;
    [SerializeField] private LayerMask layerGround;
    [SerializeField] private GameObject objSpell;
    [SerializeField] private Transform trsSpell;
    [SerializeField,Tooltip("cast�����ϴ� Ƚ��")] private int SpellLimit = 0;
    private int SpellCount = 0;
    bool isDeath = false;
    bool isHurt = false;
    bool Attack1 = false;
    bool Attack2 = false;
    bool isMeet = false;//������ �ִ� ���������ȿ� �÷��̾ ���Դ���
    float spellTimer = 0.0f;//�����ð� ������ cast����
    float reloadTimer = 0.0f;//cast���ݰ��� ��Ÿ��
    float rushTimer = 0.0f;//���� ��Ÿ��
    int onSpell = 0;
    float phase1 = 0;
    float phase2 = 0;
    bool isRush = false;

    [Header("����Ui")]
    [SerializeField] Slider slider;
    [SerializeField] Image sliderFillImage;
    [SerializeField] GameObject BossUi;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            isHurt = true;
            player = collision.gameObject.GetComponent<Player>();
            player.Hit(1);
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
            Invoke("offHurt", 0.8f);
        }
    }
    private void offHurt()
    {
        isHurt = false;
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
        phase1 = maxHp * 0.7f;
        phase2 = maxHp * 0.25f;
    }

    public void Hit(float _damage)
    {
        curHp -= _damage;
        SetBossHp(curHp, maxHp);
        if (curHp <= 0f)
        {
            rigid.velocity = Vector2.zero;
            isDeath = true;
            anim.Play("Death");
            BossUi.SetActive(false);
        }
    }

    void Update()
    {
        if (isMeet != true) return;
        if (isDeath == true)return;
        move2();
        doAni();
        onOffUi();
        onOffSpell();
    }
    /// <summary>
    /// �÷��̾ �ִ� �������� �̵��ϴ� �Լ�
    /// </summary>
    //private void move()
    //{
    //    if(isHurt == true || AttackNum == 1)return;
    //    GameObject objPlayer = GameObject.Find("Player");
    //    Player player = objPlayer.GetComponent<Player>();
    //    Vector3 playerPos = player.transform.position;
    //    float x = transform.position.x - playerPos.x;
    //    //rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);
    //    float moveDir = 0;
    //    if (x > 0)
    //    {
    //        moveDir = 1;
    //    }
    //    else
    //    {
    //        moveDir = -1;
    //    }

    //    if(moveDir == -1)
    //    {
    //        Vector2 scale = transform.localScale;
    //        scale.x = -1;
    //        transform.localScale = scale;
    //    }
    //    else if(moveDir == 1)
    //    {
    //        Vector2 scale = transform.localScale;
    //        scale.x = 1;
    //        transform.localScale = scale;
    //    }
    //    rigid.velocity = new Vector2(moveSpeed * moveDir, rigid.velocity.y);
    //}
    /// <summary>
    /// �����ȿ� ���� ��´ٸ� �ݴ�������� �̵�
    /// </summary>
    private void move2()
    {
        if (isHurt == true) return;

        rigid.velocity = new Vector2(moveSpeed, rigid.velocity.y);

        if(turnRange.IsTouchingLayers(layerGround) == true)
        {
            turn();
        }
    }
    private void turn()
    {
        if (Attack2 == true || isRush == true) return;
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

    private void onOffUi()
    {
        if(isMeet == true)
        {
            BossUi.SetActive(true);
            SetBossHp(curHp, maxHp);
        }
    }
    /// <summary>
    /// ü�º� ����
    /// </summary>
    private void phasePattern()
    {
        //ü���� �ִ뿡�� 70�۱���
        if(phase1 < curHp)
        {
            //�÷��̾�� ������ �⺻����
            if (Range.IsTouchingLayers(layerPlayer) == true)
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
        }
        //70~25%
        else if(phase2 < curHp)
        {
            //�⺻����
            if (Range.IsTouchingLayers(layerPlayer) == true)
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
            //�����ð����� �÷��̾� ��ġ�� spell����
            reloadTimer += Time.deltaTime;//spell���� ��Ÿ���� �ֱ����� �ð���
            if(onSpell == 1)//������ �ʸ��� onspell�� 1�� �ٲ����
            {
                Attack2 = true;//�ι�° ���� ����
                if(reloadTimer >= 1.0f)
                {
                    reloadTimer = 0.0f;
                    posSpell();
                    if(SpellCount >= SpellLimit)//������ Ƚ����ŭ spell�� ����ϸ� ������ ��������
                    {
                        SpellCount = 0;
                        onSpell = 0;
                        Attack2 = false;
                    }
                }
            }
            if(Attack2 == true)//�ι�° ������ ����Ǹ� �������� ����
            {
                rigid.velocity = Vector2.zero;
            }
        }
        //25~0%
        else
        {
            //�⺻����
            if (Range.IsTouchingLayers(layerPlayer) == true)
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
            //�����ð����� �÷��̾� ��ġ�� spell����
            reloadTimer += Time.deltaTime;//spell���� ��Ÿ���� �ֱ����� �ð���
            if (onSpell == 1)//������ �ʸ��� onspell�� 1�� �ٲ����
            {
                //���������� �������̸� �۵�����
                if (isRush == true) return;

                Attack2 = true;//�ι�° ���� ����

                if (reloadTimer >= 1.0f)
                {
                    reloadTimer = 0.0f;
                    posSpell();
                    if (SpellCount >= SpellLimit)//������ Ƚ����ŭ spell�� ����ϸ� ������ ��������
                    {
                        SpellCount = 0;
                        onSpell = 0;
                        Attack2 = false;
                    }
                }
            }
            if (Attack2 == true)//�ι�° ������ ����Ǹ� �������� ����
            {
                rigid.velocity = Vector2.zero;
            }

            //���� �ð����� ������
            rushTimer += Time.deltaTime;//���� ��Ÿ��
            if(rushTimer >= 30f)
            {
                //���� �������� �۵����̶�� �۵�����
                if (Attack2 == true) return;

                isRush = true;
                //�������� ����
                rigid.velocity = Vector2.zero;
                //���� �ٶ󺸰��ִ� ���� ������
                float dir = transform.localScale.x;
                //�� �������� ���� ���������� ������
                if(dir == 1)
                {
                    float power = 40f;
                    rigid.AddForce(Vector2.left * power, ForceMode2D.Impulse); 
                }
                else if(dir == -1)
                {
                    float power = 40f;
                    rigid.AddForce(Vector2.right * power, ForceMode2D.Impulse);
                }
                //���� ������ �ݴ�������� ��ȯ �� �������
                if(turnRange.IsTouchingLayers(layerGround) == true)
                {
                    turn();
                    rushTimer = 0.0f;
                    isRush = false;
                }
            }
        }
    }

    private void onOffSpell()
    {
        if(onSpell == 1) return;
        spellTimer += Time.deltaTime;
        if(spellTimer >= 15.0f)
        {
            spellTimer = 0;
            onSpell = 1;
        }
    }
    /// <summary>
    /// �����̴� ui�� ���� ü�°� ����
    /// </summary>
    /// <param name="_curHp"></param>
    /// <param name="_maxHp"></param>
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
        phasePattern();
    }

    private void createSpell(GameObject _obj, Vector3 _pos, Vector3 _rot)
    {
        GameObject obj = Instantiate(_obj, _pos, Quaternion.Euler(_rot), trsSpell);
    }

    private void posSpell()
    {
        GameObject objPlayer = GameObject.Find("Player");//player��� �̸��� ���� ������Ʈ ã��
        Player player = objPlayer.GetComponent<Player>();//player component�� ������
        Vector3 playerPos = player.transform.position;//player ��ġ�� ����
        if(playerPos == null) return;
        createSpell(objSpell, playerPos + new Vector3(0, 3, 0), Vector3.zero);//spell ����
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
