using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraManger : MonoBehaviour
{
    Player player;
    CinemachineVirtualCamera virCam;
    
    void Start()
    {
        player = Player.instance;
        virCam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if(player == null) return;
        //만약 virtualcamera의 follow가 없다면 player object를 찾아서 넣어준다.
        player = FindAnyObjectByType<Player>();
        Transform trsPlayer = player.GetComponent<Transform>();
        if (virCam.Follow == null)
        {
            virCam.Follow = trsPlayer.transform;
        }
    }
}
