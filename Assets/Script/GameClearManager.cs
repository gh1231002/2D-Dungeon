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
    /// 보스 처치시 실행되는 함수
    /// </summary>
    private void gameClear()
    {
        //보스가 죽었을때만 실행
        if (bossDeath == false) return;
        //팝업창이 나타남, 뒤에 시간은 멈춤
        GameClear.SetActive(true);
    }

    private void btn()
    {
        //메인버튼을누르면 메인화면으로 이동, 플레이어 위치값삭제
        BtnMain.onClick.AddListener(() =>
        {
            GameClear.SetActive(false);
            LoadControl.LoadScene("MainMenu");
            PlayerPrefs.DeleteKey(Tool.sceneNameKey);
        });
    }
}
