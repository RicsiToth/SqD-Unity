using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenshotScript : MonoBehaviour
{
	public KeyCode screenshotKey = KeyCode.Print;
    public string fileName;

    private Transform canvas;
    private Transform camera;
    private Camera camComponent;
    
    void Start () {
		canvas = GameObject.Find("Canvas").transform;
		camera = transform;
		camComponent = transform.GetComponent<Camera>();
    }

	void TakeScreenshot(){
		ScreenCapture.CaptureScreenshot($"Screenshots/{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
	}

	void Update()
	{
		var pos = canvas.position;
		pos.z = -1;
		camera.position = pos;
		camComponent.orthographicSize = pos.y;
		
		if (Input.GetKey(screenshotKey))
		{
			TakeScreenshot();
		}
	}
}
