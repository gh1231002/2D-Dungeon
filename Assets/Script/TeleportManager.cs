using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportManager : MonoBehaviour
{
    public enum StageType
    {
        None,
        GoTo1,
        GoTo2Start,
        GoTo2Fin,
        GoToBoss,
    }

    public StageType Stage;
    Collider2D _collision;
    Rigidbody2D obj;
    bool isLoaded = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _collision = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            _collision = null;
        }
    }

    private void Update()
    {
        tpStage(_collision);
    }

    private void sceneLoadedAction(Scene scene, LoadSceneMode  loadMode)
    {
        if (scene.name == "Stage1")
        {
            Player player = FindAnyObjectByType<Player>();
            Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
            if (rigid.bodyType == RigidbodyType2D.Static)
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
            }
            isLoaded = false;
            SceneManager.sceneLoaded -= sceneLoadedAction;
        }
        if (scene.name == "Stage2")
        { 
            Player player = FindAnyObjectByType<Player>();
            Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
            if (rigid.bodyType == RigidbodyType2D.Static)
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
            }
            isLoaded = false;
            SceneManager.sceneLoaded -= sceneLoadedAction;
        }
        if(scene.name == "BossStage")
        {
            Player player = FindAnyObjectByType<Player>();
            Rigidbody2D rigid = player.GetComponent <Rigidbody2D>();
            if (rigid.bodyType == RigidbodyType2D.Static)
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
            }
            isLoaded = false;
            SceneManager.sceneLoaded -= sceneLoadedAction;
        }
    }

    private void tpStage(Collider2D collider)
    {
        if (isLoaded == true) return;
        if (_collision == null) return;

        if (Input.GetKeyDown(KeyCode.G) && Stage == StageType.GoTo2Start && collider.gameObject.tag == "Player")
        {
            isLoaded = true;
            SceneManager.sceneLoaded += sceneLoadedAction;

            LoadControl.LoadScene("Stage2");
            obj = collider.gameObject.GetComponent<Rigidbody2D>();
            obj.bodyType = RigidbodyType2D.Static;//플레이어가 중력으로 인해 떨어지지 않게  변경, 위치 고정
            collider.transform.position = new Vector2(-27.89f, -0.14f);//Stage2 start 포탈위치
        }
        if (Input.GetKeyDown(KeyCode.G) && Stage == StageType.GoTo1 && collider.gameObject.tag == "Player")
        {
            isLoaded = true;
            SceneManager.sceneLoaded += sceneLoadedAction;

            LoadControl.LoadScene("Stage1");
            obj = collider.gameObject.GetComponent<Rigidbody2D>();
            obj.bodyType = RigidbodyType2D.Static;//플레이어가 중력으로 인해 떨어지지 않게  변경, 위치 고정
            collider.transform.position = new Vector2(58.53f, 4.49f);//Stage1 포탈위치
        }
        if (Input.GetKeyDown(KeyCode.G) && Stage == StageType.GoToBoss && collider.gameObject.tag == "Player")
        {
            isLoaded = true;
            SceneManager.sceneLoaded += sceneLoadedAction;

            LoadControl.LoadScene("BossStage");
            obj = collider.gameObject.GetComponent <Rigidbody2D>();
            obj.bodyType = RigidbodyType2D.Static;
            collider.transform.position = new Vector2(-29.5f, -2.5f);//boss 포탈위치
        }
        if (Input.GetKeyDown(KeyCode.G) && Stage == StageType.GoTo2Fin && collider.gameObject.tag == "Player")
        {
            isLoaded = true;
            SceneManager.sceneLoaded += sceneLoadedAction;

            LoadControl.LoadScene("Stage2");
            obj = collider.gameObject.GetComponent<Rigidbody2D>();
            obj.bodyType = RigidbodyType2D.Static;
            collider.transform.position = new Vector2(71.54f, -9.5f);//Stage2 fin 포탈위치
        }
    }
}
