using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject G;
    [SerializeField] GameObject ShopPanel;
    [SerializeField] Button BtnAtk;
    [SerializeField] Button BtnExit;
    [SerializeField] TMP_Text textAtk;
    [SerializeField] TMP_Text textStat;
    Player player;
    bool isEnter;
    bool shoponoff;
    float maxHp;//�÷��̾� �ִ�ü��
    float curHp;//�÷��̾� ����ü��
    int atkPlayer;//�÷��̾� ���ݷ�
    int atkDam;
    int itemCount;

    public static ShopManager instance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            G.SetActive(true);
            isEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            G.SetActive(false);
            isEnter = false;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        G.SetActive(false);
        ShopPanel.SetActive(false);
    }

    private void Start()
    {
        player = Player.instance;
    }

    private void Update()
    {
        if(isEnter == true)
        {
            //���� ��� ������... �������� ���׷��̵� ������... (hp ��������)
            if(Input.GetKeyDown(KeyCode.G) && shoponoff == false)
            {
                ShopPanel.SetActive(true);
                Time.timeScale = 0.0f;
                shoponoff = true;
                //��ư�� ������ �÷��̾��� ���ݷ��� 1�� ����
                BtnAtk.onClick.AddListener(() =>
                {
                    atkDam = player.RePlayerAtkDamage();
                    atkDam += 1;
                    player.upgradeAtk(atkDam);
                    itemCount = player.coinitemCount();
                    itemCount -= 1;
                    player.updownCoin(itemCount);
                });
            }
            else if(Input.GetKeyDown(KeyCode.Escape) && shoponoff == true)
            {
                ShopPanel.SetActive(false);
                Time.timeScale = 1.0f;
                shoponoff = false;
            }
            BtnExit.onClick.AddListener(() =>
            {
                ShopPanel.SetActive(false);
                Time.timeScale = 1.0f;
                shoponoff = false;
            });
        }
        atkPlayer = player.RePlayerAtkDamage();
        maxHp = player.rePlayerMaxHp();
        curHp = player.RePlayerCurHp();
        CurPlayerStat(maxHp, curHp,atkPlayer);
    }
    /// <summary>
    /// �÷��̾��� ������ ����
    /// </summary>
    /// <param name="_MaxHp"></param>
    /// <param name="_CurHp"></param>
    /// <param name="_CurAtk"></param>
    public void CurPlayerStat(float _MaxHp, float _CurHp, int _CurAtk)
    {
        textStat.text = $"�ִ� ü�� = {_MaxHp.ToString()}, ���� ü�� = {_CurHp.ToString()}, \n ���� ���ݷ� = {_CurAtk.ToString()}";
    }
}
