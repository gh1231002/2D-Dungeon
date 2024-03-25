using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportManager : MonoBehaviour
{
    public enum StageType
    {
        GoTo1,
        GoTo2,
        GoToBoss,
    }

    public StageType Stage;
    Collider2D _collision;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _collision = collision;
        }
    }
    private void Update()
    {
        tpStage(_collision);
    }
    private void tpStage(Collider2D collider)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && Stage == StageType.GoTo2 && collider.gameObject.tag == "Player")
        {
            SceneManager.LoadSceneAsync("Loading");
            
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && Stage == StageType.GoTo1 && collider.gameObject.tag == "Player")
        {
            SceneManager.LoadSceneAsync("Loading");
            
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && Stage == StageType.GoToBoss && collider.gameObject.tag == "Player")
        {
            SceneManager.LoadSceneAsync("Loading");
            
        }
    }
    //private void goStage1()
    //{
    //    SceneManager.LoadSceneAsync("Stage1");
    //}
    //private void goStage2()
    //{
    //    SceneManager.LoadSceneAsync("Stage2");
    //}
}
