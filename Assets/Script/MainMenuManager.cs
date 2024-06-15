using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]Button start;
    [SerializeField]Button conti;
    [SerializeField]Button quit;
    [SerializeField]GameObject contiPanel;
    string SceneName = "";
    UiManager uiManager;
    Player player;
    bool isOpen;
    private void Awake()
    {
        contiPanel.SetActive(false);
    }
    private void Start()
    {
        player = Player.instance;
        uiManager = UiManager.instance;
        //게임을 처음시작할때
        start.onClick.AddListener(() =>
        {
            LoadControl.LoadScene("Stage1");
            player.transform.position = new Vector2(-18f, -2f);//처음동작한 이후 또 클릭하면 새게임버튼처럼 스테이지1 시작위치로 이동
        });

        //플레이했던적이 있을때
        SceneName = PlayerPrefs.GetString(Tool.sceneNameKey);
        conti.onClick.AddListener(() =>
        {
            if (SceneName == "")
            {
                isOpen = true;
                contiPanel.SetActive(true);
            }
            else if(SceneName != "")
            {
                LoadControl.LoadScene(SceneName);
            }
        });
        quit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            PlayerPrefs.DeleteKey(Tool.sceneNameKey);
            UnityEditor.EditorApplication.isPlaying = false;
#else
           Application.Quit();
#endif
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isOpen == true)
        {
            contiPanel.SetActive(false);
        }
        //SceneName = uiManager.ReCurScene();
    }
}
