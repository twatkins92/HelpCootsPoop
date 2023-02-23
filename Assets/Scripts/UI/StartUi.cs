using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUi : MonoBehaviour
{
    public float uiYPos = 50f;
    public float uiXPos = 50f;
    public UISettings uiSettings;

    private RectTransform currentUI;

    private AphorismsUi aphorismsUi;

    private TrowelController trowel;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        trowel = FindObjectOfType<TrowelController>();
        trowel.enabled = false;

        ShowStartUI();
        aphorismsUi = FindObjectOfType<AphorismsUi>();

        SoundManager.PlayMusic("CootsBackgroundMusic", 1f);
    }

    public void ShowStartUI()
    {
        var ui = uiSettings
            .MakeUi(AnchorUtil.Centre(uiXPos, uiYPos))
            .AddChildren(
                uiSettings.Title(
                    cootsMoodObjectives[Random.Range(0, cootsMoodObjectives.Count * 10) / 10]
                ),
                uiSettings.Text(
                    "Create a zen litter garden to put Coots in the perfect mood to poop."
                ),
                uiSettings.Text(""),
                uiSettings.Text("[M1]: Part the Litter"),
                uiSettings.Text("[Enter]: Finish your Creation"),
                uiSettings.Text(""),
                uiSettings.Text(""),
                uiSettings.Button("Scoop!", () => ClearStartUI())
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
        aphorismsUi.EnableCamera(true);
        trowel.enabled = true;
    }

    private static readonly List<string> cootsMoodObjectives = new List<string>
    {
        "Coots is currently spaced out",
        "Coots' nerves are getting the better of him",
        "Coots is afraid to step foot into the tray",
        "Coots requires soothing",
        "Coots is lacking excitement"
    };
}
