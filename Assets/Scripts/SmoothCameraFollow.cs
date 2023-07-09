using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    private Transform player;
    public Transform CameraTarget;

    public Vector3 cameraDefaultOffset;
    public Vector3 mouseTargetOffset;
    public Vector3 cameraMovementOffset;
    public float damping;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.Player.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CameraMovement2();
    }

    void CameraMovement()
    {
        //Vector3 relativePos = CameraTarget.position
        Vector3 distanceToMouse = CameraTarget.position - player.position;
        Debug.Log(distanceToMouse);
        distanceToMouse.x = Mathf.Clamp(CameraTarget.position.x, -mouseTargetOffset.x + player.position.x, mouseTargetOffset.x + player.position.x);
        distanceToMouse.z = Mathf.Clamp(CameraTarget.position.z, -mouseTargetOffset.z + player.position.z, +mouseTargetOffset.z + player.position.z);
        //middlePoint.y = Mathf.Clamp(CameraTarget.position.z, -2.5f + Player.position.z, 2.5f + Player.position.z);
        //middlePoint.y = Mathf.Clamp(CameraTarget.position.z, -.5f + Player.position.z, .5f + Player.position.z);

        Vector3 movePosition = new Vector3(distanceToMouse.x, transform.position.y, transform.position.z + cameraDefaultOffset.z + distanceToMouse.z);
        //Vector3 movePosition = new Vector3(middlePoint.x, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);
    }

    void CameraMovement2()
    {
        Vector3 distanceToMouse = CameraTarget.position - player.position;
        //Debug.Log(distanceToMouse.z);
        distanceToMouse.x = Mathf.Clamp(distanceToMouse.x, -mouseTargetOffset.x, mouseTargetOffset.x);
        distanceToMouse.z = Mathf.Clamp(distanceToMouse.z, -mouseTargetOffset.z, mouseTargetOffset.z);
        //Debug.Log("After Clamp = " + distanceToMouse.z);
        Vector3 movePosition = new Vector3((distanceToMouse.x / mouseTargetOffset.x) * cameraMovementOffset.x, transform.position.y, (distanceToMouse.z / mouseTargetOffset.z) * cameraMovementOffset.z);
        //Debug.Log("movePos = " + movePosition.z);

        movePosition.x = player.position.x + movePosition.x;
        movePosition.z = player.position.z + movePosition.z + cameraDefaultOffset.z;
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);
    }

    public AnimationCurve camMoveCurve; 
}
