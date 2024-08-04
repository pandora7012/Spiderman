using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : IState
{
    public PlayerController player;
    public float airControl = 1000f;
    public bool delaySwingCheck;
    public float maxSwingSpeed = 30;
    public float maxAngularVelocity = 30;

    private bool canInteract = false;

    public FallingState(PlayerController player)
    {
        this.player = player;
    }


    public void StateEnter()
    {
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsFalling, true);
        canInteract = false;
        player.DelayExecution(0.5f, () => { canInteract = true; });
    }

    public void HandleInput()
    {
        HandleMovement();
        HandleSwingInput();
    }

    public void LogicUpdate()
    {
    }

    public void UpdatePhysics()
    {
        LimitSpeed();
        CheckForGround();
        CheckForClimbable();
    }

    public void StateExit()
    {
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsFalling, false);
    }

    private void CheckForGround()
    {
        player.GetAnimator().SetFloat(Constants.Animation.Parameters.YVelocity, player.GetRigidbody().velocity.y);
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 0.3f))
        {
            player.ChangeState(new GroundedState(this.player));
        }

        Debug.DrawRay(player.transform.position, Vector3.down * 0.3f, Color.red);
    }

    private void CheckForClimbable()
    {
        if (!canInteract)
            return;
        RaycastHit hit;
        if (Physics.Raycast(player.GetRaycastOrigin().position, player.transform.forward, out hit, 0.5f))
        {
            if (hit.collider.CompareTag(Constants.Tags.Wall))
            {
                player.ChangeState(new ClimbingState(this.player));
            }
        }

        Debug.DrawRay(player.GetRaycastOrigin().position, player.transform.forward * 0.5f, Color.red);
    }

    private void HandleMovement()
    {
        if (!canInteract)
            return;

        float moveHorizontal = Input.GetAxis(Constants.Input.Horizontal);
        float moveVertical = Input.GetAxis(Constants.Input.Vertical);
        Vector3 inputDirection = new Vector3(moveHorizontal, 0, moveVertical);
        Vector3 worldDirection = Camera.main.transform.TransformDirection(inputDirection);
        worldDirection.y = 0;
        worldDirection.Normalize();
        Vector3 velocity = worldDirection * (airControl * Time.deltaTime);
        player.GetRigidbody().AddForce(velocity);
        if (worldDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
            player.transform.rotation =
                Quaternion.RotateTowards(player.transform.rotation, targetRotation, Time.deltaTime * airControl);
        }

        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Speed, worldDirection.magnitude);
    }

    private void HandleSwingInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ChangeState(new SwingingState(player));
        }
    }

    void LimitSpeed()
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
}