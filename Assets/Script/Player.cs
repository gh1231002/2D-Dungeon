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
    bool isDeath = false;
    [SerializeField] bool isEnter = false;
    float enterTimer = 0.0f;
    [SerializeField,Tooltip("������� ƨ�ܳ����� ��")] float enterForce = 0.0f;
    bool isHurt = false;
    [SerializeField]bool isLadder = false;//��ٸ�
    [SerializeField] BoxCollider2D AttackBox;

    Camera cam;
    Animator anim;
    Rigidbody2D rigid;
    BoxCollider2D boxCollider;
    Vector3 moveDir; //�÷��̾� �̵���

    [Header("�����̵�")]
    bool isSlide = false;//�뽬�ϴ�������
    [SerializeField,Tooltip("�����̵� ��Ÿ��")]float slideTimer = 0.0f;
    float slideTime = 0.0f;

    public static Player instance;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Trap")//������ �ε�����
        {
            isEnter = true;
            isHurt = true;
            Hit(1);
            Vector2 target = collision.transform.position;
            onDamaged(target);
        }
        if(collision.gameObject.tag == "Enemy")//���Ϳ� �ε����ٸ�
        {
            isEnter = true;
            isHurt = true;
            Vector2 target = collision.transform.position;
            onDamaged(target);
        }
        if( collision.gameObject.tag == "Spell")//spell���ݿ� ������
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
        if (collision.gameObject.tag == "Ladder")//��ٸ��� ������ layer�� ����ǰ� rigid�ٵ�Ÿ�Ե� ����
        {
            gameObject.layer = LayerMask.NameToLayer("Ladder");
            rigid.bodyType = RigidbodyType2D.Kinematic;
        }
        if(collision.gameObject.tag == "EBox")//���� ������ ������
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
        if (collision.gameObject.tag == "Ladder")//��ٸ����� ����� ������� ����
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            rigid.bodyType = RigidbodyType2D.Dynamic;
            isLadder = false;
        }
    }

    /// <summary>
    /// �÷��̾ �������� �Ծ����� ƨ�ܳ��� �����ð� ����
    /// </summary>
    private void onDamaged(Vector2 _target)
    {
        //���� ��ġ���� �ݴ�������� ƨ���
        Vector2 curPos = transform.position;
        Vector2 dir = curPos - _target;
        dir.Normalize();
        rigid.AddForce(dir * enterForce, ForceMode2D.Impulse);

        gameObject.layer = LayerMask.NameToLayer("PlayerDamage");

        Invoke("offDamage", 1f);
    }
    /// <summary>
    /// �����̾��� ���¸� �ٽ� �����·� ����
    /// </summary>
    private void offDamage()
    {
        //layer�� �ٽ� �����·� ����
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
        //transform.position = new Vector2(-18f, -2); //Stage1 ���� ��ġ
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
    /// �÷��̾� �̵�
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
    /// �÷��̾� layer�� Ladder�϶� �����ϴ� �Լ�
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
    /// isLadder�� true�϶��� �۵��ϴ� ��ٸ� �ִϸ��̼� �Լ�
    /// </summary>
    private void climingLadder()
    {
        if (isLadder == false) return;//��ٸ��� ���� �ʾ����� ������������

        float velVertical = Input.GetAxisRaw("Vertical");//������ vertical �������� ���� ����
        anim.SetFloat("LadderM", velVertical);//�������� �ִϸ��̼��� �۵�, �Ķ���Ϳ� ���� ����
        rigid.velocity = new Vector2(moveDir.x, velVertical);//rigid ���� ���Ʒ��� �����̴� ���� ����
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
        anim.SetBool("Hurt", isHurt);
        anim.SetBool("Ladder", isLadder);
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
        if (isSlide == true || isEnter == true || isHurt == true || isLadder == true) return;
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
        if (isSlide == true || isEnter == true || isHurt == true || isLadder == true || rigid.bodyType == RigidbodyType2D.Static) return;
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
            //���� �ִϸ��̼ǿ� ���缭 ���������ڽ��� Ű�� ��
            Invoke("onAttackBox", 0.5f);
            Invoke("offAttackBox", 1f);
        }
        else
        {
            isAttack = false;
        }
    }
    /// <summary>
    /// ���������ڽ� Ŵ
    /// </summary>
    private void onAttackBox()
    {
        AttackBox.enabled = true;
    }
    /// <summary>
    /// ���������ڽ� ��
    /// </summary>
    private void offAttackBox()
    {
        AttackBox.enabled = false;
    }
    /// <summary>
    /// ĳ���Ͱ� �����̵��� ��
    /// </summary>
    private void slide()
    {
        if (isGround == false || isEnter == true || isHurt == true) return;
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
    /// <summary>
    /// ������Ʈ�� ��Ƽ� ƨ�ܳ��� ���ʵ��� ƨ�ܳ��� ����
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
    /// �÷��̾� ���� Hp���� return�ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    public float RePlayerCurHp()
    {
        return playerCurHp;
    }
}
