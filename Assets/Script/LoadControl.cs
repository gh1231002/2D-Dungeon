using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadControl : MonoBehaviour
{
    static string nextScene;
    Player player;
    [SerializeField]Image loadingBar;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        //SceneManager.LoadScene("Loading");
        SceneManager.LoadSceneAsync("Loading");
    }
    void Start()
    {
        player = Player.instance;
        StartCoroutine(LoadSceneProcess());
    }

    /// <summary>
    /// 로딩씬으로 들어왔을때 다음씬으로 가기위한 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        //GameObject obj = GameObject.Find("PlayerUi");
        //if (nextScene == "Stage1" || nextScene == "Stage2" || nextScene == "BossStage" || nextScene == "MainMenu")
        //{
        //    obj.gameObject.SetActive(false);
        //}

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;
            if(op.progress < 0.9f)
            {
                loadingBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0f, 1f, timer);
                if(loadingBar.fillAmount >= 1f)
                {
                    yield return new WaitForSeconds(1f);

                    op.allowSceneActivation = true;
                    if (nextScene == "Stage1" || nextScene == "Stage2" || nextScene == "BossStage")
                    {
                        //obj.gameObject.SetActive(true);
                        Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
                        rigid.bodyType = RigidbodyType2D.Dynamic;
                    }
                    break;
                }
            }
        }
    }
}
