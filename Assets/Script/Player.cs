using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField,Tooltip("�÷��̾� �̵��ӵ�")] private float playerSpeed = 0.0f; 
    [SerializeField,Tooltip("�����ϴ� ��")]float jumpForce = 0.0f;
    [SerializeField,Tooltip("�߷°�")]float gravity = 9.81f;
    [SerializeField]bool isGround = false; //���� ��Ҵ���
    [SerializeField]float verticalVelocity; //�÷��̾� ������
    bool isJump = false; //�����ϴ�������
    Camera cam;
    Animator anim;
    Rigidbody2D rigid;
    BoxCollider2D boxCollider;
    Vector3 moveDir; //�÷��̾� �̵���

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
        //����
    }
    /// <summary>
    /// �÷��̾� �̵�
    /// </summary>
    private void move()
    {
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
}
