using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotScript : MonoBehaviour
{
    public Button yourButton;
    public string fileName;

    void Start () {
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		ScreenCapture.CaptureScreenshot($"Screenshots/{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
	}
}
