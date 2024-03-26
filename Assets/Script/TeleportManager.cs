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
    Rigidbody2D obj;
    float timer = 0.0f;

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

    private void sceneLoadedAction(Scene scene, LoadSceneMode  loadMode)
    {
        if (scene.name == "Stage2")
        { 
            Player player = FindAnyObjectByType<Player>();
            Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Dynamic;

            SceneManager.sceneLoaded -= sceneLoadedAction;
        }
    }

    private void tpStage(Collider2D collider)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && Stage == StageType.GoTo2 && collider.gameObject.tag == "Player")
        {
            SceneManager.sceneLoaded += sceneLoadedAction;

            LoadControl.LoadScene("Stage2");
            obj = collider.gameObject.GetComponent<Rigidbody2D>();
            obj.isKinematic = true;//�÷��̾ �߷����� ���� �������� �ʰ� kinematic���� ����, ��ġ ����
            collider.transform.position = new Vector2(-27.89f, -0.14f);//Stage2 ��Ż��ġ
            //�̵� �Ϸ��� kinematic ����

        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && Stage == StageType.GoTo1 && collider.gameObject.tag == "Player")
        {
            LoadControl.LoadScene("Stage1");
            collider.transform.position = new Vector2(58.53f, 4.49f);//Stage1 ��Ż��ġ
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && Stage == StageType.GoToBoss && collider.gameObject.tag == "Player")
        {
            
        }
    }
}
