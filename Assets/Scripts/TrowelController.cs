using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrowelController : Digger
{
    public GameObject trowel;

    public bool useLerpForTrowelMovement = true;
    public float trowelMoveSpeedNormal = 4.0f;
    public float trowelMoveSpeedAiming = 2.0f;

    public float trowelDistanceFromCamera = 2.0f;
    public float trowelDrawPathDistanceFromCamera = 20f;

    private bool aiming = false;

    private Vector3 positionTarget;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        //might not need to be confined

        positionTarget = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        HandleTrowelMovement();
        if (Input.GetMouseButton(0))
            Dig();
    }

    public override void Dig()
    {
        foreach (Diggable diggable in diggables)
        {
            if (diggable != null)
                diggable.TryDig(trowel.transform.position.Horizontal());
        }
    }

    void HandleTrowelMovement()
    {
        Vector3 newTrowelPosition;

        if (Input.GetAxis("Mouse Y") == 0 && Input.GetAxis("Mouse X") == 0)
        {
            newTrowelPosition = positionTarget;
        }
        else
        {
            newTrowelPosition = WorldPosWithRange();
            positionTarget = newTrowelPosition;
        }

        float speed = aiming ? trowelMoveSpeedAiming : trowelMoveSpeedNormal;
        if (useLerpForTrowelMovement)
            trowel.transform.position = Vector3.Lerp(
                trowel.transform.position,
                newTrowelPosition,
                speed * Time.deltaTime
            );
        else
            trowel.transform.position = newTrowelPosition;
    }

    void HandleAiming()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aiming = true;
            CameraController.Instance.AimCamera();
        }
        else if (aiming && Input.GetKeyUp(KeyCode.Mouse1))
        {
            CameraController.Instance.DefaultCamera();
            aiming = false;
        }
    }

    private Vector3 WorldPosWithRange()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float intersectDistance = default;
        new Plane(Vector3.up, Vector3.zero).Raycast(ray, out intersectDistance);
        Vector3 zeroedIntersectPoint = ray.origin + ray.direction * intersectDistance;

        return zeroedIntersectPoint + Vector3.up * 0.5f;
        /*Vector3 worldPos;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        float distance = Input.GetKey(KeyCode.Mouse0)
            ? trowelDrawPathDistanceFromCamera
            : trowelDistanceFromCamera;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, distance))
        {
            worldPos = hit.point;
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(mousePos.x, mousePos.y, trowelDistanceFromCamera)
            );
        }

        return worldPos;*/
    }
}
