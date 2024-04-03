using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] Image Hp1;
    [SerializeField] Image Hp2;
    [SerializeField] Image Hp3;
    [SerializeField] Button Pause;
    [SerializeField] Button PauseExit;
    [SerializeField] Button MainMenu;
    [SerializeField] GameObject PausePanel;
    [SerializeField] float uiPlayerHp;
    Player player;
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
    }
    void Start()
    {
        player = Player.instance;
        btnDoit();
    }

    private void btnDoit()
    {
        Pause.onClick.AddListener(() =>
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0.0f;
        });
        PauseExit.onClick.AddListener(() =>
        {
            PausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        });
        MainMenu.onClick.AddListener(() =>
        {
            LoadControl.LoadScene("MainMenu");
            Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Static;
        });
    }
    private void Update()
    {
        uiPlayerHp = player.RePlayerCurHp();
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
