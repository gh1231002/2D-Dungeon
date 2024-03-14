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

    Camera cam;
    Animator anim;
    Rigidbody2D rigid;
    BoxCollider2D boxCollider;
    Vector3 moveDir; //플레이어 이동값

    [Header("슬라이드")]
    bool isSlide = false;//대쉬하는중인지
    [SerializeField,Tooltip("슬라이딩 쿨타임")]float slideTimer = 0.0f;
    float slideTime = 0.0f;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            isEnter = true;
            isHurt = true;
            Vector2 target = collision.transform.position;
            onDamaged(target);
        }
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.layer = LayerMask.NameToLayer("Ladder");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.gameObject.layer = LayerMask.NameToLayer("Player");
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
        if (playerCurHp <= 0)
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
        //transform.position = new Vector2(-18f, -2); //시작 위치
        playerCurHp = playerMaxHp;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (isDeath == true) return;
        move();
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
    /// 플레이어 애니메이션 연결
    /// </summary>
    private void doAni()
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
        anim.SetBool("isGround", isGround);
        anim.SetBool("Attack", isAttack);
        anim.SetBool("Slide", isSlide);
        anim.SetBool("Hurt", isHurt);
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
        if (isSlide == true || isEnter == true || isHurt == true) return;
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
        if (isSlide == true || isEnter == true || isHurt == true) return;
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
        }
        else
        {
            isAttack = false;
        }
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
            if (enterTimer >= 1.0f)
            {
                enterTimer = 0.0f;
                isEnter = false;
            }
        }
    }
}
