using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CootsAnimationController : MonoBehaviour
{
    public Animator animator;
    public SkinnedMeshRenderer meshRenderer;

    public Material napMaterial;
    public Material neutralMaterial;
    public Material screamMaterial;

    public Transform targetPos;
    public float speed;

    private Dictionary<CootsMood, Material> materialByMood = new Dictionary<CootsMood, Material>();
    private Dictionary<int, string> currentState = new Dictionary<int, string>();

    public bool moving = false;
    public bool idling = true;

    void Start()
    {
        materialByMood.Add(CootsMood.NEUTRAL, neutralMaterial);
        materialByMood.Add(CootsMood.NAP, napMaterial);
        materialByMood.Add(CootsMood.SCREAM, screamMaterial);
    }

    void Update()
    {
        if (!moving)
        {
            if (idling) IdleAnim();
            return;
        }
        else MoveCoots();
    }

    public void ChangeAnimationState(string newAnimationStateName, int layer = 0)
    {
       if (animator.GetCurrentAnimatorStateInfo(layer).IsName(newAnimationStateName)) return;

       animator.Play(newAnimationStateName, layer);
       currentState[layer] = newAnimationStateName;
    }

    public void ChangeMaterial(CootsMood cootsMood)
    {
        Material material = materialByMood[cootsMood];
        meshRenderer.material = material;
    }

    public void SetCootsMoving(bool moving)
    {
        this.moving = moving;
    }

    public bool GetCootsMoving()
    {
        return this.moving;
    }

    public void MoveCoots()
    {
        if (Vector3.Distance(this.transform.position, targetPos.position) < 0.5f)
        {
            this.moving = false;
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos.position, speed * Time.deltaTime);
            this.moving = true;
        }
    }

    private void IdleAnim()
    {
       if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
       {
           ChangeAnimationState(idleAnimationStates[Random.Range(0, idleAnimationStates.Count*10)/10]);
       }
    }

    public enum CootsMood
    {
        NEUTRAL,
        SCREAM,
        NAP
    }


    private static readonly List<string> idleAnimationStates = new List<string> { "squat", "spin", "lookup", "wriggle", "hop" };
}
