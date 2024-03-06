using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField,Tooltip("플레이어 이동속도")] private float playerSpeed = 0.0f; 
    [SerializeField,Tooltip("점프하는 힘")]float jumpForce = 0.0f;
    [SerializeField,Tooltip("중력값")]float gravity = 9.81f;
    [SerializeField]bool isGround = false; //땅에 닿았는지
    [SerializeField]float verticalVelocity; //플레이어 수직값
    bool isJump = false; //점프하는중인지
    Camera cam;
    Animator anim;
    Rigidbody2D rigid;
    BoxCollider2D boxCollider;
    Vector3 moveDir; //플레이어 이동값

    private void Awake()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        transform.position = new Vector2(-18f, -2);
    }

    void Start()
    {
        
    }

    void Update()
    {
        move();
        doAni();
        turnDir();
        checkGround();
        dojump();
        checkGravity();
        //공격
    }
    /// <summary>
    /// 플레이어 이동
    /// </summary>
    private void move()
    {
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
}
