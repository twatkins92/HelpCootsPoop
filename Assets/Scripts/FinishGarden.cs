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
    private CootsAnimationController.CootsMood cootsMood;

    private CootsAnimationController cootsAnimationController;

    private bool finished = false;
    private bool finishing = false;

    private Dictionary<CootsAnimationController.CootsMood, List<string>> cootsMoodToStringList = new Dictionary<CootsAnimationController.CootsMood, List<string>>();

    void Start()
    {
        cootsAnimationController = FindObjectOfType<CootsAnimationController>();

        cootsMoodToStringList.Add(CootsAnimationController.CootsMood.NEUTRAL, neutralSynonyms);
        cootsMoodToStringList.Add(CootsAnimationController.CootsMood.NAP, napSynonyms);
        cootsMoodToStringList.Add(CootsAnimationController.CootsMood.SCREAM, screamSynonyms);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1 && Input.GetKeyDown(KeyCode.Return) && !finished && !finishing)
        {
            //calculate score
            Debug.Log("score is : " + poopsCleared_so.Value);
            cootsMood = CootsAnimationController.CootsMood.NEUTRAL;
            //then ruin next animation based on happiness
            CameraController.Instance.AimCamera();
            cootsAnimationController.SetCootsMoving(true);
            finishing = true;

            TrowelController trowel = FindObjectOfType<TrowelController>();
            if (trowel != null) Destroy(trowel);
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
                uiSettings.Text("Coots seems to be " + GetRandomStringFromList(cootsMoodToStringList[cootsMood]))
            );

        currentUI = ui;
    }

    public void ShowFinishUI()
    {
        ClearFinishUI();

        var ui = uiSettings
            .MakeUi(AnchorUtil.BottomCentre(uiYPos))
            .AddChildren(
                uiSettings.Button("Play Again", () => Transitions.Start("SimpleFade", "End2End")),
                uiSettings.Button("Quit", () => Application.Quit())
            );

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

    private string GetRandomStringFromList(List<string> list)
    {
        return list[Random.Range(0, list.Count*10)/10];
    }

    private static readonly List<string> napSynonyms = new List<string> { "contented", "at one with himself", "on cloud nine", "as peaceful as could be", "ready for a nap" }; 

    private static readonly List<string> neutralSynonyms = new List<string> { "neither here no there", "contemplating something greater", "done with your presence", "shy, so turn around" };

    private static readonly List<string> screamSynonyms = new List<string> { "displeased", "disgusted" };
}
