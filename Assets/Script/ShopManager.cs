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
    float maxHp;//플레이어 최대체력
    float curHp;//플레이어 현재체력
    int atkPlayer;//플레이어 공격력
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
            //상점 기능 구상중... 코인으로 업그레이드 뭐하지... (hp 업글제외)
            if(Input.GetKeyDown(KeyCode.G) && shoponoff == false)
            {
                ShopPanel.SetActive(true);
                Time.timeScale = 0.0f;
                shoponoff = true;
                //버튼을 누르면 플레이어의 공격력이 1씩 증가
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
    /// 플레이어의 스탯을 전달
    /// </summary>
    /// <param name="_MaxHp"></param>
    /// <param name="_CurHp"></param>
    /// <param name="_CurAtk"></param>
    public void CurPlayerStat(float _MaxHp, float _CurHp, int _CurAtk)
    {
        textStat.text = $"최대 체력 = {_MaxHp.ToString()}, 현재 체력 = {_CurHp.ToString()}, \n 현재 공격력 = {_CurAtk.ToString()}";
    }
}
