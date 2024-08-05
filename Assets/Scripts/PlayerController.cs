using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private IState currentState;

    [SerializeField] private Rigidbody RB;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private SpringJoint springJoint;
    [SerializeField] private Transform jointAnchor;

    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private Transform handGrabL;
    [SerializeField] private Transform handGrabR;

    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private CameraController cameraController;

    public float gravityScale = 2f;

    public static float globalGravity = -9.81f;

    private void Start()
    {
        currentState = new GroundedState(this);
    }

    public void DelayExecution(float time, Action action)
    {
        StartCoroutine(IE_DelayExecution(time, action));
    }

    public CameraController GetCameraController()
    {
        return cameraController;
    }

    private IEnumerator IE_DelayExecution(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    private void Update()
    {
        currentState.HandleInput();
        currentState.LogicUpdate();
    }


    private void FixedUpdate()
    {
        CustomGravity();
        currentState.UpdatePhysics();
    }

    public void SetupLineRenderer(bool isEnable, Transform pos1 = null, Transform pos2 = null)
    {
        lineRenderer.gameObject.SetActive(isEnable);
        if (isEnable)
        {
            lineRenderer.SetPosition(0, pos1.position);
            lineRenderer.SetPosition(1, pos2.position);
        }
    }


    public void ChangeState(IState nextState)
    {
        Debug.Log("Change state from " + currentState + " to " + nextState);
        currentState?.StateExit();
        currentState = nextState;
        currentState.StateEnter();
    }

    public Transform GetJointAnchor()
    {
        return jointAnchor;
    }


    public Rigidbody GetRigidbody()
    {
        return RB;
    }

    public AnimationController GetAnimator()
    {
        return animationController;
    }

    public SpringJoint GetSpringJoint()
    {
        return springJoint;
    }

    public void SetupSpringJoint(Vector3 pos)
    {
        jointAnchor.position = pos;
    }

    public Transform GetHandGrabLeft()
    {
        return handGrabL;
    }

    public Transform GetHandGrabRight()
    {
        return handGrabR;
    }


    private void OnDrawGizmos()
    {
        if (currentState is SwingingState)
        {
            Gizmos.color = Color.red;
            var state = currentState as SwingingState;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            Gizmos.DrawWireSphere(state.swingPoint, 0.1f);
            Gizmos.DrawWireSphere(state.swingPoint, state.swingRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, jointAnchor.position);
        }
    }

    public void CustomGravity()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        RB.AddForce(gravity, ForceMode.Acceleration);
    }

    public Transform GetRaycastOrigin()
    {
        return raycastOrigin;
    }
}