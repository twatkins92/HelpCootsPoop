using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AphorismsUi : MonoBehaviour
{
    public float uiYPos = 50f;
    public float uiXPos = 50f;
    public float fadeAphorismAfterTime = 1f;

    public UISettings uiSettings;

    public Camera catCamera;

    private RectTransform currentUI;

    // Start is called before the first frame update
    void Start()
    {
        EnableCamera(false);
    }

    public void EnableCamera(bool enable)
    {
        catCamera.gameObject.SetActive(enable);
    }

    public void ShowAphorism()
    {
        if (currentUI != null) return;
        var ui = uiSettings
            .MakeUi(AnchorUtil.TopRight(uiXPos, uiYPos))
            .AddChildren(
                    uiSettings.Text(GetAphorism())
            );

        currentUI = ui;
        this.DoAfter(fadeAphorismAfterTime, () => ClearAphorismUI());
    }

    private string GetAphorism()
    {
        return aphorisms[Random.Range(0, aphorisms.Count*10)/10];
    }

    private void ClearAphorismUI()
    {
        if (currentUI != null)
        {
            currentUI.gameObject.SetActive(false);
            this.DoAfter(fadeAphorismAfterTime, () => UISettings.DestroyUi(currentUI));
        }
    }

    private static readonly List<string> aphorisms = new List<string> { "You're doing just fine!", "Peaceful!", "At one with the poop!", "Zentacular", "Zentastic", "Good enough!", "Not quite", "Coots will appreciate that", "Your stream is good enough", "Great", "You'll do better", "Delightful" };
}
