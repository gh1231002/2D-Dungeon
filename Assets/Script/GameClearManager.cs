using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    
    [SerializeField] GameObject GameClear;
    [SerializeField] Button BtnMain;

    bool bossDeath;
    BossManager bossManager;

    private void Awake()
    {
        bossManager = FindAnyObjectByType<BossManager>();
        GameClear.SetActive(false);
        btn();
    }

    void Update()
    {
        gameClear();
        bossDeath = bossManager.reBossDeath();
    }

    /// <summary>
    /// ���� óġ�� ����Ǵ� �Լ�
    /// </summary>
    private void gameClear()
    {
        //������ �׾������� ����
        if (bossDeath == false) return;
        //�˾�â�� ��Ÿ��, �ڿ� �ð��� ����
        GameClear.SetActive(true);
    }

    private void btn()
    {
        //���ι�ư�������� ����ȭ������ �̵�, �÷��̾� ��ġ������
        BtnMain.onClick.AddListener(() =>
        {
            GameClear.SetActive(false);
            LoadControl.LoadScene("MainMenu");
            PlayerPrefs.DeleteKey(Tool.sceneNameKey);
        });
    }
}
