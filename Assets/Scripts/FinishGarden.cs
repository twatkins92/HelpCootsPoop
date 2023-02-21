using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGarden : MonoBehaviour
{
    public IntVariable poopsCleared_so;

    public float uiYPos = 50f;
    public UISettings uiSettings;

    private RectTransform currentUI;
    private string CootsMood = "Happy";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //calculate score
            //trigger coots animator
            //bring up ui for end/regen
            Debug.Log("score is : " + poopsCleared_so.Value);
            ShowFinishUI();
            //resetting here but need to put this in the start/restart logic
            poopsCleared_so.Value = 0;
        }
    }

    public void ShowFinishUI()
    {
        var ui = uiSettings
            .MakeUi(AnchorUtil.BottomCentre(uiYPos))
            .AddChildren(
                uiSettings.Text("You've completed your garden and now Coots can poop freely"),
                uiSettings.Text("Poops seems to be " + CootsMood), 
                //reload scene
                uiSettings.Button("Play Again", () => Transitions.Start("SimpleFade", "Game")),
                uiSettings.Button("Quit", () => Application.Quit())
            );

        currentUI = ui;
    }

    public void ClearFinishUI()
    {
        UISettings.DestroyUi(currentUI);
        currentUI = null;
    }
}
