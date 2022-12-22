using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenshotScript : MonoBehaviour
{
    public Button yourButton;
    public string fileName;

    private Transform canvas;
    private Transform camera;
    private Camera camComponent;
    
    void Start () {
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);

		canvas = GameObject.Find("Canvas").transform;
		camera = transform;
		camComponent = transform.GetComponent<Camera>();
    }

	void TaskOnClick(){
		ScreenCapture.CaptureScreenshot($"Screenshots/{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
	}

	void Update()
	{
		var pos = canvas.position;
		pos.z = -1;
		camera.position = pos;
		camComponent.orthographicSize = pos.y;
	}
}
