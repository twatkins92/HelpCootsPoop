using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGarden : MonoBehaviour
{
    public IntVariable poopsCleared_so;

    public float uiYPos = 50f;
    public float showFinalMenuAfterTime = 2.0f;
    public UISettings uiSettings;

    private RectTransform currentUI;
    private string CootsMood = "Happy";

    private CootsAnimationController cootsAnimationController;

    private bool finished = false;
    private bool finishing = false;

    void Start()
    {
        cootsAnimationController = FindObjectOfType<CootsAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1 && Input.GetKeyDown(KeyCode.Return) && !finished && !finishing)
        {
            //calculate score
            Debug.Log("score is : " + poopsCleared_so.Value);
            //then ruin next animation based on happiness
            CameraController.Instance.AimCamera();
            cootsAnimationController.SetCootsMoving(true);
            finishing = true;
        }

        if (finishing && !finished) FinishGame();
        if (finished && Input.anyKey) PlayAnimationBasedOnScoreAndMood();
    }

    public void FinishGame()
    {
        bool cootsMoving = cootsAnimationController.GetCootsMoving();

        if (!cootsMoving)
        {
            cootsAnimationController.ChangeMaterial(CootsAnimationController.CootsMood.NAP);
            PlayAnimationBasedOnScoreAndMood();
            ShowCootsMoodUI();
            //resetting here but need to put this in the start/restart logic
            poopsCleared_so.Value = 0;
            this.DoAfter(showFinalMenuAfterTime, () => ShowFinishUI());
            finishing = false;
            finished = true;
        }
    }

    private void PlayAnimationBasedOnScoreAndMood()
    {
        cootsAnimationController.ChangeAnimationState("Jump");
    }

    public void ShowCootsMoodUI()
    {
        ClearFinishUI();

        var ui = uiSettings
            .MakeUi(AnchorUtil.BottomCentre(uiYPos))
            .AddChildren(
                uiSettings.Text("You've completed your garden"),
                uiSettings.Text("Coots can poop freely"),
                uiSettings.Text("Poops seems to be " + CootsMood)
            );

        currentUI = ui;
    }

    public void ShowFinishUI()
    {
        ClearFinishUI();

        var ui = uiSettings
            .MakeUi(AnchorUtil.BottomCentre(uiYPos))
            .AddChildren(
                uiSettings.Button("Play Again", () => Transitions.Start("SimpleFade", "Game")),
                uiSettings.Button("Quit", () => Application.Quit())
            );

        currentUI = ui;
    }

    public void ClearFinishUI()
    {
        if (currentUI != null)
        {
            UISettings.DestroyUi(currentUI);
            currentUI = null;
        }
    }
}
