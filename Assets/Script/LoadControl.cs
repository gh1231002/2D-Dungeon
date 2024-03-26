using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadControl : MonoBehaviour
{
    static string nextScene;
    [SerializeField]Image loadingBar;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        //SceneManager.LoadScene("Loading");
        SceneManager.LoadSceneAsync("Loading");
    }
    void Start()
    {
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
                    yield return new WaitForSeconds(3f);
                    op.allowSceneActivation = true;
                    break;
                }
            }
        }
    }
}
