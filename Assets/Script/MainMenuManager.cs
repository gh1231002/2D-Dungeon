using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]Button start;
    [SerializeField]Button quit;
    private void Start()
    {
        start.onClick.AddListener(() =>
        {
            LoadControl.LoadScene("stage1");
        });
        quit.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
           Application.Quit();
#endif
        });
    }
}
