using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGarden : MonoBehaviour
{
    public IntVariable poopsCleared_so;

    public float uiYPos = 50f;
    public float showFinalMenuAfterTime = 2.0f;
    public UISettings uiSettings;

    public float successfulScore = 100f;

    private RectTransform currentUI;
    private CootsAnimationController.CootsMood cootsMood;

    private CootsAnimationController cootsAnimationController;
    private AphorismsUi aphorismsUi;
    private MeshClicker meshClicker;

    private bool finished = false;
    private bool finishing = false;
    private bool poopsCleared = false;

    private Dictionary<CootsAnimationController.CootsMood, List<string>> cootsMoodToStringList = new Dictionary<CootsAnimationController.CootsMood, List<string>>();

    void Start()
    {
        cootsAnimationController = FindObjectOfType<CootsAnimationController>();
        aphorismsUi = FindObjectOfType<AphorismsUi>();
        meshClicker = FindObjectOfType<MeshClicker>();

        cootsMoodToStringList.Add(CootsAnimationController.CootsMood.NEUTRAL, neutralSynonyms);
        cootsMoodToStringList.Add(CootsAnimationController.CootsMood.NAP, napSynonyms);
        cootsMoodToStringList.Add(CootsAnimationController.CootsMood.SCREAM, screamSynonyms);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1 && Input.GetKeyDown(KeyCode.Return) && !finished && !finishing)
        {
            cootsMood = FigureOutCootsMood();
            aphorismsUi.EnableCamera(false);
            CameraController.Instance.AimCamera();
            cootsAnimationController.ChangeAnimationState("Run");
            cootsAnimationController.patrolling = false;
            cootsAnimationController.SetCootsMoving(true);
            finishing = true;

            TrowelController trowel = FindObjectOfType<TrowelController>();
            if (trowel != null) Destroy(trowel);
        }

        if (finishing && !finished) FinishGame();
        if (finished && Input.anyKey) PlayAnimationBasedOnScoreAndMood();
        if (!poopsCleared && poopsCleared_so.Value == 0) ShowPoopsClearedUI();
    }

    public void FinishGame()
    {
        bool cootsMoving = cootsAnimationController.GetCootsMoving();

        if (!cootsMoving)
        {
            cootsAnimationController.idling = false;
            cootsAnimationController.ChangeMaterial(cootsMood);
            PlayAnimationBasedOnScoreAndMood();
            ShowCootsMoodUI();
            this.DoAfter(showFinalMenuAfterTime, () => ShowFinishUI());
            finishing = false;
            finished = true;
        }
    }

    private void PlayAnimationBasedOnScoreAndMood()
    {
        string animation = "Jump";
        if (cootsMood == CootsAnimationController.CootsMood.SCREAM) animation = screamAnimations[Random.Range(0, screamAnimations.Count*10)/10];
        if (cootsMood == CootsAnimationController.CootsMood.NAP) animation = napAnimations[Random.Range(0, napAnimations.Count*10)/10];
        if (cootsMood == CootsAnimationController.CootsMood.NEUTRAL) animation = neutralAnimations[Random.Range(0, neutralAnimations.Count*10)/10];
        cootsAnimationController.ChangeAnimationState(animation);
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

    public void ShowPoopsClearedUI()
    {
        poopsCleared = true;
        ClearFinishUI();

        var ui = uiSettings
            .MakeUi(AnchorUtil.BottomCentre(uiYPos))
            .AddChildren(
                uiSettings.Text("The poops are cleared"),
                uiSettings.Text("feel free to keep brushing"),
                uiSettings.Text("or hit [Enter] to finish up here")
            );
        
        currentUI = ui;

        this.DoAfter(5f, () => ClearFinishUI());
    }

    public void ShowFinishUI()
    {
        ClearFinishUI();

        var ui = uiSettings
            .MakeUi(AnchorUtil.BottomCentre(uiYPos))
            .AddChildren(
                uiSettings.Button("Clean Another?", () => Transitions.Start("SimpleFade", "End2End")),
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

    public CootsAnimationController.CootsMood FigureOutCootsMood()
    {
        if (poopsCleared_so.Value > 0) return CootsAnimationController.CootsMood.SCREAM;
        if (meshClicker.score < successfulScore) return CootsAnimationController.CootsMood.NEUTRAL;
        else return CootsAnimationController.CootsMood.NAP;
    }

    private string GetRandomStringFromList(List<string> list)
    {
        return list[Random.Range(0, list.Count*10)/10];
    }

    private static readonly List<string> napSynonyms = new List<string> { "contented", "at one with himself", "on cloud nine", "as peaceful as could be", "ready for a nap" }; 

    private static readonly List<string> neutralSynonyms = new List<string> { "neither here no there", "contemplating something greater", "done with your presence", "shy, so turn around" };

    private static readonly List<string> screamSynonyms = new List<string> { "displeased", "disgusted" };

    private static readonly List<string> neutralAnimations = new List<string> { "wriggle", "squat" };
    private static readonly List<string> napAnimations = new List<string> { "Jump", "spin", "hop" };
    private static readonly List<string> screamAnimations = new List<string> { "tantrum", "lookup" };
}
