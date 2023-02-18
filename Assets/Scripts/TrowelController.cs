using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrowelController : MonoBehaviour
{
    public GameObject trowel;

    public bool useLerpForTrowelMovement = true;
    public float trowelMoveSpeedNormal = 4.0f;
    public float trowelMoveSpeedAiming = 2.0f;
    public float trowelDistanceFromCamera = 2.0f;

    private bool aiming = false;

    // Start is called before the first frame update
    void Start()
    {
        //might not need to be confined
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleTrowelMovement();
        HandleAiming();
        HandleFlick();
    }
    
    void HandleTrowelMovement()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 newTrowelPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.transform.position.z);
        newTrowelPosition = WorldPosWithRange();
        float speed = aiming ? trowelMoveSpeedAiming : trowelMoveSpeedNormal;
        if (useLerpForTrowelMovement)
            trowel.transform.position = Vector3.Lerp(trowel.transform.position, newTrowelPosition, speed * Time.deltaTime);
        else
            trowel.transform.position = newTrowelPosition;
    }

    void HandleAiming()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aiming = true;
            //set cameras in camera controller
        }
        else if (aiming && Input.GetKeyUp(KeyCode.Mouse1))
        {
            CameraController.Instance.DefaultCamera();
            aiming = false;
        }
    }

    void HandleFlick()
    {
        if (aiming && Input.GetKeyDown(KeyCode.Mouse0))
        {
            //play animation
            //do flick
        }
    }

    private Vector3 WorldPosWithRange()
    {
        Vector3 worldPos;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, trowelDistanceFromCamera))
        {
           worldPos = hit.point;
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, trowelDistanceFromCamera));

        }

        return worldPos;
    }

}
