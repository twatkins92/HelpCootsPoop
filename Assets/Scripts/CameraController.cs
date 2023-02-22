using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public CinemachineVirtualCamera defaultCam;
    public CinemachineVirtualCamera aimCam;

    public float defaultBlendTime = 2f;

    private CinemachineVirtualCamera activeCamera;

    void Awake()
    {
        Instance = this;
        DefaultCamera();
    }

    public void DefaultCamera()
    {
        if (activeCamera != null) activeCamera.gameObject.SetActive(false);
        defaultCam.gameObject.SetActive(true);
        activeCamera = defaultCam;
    }

    public void AimCamera()
    {
        if (activeCamera != null) activeCamera.gameObject.SetActive(false);
        aimCam.gameObject.SetActive(true);
        Camera.main.orthographic = false;
        activeCamera = aimCam;
    }
}
