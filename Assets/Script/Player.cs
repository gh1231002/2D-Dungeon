using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("플레이어데이터")]
    [SerializeField,Tooltip("플레이어 이동속도")] float playerSpeed = 0.0f; 
    [SerializeField,Tooltip("점프하는 힘")] float jumpForce = 0.0f;
    [SerializeField,Tooltip("중력값")] float gravity = 9.81f;
    [SerializeField] bool isGround = false; //땅에 닿았는지
    [SerializeField] float verticalVelocity; //플레이어 수직값
    [SerializeField] float playerMaxHp = 0.0f;
    [SerializeField] float playerCurHp = 0.0f;
    bool isJump = false; //점프하는중인지
    bool isAttack = false;//공격중인지
    bool isDeath = false;
    [SerializeField] bool isEnter = false;
    float enterTimer = 0.0f;
    [SerializeField,Tooltip("닿았을때 튕겨나가는 힘")] float enterForce = 0.0f;
    bool isHurt = false;
    [SerializeField]bool isLadder = false;//사다리
    [SerializeField] BoxCollider2D AttackBox;

    Camera cam;
    Animator anim;
    Rigidbody2D rigid;
    BoxCollider2D boxCollider;
    Vector3 moveDir; //플레이어 이동값

    [Header("슬라이드")]
    bool isSlide = false;//대쉬하는중인지
    [SerializeField,Tooltip("슬라이딩 쿨타임")]float slideTimer = 0.0f;
    float slideTime = 0.0f;

    public static Player instance;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Trap")//함정에 부딪히면
        {
            isEnter = true;
            isHurt = true;
            Hit(1);
            Vector2 target = collision.transform.position;
            onDamaged(target);
        }
        if(collision.gameObject.tag == "Enemy")//몬스터와 부딪힌다면
        {
            isEnter = true;
            isHurt = true;
            Vector2 target = collision.transform.position;
            onDamaged(target);
        }
        if( collision.gameObject.tag == "Spell")//spell공격에 맞으면
        {
            isEnter = true;
            isHurt = true;
            Hit(1);
            Vector2 target = collision.transform.position;
            onDamaged(target);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")//사다리에 닿으면 layer가 변경되고 rigid바디타입도 변경
        {
            gameObject.layer = LayerMask.NameToLayer("Ladder");
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        if(collision.gameObject.tag == "EBox")//적의 공격을 맞으면
        {
            isEnter = true;
            isHurt = true;
            Hit(1);
            Vector2 target = collision.transform.position;
            onDamaged(target);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")//사다리에서 벗어나면 원래대로 변경
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            rigid.bodyType = RigidbodyType2D.Dynamic;
            isLadder = false;
        }
    }

    /// <summary>
    /// 플레이어가 데미지를 입었을때 튕겨나고 일정시간 무적
    /// </summary>
    private void onDamaged(Vector2 _target)
    {
        //닿은 위치에서 반대방향으로 튕기게
        Vector2 curPos = transform.position;
        Vector2 dir = curPos - _target;
        dir.Normalize();
        rigid.AddForce(dir * enterForce, ForceMode2D.Impulse);

        gameObject.layer = LayerMask.NameToLayer("PlayerDamage");

        Invoke("offDamage", 1f);
    }
    /// <summary>
    /// 무적이었던 상태를 다시 원상태로 복구
    /// </summary>
    private void offDamage()
    {
        //layer를 다시 원상태로 복귀
        gameObject.layer = LayerMask.NameToLayer("Player");
        isHurt = false;
    }

    public void Hit(float _damage)
    {
        playerCurHp -= _damage;
        if (playerCurHp <= 0f)
        {
            isDeath = true;
            anim.Play("Death");
        }
    }


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        AttackBox.enabled = false;
        //transform.position = new Vector2(-18f, -2); //Stage1 시작 위치
        playerCurHp = playerMaxHp;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (isDeath == true) return;
        move();
        doLadder();
        climingLadder();
        doAni();
        turnDir();
        checkGround();
        dojump();
        checkGravity();
        attack();
        slide();
        enterCooltime();
    }
    /// <summary>
    /// 플레이어 이동
    /// </summary>
    private void move()
    {
        if (isSlide == true || isEnter == true || isHurt == true)
            return;

        moveDir.x = Input.GetAxisRaw("Horizontal") * playerSpeed;
        moveDir.y = rigid.velocity.y;
        rigid.velocity = moveDir;
    }
    /// <summary>
    /// 플레이어 layer가 Ladder일때 동작하는 함수
    /// </summary>
    private void doLadder()
    {
        if(gameObject.layer == 11 && Input.GetKeyDown(KeyCode.UpArrow))
        {
            isLadder = true;
            //rigid.velocity += Vector2.up;
        }
        if (gameObject.layer == 11 && Input.GetKeyDown(KeyCode.DownArrow))
        {
            isLadder = true;
            //rigid.velocity += Vector2.down;
        }
    }
    /// <summary>
    /// isLadder가 true일때만 작동하는 사다리 애니메이션 함수
    /// </summary>
    private void climingLadder()
    {
        if (isLadder == false) return;//사다리에 닿지 않았을땐 동작하지않음

        float velVertical = Input.GetAxisRaw("Vertical");//변수에 vertical 누를때의 값을 저장
        anim.SetFloat("LadderM", velVertical);//누를때만 애니메이션이 작동, 파라미터에 값을 전달
        rigid.velocity = new Vector2(moveDir.x, velVertical);//rigid 값에 위아래로 움직이는 값을 넣음
    }
    /// <summary>
    /// 플레이어 애니메이션 연결
    /// </summary>
    private void doAni()
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
        anim.SetBool("isGround", isGround);
        anim.SetBool("Attack", isAttack);
        anim.SetBool("Slide", isSlide);
        anim.SetBool("Hurt", isHurt);
        anim.SetBool("Ladder", isLadder);
    }
    /// <summary>
    /// 입력된 방향키에 따른 플레이어가 바라보는 방향
    /// </summary>
    private void turnDir()
    {
        if(moveDir.x > 0 && transform.localScale.x != 1)//이동값이 양수이고 스케일이 1이 아닐때
        {
            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
        else if(moveDir.x < 0 && transform.localScale.x != -1)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
    }
    /// <summary>
    /// 플레이어가 땅에 닿았는지 체크
    /// </summary>
    private void checkGround()
    {
        isGround = false;
        if (verticalVelocity > 0f) return;
        isGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));//ground layer와 닿으면 true, 아니라면 false
    }
    /// <summary>
    /// 점프키를 누름
    /// </summary>
    private void dojump()
    {
        if (isSlide == true || isEnter == true || isHurt == true || isLadder == true) return;
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)//키를 눌렀을때 isground가 true라면
        {
            isJump = true;
        }
    }
    /// <summary>
    /// 중력 관리
    /// </summary>
    private void checkGravity()
    {
        if (isSlide == true || isEnter == true || isHurt == true || isLadder == true || rigid.bodyType == RigidbodyType2D.Static) return;
        //isGround 가 false 일때 중력이 시간에 따라 증가함
        if(isGround == false)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            if (verticalVelocity < -10.0f) //증가한 값이 일정값 이상일때 고정
            {
                verticalVelocity = -10f;
            }
        }
        else //true라면 0
        {
            verticalVelocity = 0f;
        }

        if(isJump == true)//isjump가 true일때 verticalvelocity 값을 jumpeforce로
        {
            isJump = false;
            verticalVelocity = jumpForce;
        }
        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }
    /// <summary>
    /// 특정키 입력시 캐릭터가 공격함
    /// </summary>
    private void attack()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            isAttack = true;
            //공격 애니메이션에 맞춰서 공격판정박스를 키고 끔
            Invoke("onAttackBox", 0.5f);
            Invoke("offAttackBox", 1f);
        }
        else
        {
            isAttack = false;
        }
    }
    /// <summary>
    /// 공격판정박스 킴
    /// </summary>
    private void onAttackBox()
    {
        AttackBox.enabled = true;
    }
    /// <summary>
    /// 공격판정박스 끔
    /// </summary>
    private void offAttackBox()
    {
        AttackBox.enabled = false;
    }
    /// <summary>
    /// 캐릭터가 슬라이딩을 함
    /// </summary>
    private void slide()
    {
        if (isGround == false || isEnter == true || isHurt == true) return;
        if(Input.GetKeyDown(KeyCode.LeftShift) && isSlide == false)
        {
            isSlide = true;
            verticalVelocity = 0f;
            //캐릭터가 보고있는 방향에 따라 슬라이딩 값 다르게
            if(transform.localScale.x == 1)
            {
                rigid.velocity = new Vector2(10f, 0f);
            }
            else if(transform.localScale.x == -1)
            {
                rigid.velocity = new Vector2(-10f, 0f);
            }
            //캐릭터가 slide layer 획득하고 물리무시
            gameObject.layer = LayerMask.NameToLayer("Slide");
        }
        else if(isSlide == true)//슬라이딩이 true
        {
            slideTime += Time.deltaTime;//내부타이머 증가
            if(slideTime >= slideTimer)//설정한 쿨타임보다 크거나 같으면
            {
                slideTime = 0f;
                isSlide = false;
                //플레이어 layer으로 변경
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }
    /// <summary>
    /// 오브젝트에 닿아서 튕겨날때 몇초동안 튕겨날지 관리
    /// </summary>
    private void enterCooltime()
    {
        if (isEnter == true)
        {
            enterTimer += Time.deltaTime;
            if (enterTimer >= 0.5f)
            {
                enterTimer = 0.0f;
                isEnter = false;
            }
        }
    }
    /// <summary>
    /// 플레이어 현재 Hp값을 return하는 함수
    /// </summary>
    /// <returns></returns>
    public float RePlayerCurHp()
    {
        return playerCurHp;
    }
}
