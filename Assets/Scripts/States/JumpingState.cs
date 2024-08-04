using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class JumpingState : IState
{
    PlayerController player;

    public float jumpForce = 15f;
    public float airControl = 300;
    public bool isGroundDelayCheck = false;

    public JumpingState(PlayerController player)
    {
        this.player = player;
    }

    public void StateEnter()
    {
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsJumping, true);
        player.GetRigidbody().velocity = Vector3.zero;
        player.GetRigidbody().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGroundDelayCheck = false;
        player.DelayExecution(0.5f, () => { isGroundDelayCheck = true; });
    }

    public void HandleInput()
    {
        WaitInputForSwing();
        HandleMovement();
    }

    public void LogicUpdate()
    {
        CheckForGround();

        CheckForClimbable();
    }

    public void UpdatePhysics()
    {
    }

    public void StateExit()
    {
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsJumping, false);
    }


    private void CheckForGround()
    {
        if (!isGroundDelayCheck)
            return;
        RaycastHit hit;


        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 0.3f))
        {
            if (hit.collider.CompareTag(Constants.Tags.Ground))
            {
                player.ChangeState(new GroundedState(this.player));
            }
        }

        Debug.DrawRay(player.transform.position, Vector3.down * 0.1f, Color.red);
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis(Constants.Input.Horizontal);
        float moveVertical = Input.GetAxis(Constants.Input.Vertical);

        Vector3 direction = new Vector3(moveHorizontal, 0, moveVertical);
        direction = Camera.main.transform.TransformDirection(direction);
        direction.Normalize();
        direction.y = 0;

        Vector3 velo = direction * (airControl * Time.deltaTime);

        player.GetRigidbody().AddForce(velo);
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation =
                Quaternion.RotateTowards(player.transform.rotation, targetRotation, Time.deltaTime * airControl);
        }

        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Speed, direction.magnitude);
    }


    private void CheckForClimbable()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.GetRaycastOrigin().position, player.transform.forward, out hit, 0.5f))
        {
            if (hit.collider.CompareTag(Constants.Tags.Wall))
            {
                player.ChangeState(new ClimbingState(this.player));
            }
        }

        Debug.DrawRay(player.transform.position, player.transform.forward * 0.5f, Color.red);
    }

    public void WaitInputForSwing()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ChangeState(new SwingingState(player));
        }
    }
}