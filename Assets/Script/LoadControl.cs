using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadControl : MonoBehaviour
{
    static string nextScene;
    GameObject obj = null;
    GameObject objPlayer;
    [SerializeField]Image loadingBar;
    Player player;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        //SceneManager.LoadScene("Loading");
        SceneManager.LoadSceneAsync("Loading");
    }
    private void Awake()
    {
        player = Player.instance;
        obj = GameObject.Find("PlayerUi");
        objPlayer = GameObject.Find("Player");
    }
    void Start()
    {
        if(obj == null)
        {
            StartCoroutine(basicLoadSceneProcess());
        }
        else if(obj != null)
        {
            StartCoroutine(LoadSceneProcess());
        }
    }

    IEnumerator basicLoadSceneProcess()
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

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        obj.gameObject.SetActive(false);
        objPlayer.gameObject.SetActive(false);

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

                    obj.gameObject.SetActive(true);
                    objPlayer.gameObject.SetActive(true);

                    if (nextScene == "Loading" || nextScene == "MainMenu")
                    {
                        Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
                        rigid.bodyType = RigidbodyType2D.Static;
                    }
                    else
                    {
                        Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
                        rigid.bodyType = RigidbodyType2D.Dynamic;
                    }
                    break;
                }
            }
        }
    }
}
