using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�÷��̾����")]
    [SerializeField,Tooltip("�÷��̾� �̵��ӵ�")] float playerSpeed = 0.0f; 
    [SerializeField,Tooltip("�����ϴ� ��")] float jumpForce = 0.0f;
    [SerializeField,Tooltip("�߷°�")] float gravity = 9.81f;
    [SerializeField] bool isGround = false; //���� ��Ҵ���
    [SerializeField] float verticalVelocity; //�÷��̾� ������
    [SerializeField] float playerMaxHp = 0.0f;
    [SerializeField] float playerCurHp = 0.0f;
    bool isJump = false; //�����ϴ�������
    bool isAttack = false;//����������
    [SerializeField]bool isDeath;

    Camera cam;
    Animator anim;
    Rigidbody2D rigid;
    BoxCollider2D boxCollider;
    Vector3 moveDir; //�÷��̾� �̵���

    [Header("�����̵�")]
    bool isSlide = false;//�뽬�ϴ�������
    [SerializeField,Tooltip("�����̵� ��Ÿ��")]float slideTimer = 0.0f;
    float slideTime = 0.0f;


    public void Hit(float _damage)
    {
        playerCurHp -= _damage;
        if (playerCurHp == 0)
        {
            isDeath = true;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        transform.position = new Vector2(-18f, -2);
        playerCurHp = playerMaxHp;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        move();
        doAni();
        turnDir();
        checkGround();
        dojump();
        checkGravity();
        attack();
        slide();
    }
    /// <summary>
    /// �÷��̾� �̵�
    /// </summary>
    private void move()
    {
        if (isSlide == true) return;
        moveDir.x = Input.GetAxisRaw("Horizontal") * playerSpeed;
        moveDir.y = rigid.velocity.y;
        rigid.velocity = moveDir;
    }
    /// <summary>
    /// �÷��̾� �ִϸ��̼� ����
    /// </summary>
    private void doAni()
    {
        anim.SetInteger("Horizontal", (int)moveDir.x);
        anim.SetBool("isGround", isGround);
        anim.SetBool("Attack", isAttack);
        anim.SetBool("Slide", isSlide);
        anim.SetBool("Death", isDeath);
    }
    /// <summary>
    /// �Էµ� ����Ű�� ���� �÷��̾ �ٶ󺸴� ����
    /// </summary>
    private void turnDir()
    {
        if(moveDir.x > 0 && transform.localScale.x != 1)//�̵����� ����̰� �������� 1�� �ƴҶ�
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
    /// �÷��̾ ���� ��Ҵ��� üũ
    /// </summary>
    private void checkGround()
    {
        isGround = false;
        if (verticalVelocity > 0f) return;
        isGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));//ground layer�� ������ true, �ƴ϶�� false
    }
    /// <summary>
    /// ����Ű�� ����
    /// </summary>
    private void dojump()
    {
        if (isSlide == true) return;
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)//Ű�� �������� isground�� true���
        {
            isJump = true;
        }
    }
    /// <summary>
    /// �߷� ����
    /// </summary>
    private void checkGravity()
    {
        if (isSlide == true) return;
        //isGround �� false �϶� �߷��� �ð��� ���� ������
        if(isGround == false)
        {
            verticalVelocity -= gravity * Time.deltaTime;
            if (verticalVelocity < -10.0f) //������ ���� ������ �̻��϶� ����
            {
                verticalVelocity = -10f;
            }
        }
        else //true��� 0
        {
            verticalVelocity = 0f;
        }

        if(isJump == true)//isjump�� true�϶� verticalvelocity ���� jumpeforce��
        {
            isJump = false;
            verticalVelocity = jumpForce;
        }
        rigid.velocity = new Vector2(rigid.velocity.x, verticalVelocity);
    }
    /// <summary>
    /// Ư��Ű �Է½� ĳ���Ͱ� ������
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
    /// ĳ���Ͱ� �����̵��� ��
    /// </summary>
    private void slide()
    {
        if (isGround == false) return;
        if(Input.GetKeyDown(KeyCode.LeftShift) && isSlide == false)
        {
            isSlide = true;
            verticalVelocity = 0f;
            //ĳ���Ͱ� �����ִ� ���⿡ ���� �����̵� �� �ٸ���
            if(transform.localScale.x == 1)
            {
                rigid.velocity = new Vector2(10f, 0f);
            }
            else if(transform.localScale.x == -1)
            {
                rigid.velocity = new Vector2(-10f, 0f);
            }
            //ĳ���Ͱ� slide layer ȹ���ϰ� ��������
            gameObject.layer = LayerMask.NameToLayer("Slide");
        }
        else if(isSlide == true)//�����̵��� true
        {
            slideTime += Time.deltaTime;//����Ÿ�̸� ����
            if(slideTime >= slideTimer)//������ ��Ÿ�Ӻ��� ũ�ų� ������
            {
                slideTime = 0f;
                isSlide = false;
                //�÷��̾� layer���� ����
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }
}
