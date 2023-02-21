using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUi : MonoBehaviour
{
    public float uiYPos = 50f;
    public float uiXPos = 50f;
    public UISettings uiSettings;

    private RectTransform currentUI;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        ShowStartUI();       
        //block inputs
    }


    public void ShowStartUI()
    {
        var ui = uiSettings
            .MakeUi(AnchorUtil.Centre(uiXPos, uiYPos))
            .AddChildren(
                    uiSettings.Text(""),
                    uiSettings.Button("Start to Scoop!", () => ClearStartUI())
            );

        currentUI = ui;
    }

    public void ClearStartUI()
    {
        UISettings.DestroyUi(currentUI);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        currentUI = null;
    }
}
