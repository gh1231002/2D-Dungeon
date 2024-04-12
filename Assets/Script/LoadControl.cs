using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadControl : MonoBehaviour
{
    static string nextScene;
    GameObject obj = null;
    [SerializeField]Image loadingBar;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        //SceneManager.LoadScene("Loading");
        SceneManager.LoadSceneAsync("Loading");
    }

    private void Awake()
    {
        obj = GameObject.Find("PlayerUi");
    }

    void Start()
    {
        if (obj != null)
        {
            StartCoroutine(LoadSceneProcess());
        }
        else if (obj == null)
        {
            StartCoroutine(LoadSceneProcess2());
        }
    }
    
    /// <summary>
    /// Ui를 찾고 null이 아니라면 진행되는 로딩함수
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        
        if (nextScene == "Stage1" || nextScene == "Stage2" || nextScene == "BossStage" || nextScene == "MainMenu")
        {
            obj.gameObject.SetActive(false);
        }

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
                        obj.gameObject.SetActive(true);
                        Player player = FindAnyObjectByType<Player>();
                        Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
                        rigid.bodyType = RigidbodyType2D.Dynamic;
                    }
                    break;
                }
            }
        }
    }
    /// <summary>
    /// Ui가 null일때 진행되는 로딩함수
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneProcess2()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;
            if (op.progress < 0.9f)
            {
                loadingBar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0f, 1f, timer);
                if (loadingBar.fillAmount >= 1f)
                {
                    yield return new WaitForSeconds(1f);

                    op.allowSceneActivation = true;

                    break;
                }
            }
        }
    }
}
