using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class UiManager : MonoBehaviour
{
    [SerializeField] Image Hp1;
    [SerializeField] Image Hp2;
    [SerializeField] Image Hp3;
    [SerializeField] Button Pause;
    [SerializeField] Button PauseExit;
    [SerializeField] Button MainMenu;
    [SerializeField] Button DeMainMenu;
    [SerializeField] Button Guide;
    [SerializeField] Button GuideExit;
    [SerializeField] Button ModeOnOff;
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject GuidePanel;
    [SerializeField] GameObject DebugMode;
    [SerializeField] GameObject DeathPanel;
    [SerializeField] float uiPlayerHp;
    [SerializeField] TMP_Text counttext;

    Player player;
    bool isOpen;
    bool guideOpen;
    bool isdebugMode = false;
    bool playerDeath;
    Vector2 playerPos;
    string curScene;

    public static UiManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        PausePanel.SetActive(false);
        GuidePanel.SetActive(false);
        DeathPanel.SetActive(false);
        DebugMode.SetActive(false);
    }
    void Start()
    {
        player = Player.instance;
        isOpen = false;
        guideOpen = false;
        btnDoit();
    }
    /// <summary>
    /// ���콺�� ��ư ������������ �۵�
    /// </summary>
    private void btnDoit()
    {
        Pause.onClick.AddListener(() =>
        {
            PausePanel.SetActive(true);
            isOpen = true;
            Time.timeScale = 0.0f;
        });
        PauseExit.onClick.AddListener(() =>
        {
            PausePanel.SetActive(false);
            isOpen = false;
            Time.timeScale = 1.0f;
        });
        MainMenu.onClick.AddListener(() =>
        {
            LoadControl.LoadScene("MainMenu");
            PausePanel.SetActive(false);
            Time.timeScale = 1.0f;
            Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Static;

            PlayerPrefs.SetString(Tool.sceneNameKey, SceneManager.GetActiveScene().name);
            //PlayerPrefs.DeleteKey(Tool.sceneNameKey); //Ű�� ������ ���� �����Ҷ� 

            //curScene = SceneManager.GetActiveScene().name;//� ������ ��ư�� �������� ������ ����
        });
        Guide.onClick.AddListener(() =>
        {
            GuidePanel.SetActive(true);
            guideOpen = true;
            Time.timeScale = 0f;
        });
        GuideExit.onClick.AddListener(() =>
        {
            GuidePanel.SetActive(false);
            guideOpen = false;
            Time.timeScale = 1.0f;
        });
        ModeOnOff.onClick.AddListener(() =>
        {
            if(isdebugMode == false)
            {
                isdebugMode = true;
                DebugMode.SetActive(true);
            }
            else if(isdebugMode == true)
            {
                isdebugMode = false;
                DebugMode.SetActive(false);
            }
        });
        DeMainMenu.onClick.AddListener(() =>
        {
            LoadControl.LoadScene("MainMenu");
            DeathPanel.SetActive(false);
            PlayerPrefs.SetString(Tool.sceneNameKey, SceneManager.GetActiveScene().name);
            player.gameOver();
        });
    }
    /// <summary>
    /// ����׸�尡 onoff���� ����
    /// </summary>
    /// <returns></returns>
    public bool DebugModeOnOff()
    {
        return isdebugMode;
    }
    /// <summary>
    /// �ΰ��ӿ��� ����ȭ������ �Ѿ�� ���� ���̾����� ����
    /// </summary>
    /// <returns></returns>
    public string ReCurScene()
    {
        return curScene;
    }
    /// <summary>
    /// Ư��Ű �Է½� �г��� ���� Ŵ
    /// </summary>
    private void onoffPanel()
    {
        if (guideOpen == true) return;
        if (Input.GetKeyDown(KeyCode.Escape) && isOpen == false)
        {
            PausePanel.SetActive(true);
            isOpen = true;
            Time.timeScale = 0.0f;
        }
       else if(Input.GetKeyDown(KeyCode.Escape) && isOpen == true)
        {
            PausePanel.SetActive(false);
            isOpen = false;
            Time.timeScale = 1.0f;
        }
    }
    /// <summary>
    /// Ư��Ű �Է½� ���̵� ũ�� ��
    /// </summary>
    private void guideOnOff()
    {
        if(isOpen == true) return;
        if (Input.GetKeyDown(KeyCode.F1) && guideOpen == false)
        {
            GuidePanel.SetActive(true);
            guideOpen = true;
            Time.timeScale = 0.0f;
        }
        else if( Input.GetKeyDown(KeyCode.Escape) && guideOpen == true)
        {
            GuidePanel.SetActive(false);
            guideOpen = false;
            Time.timeScale = 1.0f;
        }
    }

    public void SetItemGet(int _Num)
    {
        string value = $"{_Num}";
        counttext.text = value;
    }
    private void Update()
    {
        //btnDoit();
        onoffPanel();
        guideOnOff();
        uiPlayerHp = player.RePlayerCurHp();
        playerDeath = player.ReisDead();
        if(playerDeath == true)//�÷��̾ ����ϸ� ����г� on
        {
            DeathPanel.SetActive(true);
        }
        switch (uiPlayerHp)
        {
            case 0:
                {
                    Hp1.enabled = false;
                    Hp2.enabled = false;
                    Hp3.enabled = false;
                    break;
                }
            case 1:
                {
                    Hp1.enabled = true;
                    Hp2.enabled = false;
                    Hp3.enabled = false;
                    break;
                }
            case 2:
                {
                    Hp1.enabled = true;
                    Hp2.enabled = true;
                    Hp3.enabled = false;
                    break;
                }
            case 3:
                {
                    Hp1.enabled = true;
                    Hp2.enabled = true;
                    Hp3.enabled = true;
                    break;
                }
        }
    }
}
