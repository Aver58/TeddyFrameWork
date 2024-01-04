using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCommandLineArgs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string[] commandLineArgs = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < commandLineArgs.Length; i++) {
            Debug.LogError("命令行参数：" + commandLineArgs[i]);
            if (commandLineArgs[i] == "-is_release") {
                var value = Convert.ToBoolean(commandLineArgs[i + 1]);
                Debug.LogError("命令行参数：" + value);
                if (value) {
                    TestCommandLineArgsCall.TestCall();
                }
            }
        }

        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    private float updateCount = 0;
    private float fixedUpdateCount = 0;
    private float updateUpdateCountPerSecond;
    private float updateFixedUpdateCountPerSecond;

    void Awake()
    {
        // Uncommenting this will cause framerate to drop to 10 frames per second.
        // This will mean that FixedUpdate is called more often than Update.
        Application.targetFrameRate = 10;
        StartCoroutine(Loop());
    }

    // Increase the number of calls to Update.
    void Update()
    {
        updateCount += 1;
    }

    // Increase the number of calls to FixedUpdate.
    void FixedUpdate()
    {
        fixedUpdateCount += 1;
    }

    // Show the number of calls to both messages.
    void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 24;
        GUI.Label(new Rect(100, 100, 200, 50), "Update: " + updateUpdateCountPerSecond.ToString(), fontSize);
        GUI.Label(new Rect(100, 150, 200, 50), "FixedUpdate: " + updateFixedUpdateCountPerSecond.ToString(), fontSize);
    }

    // Update both CountsPerSecond values every second.
    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            updateUpdateCountPerSecond = updateCount;
            updateFixedUpdateCountPerSecond = fixedUpdateCount;

            updateCount = 0;
            fixedUpdateCount = 0;
        }
    }
}
