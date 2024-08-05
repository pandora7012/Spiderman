using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SwingingState : IState
{
    PlayerController player;

    public Vector3 swingPoint;
    public float swingRadius = 30f;
    public float damping = 15f;
    public float airControl = 20f;
    public float maxSwingSpeed = 40;
    public float maxAngularVelocity = 30;
    public float releaseForce = 10;
    private int animRandom = 1;

    public SwingingState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void StateEnter()
    {
        animRandom = Random.Range(1, 10);
        animRandom = 9; 
        swingPoint = player.transform.position + player.transform.up * swingRadius +
                     player.transform.forward * swingRadius / 4;
        float moveHorizontal = Input.GetAxis(Constants.Input.Horizontal);


        var vector = player.transform.right * -moveHorizontal * swingRadius / 1.5f;
        swingPoint += vector;

        
        player.GetRigidbody().AddForce(player.transform.forward * 20, ForceMode.Impulse);
        player.GetAnimator().SetInt(Constants.Animation.Parameters.RandomSlideAnimation, animRandom);
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsSwinging, true);

        var springJoint = player.GetSpringJoint();

        player.SetupSpringJoint(swingPoint);


        springJoint.spring = 100f;
        springJoint.damper = damping;
        springJoint.massScale = 1f;

        springJoint.maxDistance = swingRadius;
        springJoint.minDistance = swingRadius;
    }


    public Transform GetHandRopeAsAnimation()
    {
        if (animRandom == 9)
        {
            return player.GetHandGrabRight();
        }

        return player.GetHandGrabLeft();
    }
    
    public void HandleInput()
    {
        HandleMovement();
        InputEscapeSwing();
    }

    public void LogicUpdate()
    {
        UpdateLineRenderer();
        CheckForClimbable();
    }

    public void UpdatePhysics()
    {
        LimitSwingSpeed();
    }

    public void StateExit()
    {
        var springJoint = player.GetSpringJoint();
        springJoint.spring = 0;
        springJoint.damper = 0;

        Vector3 releaseDirection = player.transform.forward / 2 + player.transform.up;
        player.GetRigidbody().AddForce(releaseDirection * releaseForce, ForceMode.Impulse);

        LimitSwingSpeed();
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsSwinging, false);
    }

    private void CheckForClimbable()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, 0.5f))
        {
            if (hit.collider.CompareTag(Constants.Tags.Wall))
            {
                player.ChangeState(new ClimbingState(this.player));
            }
        }

        Debug.DrawRay(player.transform.position, player.transform.forward * 0.5f + player.transform.up * 1f, Color.red);
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis(Constants.Input.Horizontal);
        float moveVertical = Input.GetAxis(Constants.Input.Vertical);

        Vector3 direction = new Vector3(moveHorizontal, 0, moveVertical);
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0.0f;
        direction.z = 0.0f;
        direction.Normalize();

        Vector3 velo = direction * (airControl * Time.deltaTime);
        player.GetRigidbody().velocity += velo;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation =
                Quaternion.RotateTowards(player.transform.rotation, targetRotation, Time.deltaTime * 100);
        }

        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Speed, direction.magnitude);
    }

    private void InputEscapeSwing()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            player.ChangeState(new FallingState(player));
        }
    }

    void LimitSwingSpeed()
    {
        var rb = player.GetRigidbody();
        if (rb.velocity.magnitude > maxSwingSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSwingSpeed;
        }

        if (rb.angularVelocity.magnitude > maxAngularVelocity)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
        }
    }

    public void UpdateLineRenderer()
    {
        player.SetupLineRenderer(true, player.GetJointAnchor(), GetHandRopeAsAnimation());
    }
}