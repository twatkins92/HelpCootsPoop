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

    //this is the final pos for him to walk to
    public Transform targetPos;
    public float speed;

    public List<Transform> patrolPoints = new List<Transform>();

    //this is is his next place to go
    private Transform nextPosition;
    private int patrolPositionIndex = 0;

    private Dictionary<CootsMood, Material> materialByMood = new Dictionary<CootsMood, Material>();
    private Dictionary<int, string> currentState = new Dictionary<int, string>();

    public bool moving = false;
    public bool idling = true;
    public bool patrolling = true;

    void Start()
    {
        materialByMood.Add(CootsMood.NEUTRAL, neutralMaterial);
        materialByMood.Add(CootsMood.NAP, napMaterial);
        materialByMood.Add(CootsMood.SCREAM, screamMaterial);

        if (patrolling) nextPosition = patrolPoints[patrolPositionIndex];
    }

    void Update()
    {
        if (patrolling)
        {
            Patrol();
            return;
        }
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
        ChangeMaterial(CootsMood.NEUTRAL);
        StopAllCoroutines();
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

    private void Patrol()
    {
        if (Vector3.Distance(this.transform.position, nextPosition.position) < 0.2f)
        {
            patrolPositionIndex++;
            if (patrolPositionIndex > patrolPoints.Count-1) patrolPositionIndex = 0;
            nextPosition = patrolPoints[patrolPositionIndex];
            this.transform.LookAt(nextPosition);
            patrolling = false;
            ChangeAnimationState("Idle");
            int switchVal = Random.Range(0, 10);
            if (switchVal > 5) ChangeMaterial(CootsMood.NAP);
            else ChangeMaterial(CootsMood.NEUTRAL);
            this.DoAfter(2.0f, () => patrolling = true);
        }
        else
        {
            ChangeAnimationState("walk");
            this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition.position, speed * Time.deltaTime);
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
