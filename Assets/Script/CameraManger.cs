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
        //���� virtualcamera�� follow�� ���ٸ� player object�� ã�Ƽ� �־��ش�.
        player = FindAnyObjectByType<Player>();
        Transform trsPlayer = player.GetComponent<Transform>();
        if (virCam.Follow == null)
        {
            virCam.Follow = trsPlayer.transform;
        }
    }
}
