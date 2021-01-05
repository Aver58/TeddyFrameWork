#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    CameraControll.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/4 16:57:20
=====================================================
*/
#endregion

using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    public Camera worldCamera { get; private set; }
    public Camera uiCamera { get; private set; }
    private Vector3 m_position;
    private float m_xOffset = 0f;
    private float m_yOffset = 5f;
    private float m_zOffset = -10f;
    public void Init()
    {
        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        worldCamera = GameObject.Find("SceneCamera").GetComponent<Camera>();
    }

    public void SetWorldCameraPosition(Vector3 position)
    {
        m_position.Set(position.x + m_xOffset, position.y + m_yOffset, position.z + m_zOffset);
        worldCamera.transform.position = m_position;
    }
}