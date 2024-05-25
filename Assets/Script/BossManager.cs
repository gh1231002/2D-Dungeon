
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

    Animator anim;
    Rigidbody2D rigid;
    private Vector2 scale;
    Player player;

    [Header("보스 패턴 관련")]
    [SerializeField] private Collider2D Range;
    [SerializeField] private Collider2D AtkBox;
    [SerializeField] private Collider2D turnRange;
    [SerializeField] private Collider2D meetRange;
    [SerializeField] private LayerMask layerPlayer;
    [SerializeField] private LayerMask layerGround;
    [SerializeField] private GameObject objSpell;
    [SerializeField] private Transform trsSpell;
    [SerializeField,Tooltip("cast공격하는 횟수")] private int SpellLimit = 0;
    private int SpellCount = 0;
    bool isDeath = false;
    bool isHurt = false;
    bool Attack1 = false;
    bool Attack2 = false;
    bool isMeet = false;//보스가 있는 스테이지안에 플레이어가 들어왔는지
    float spellTimer = 0.0f;//일정시간 지난후 cast공격
    float reloadTimer = 0.0f;//cast공격간의 쿨타임
    float rushTimer = 0.0f;//돌진 쿨타임
    int onSpell = 0;
    float phase1 = 0;
    float phase2 = 0;
    bool isRush = false;

    [Header("보스Ui")]
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
    /// 플레이어가 있는 방향으로 이동하는 함수
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
    /// 범위안에 벽이 닿는다면 반대방향으로 이동
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
    /// 체력별 패턴
    /// </summary>
    private void phasePattern()
    {
        //체력이 최대에서 70퍼까지
        if(phase1 < curHp)
        {
            //플레이어와 닿으면 기본공격
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
            //기본공격
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
            //일정시간마다 플레이어 위치에 spell생성
            reloadTimer += Time.deltaTime;//spell마다 쿨타임을 주기위한 시간값
            if(onSpell == 1)//설정한 초마다 onspell이 1로 바뀔것임
            {
                Attack2 = true;//두번째 공격 실행
                if(reloadTimer >= 1.0f)
                {
                    reloadTimer = 0.0f;
                    posSpell();
                    if(SpellCount >= SpellLimit)//설정한 횟수만큼 spell을 사용하면 패턴이 끝나도록
                    {
                        SpellCount = 0;
                        onSpell = 0;
                        Attack2 = false;
                    }
                }
            }
            if(Attack2 == true)//두번째 공격이 실행되면 움직임을 멈춤
            {
                rigid.velocity = Vector2.zero;
            }
        }
        //25~0%
        else
        {
            //기본공격
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
            //일정시간마다 플레이어 위치에 spell생성
            reloadTimer += Time.deltaTime;//spell마다 쿨타임을 주기위한 시간값
            if (onSpell == 1)//설정한 초마다 onspell이 1로 바뀔것임
            {
                //돌진패턴이 실행중이면 작동안함
                if (isRush == true) return;

                Attack2 = true;//두번째 공격 실행

                if (reloadTimer >= 1.0f)
                {
                    reloadTimer = 0.0f;
                    posSpell();
                    if (SpellCount >= SpellLimit)//설정한 횟수만큼 spell을 사용하면 패턴이 끝나도록
                    {
                        SpellCount = 0;
                        onSpell = 0;
                        Attack2 = false;
                    }
                }
            }
            if (Attack2 == true)//두번째 공격이 실행되면 움직임을 멈춤
            {
                rigid.velocity = Vector2.zero;
            }

            //일정 시간마다 돌진함
            rushTimer += Time.deltaTime;//돌진 쿨타임
            if(rushTimer >= 30f)
            {
                //만약 스펠패턴 작동중이라면 작동안함
                if (Attack2 == true) return;

                isRush = true;
                //움직임을 멈춤
                rigid.velocity = Vector2.zero;
                //현재 바라보고있는 방향 가져옴
                float dir = transform.localScale.x;
                //그 방향으로 벽에 닿을때까지 돌진함
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
                //벽에 닿으면 반대방향으로 전환 후 원래대로
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
    /// 슬라이더 ui에 보스 체력값 전달
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
        GameObject objPlayer = GameObject.Find("Player");//player라는 이름을 가진 오브젝트 찾기
        Player player = objPlayer.GetComponent<Player>();//player component를 가져옴
        Vector3 playerPos = player.transform.position;//player 위치값 저장
        if(playerPos == null) return;
        createSpell(objSpell, playerPos + new Vector3(0, 3, 0), Vector3.zero);//spell 생성
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
