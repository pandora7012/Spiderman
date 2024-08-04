using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingState : IState
{
    private PlayerController player;
    public float speed = 25f;

    private Vector3 wallNormal;

    public ClimbingState(PlayerController player)
    {
        this.player = player;
        player.GetRigidbody().isKinematic = true;
    }

    public void StateEnter()
    {
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsClimbing, true);
        player.GetRigidbody().velocity = Vector3.zero;
        player.GetRigidbody().useGravity = false;
        //   player.GetCameraController().enabled = false;
    }

    public void HandleInput()
    {
        HandleClimbingInput();
        HandleInputToJump();
    }

    public void LogicUpdate()
    {
        CheckForWall();
    }

    public void UpdatePhysics()
    {
        AlignWithWall();
    }

    public void StateExit()
    {
        player.GetRigidbody().useGravity = true;
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsClimbing, false);
        //player.GetCameraController().enabled = true;
    }

    private void HandleClimbingInput()
    {
        float moveHorizontal = Input.GetAxisRaw(Constants.Input.Horizontal);
        float moveVertical = Input.GetAxisRaw(Constants.Input.Vertical);

        Vector3 moveDirection = new Vector3(moveHorizontal, Mathf.Max(0, moveVertical), 0);

        Vector3 localMoveDirection = player.transform.TransformDirection(moveDirection);


        Vector3 direction = Vector3.ProjectOnPlane(localMoveDirection, wallNormal).normalized;

        direction.Normalize();

        if (moveDirection != Vector3.zero)
        {
            Vector3 move = direction * speed * Time.deltaTime;
            player.GetRigidbody().MovePosition(player.transform.position + move);
        }

        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Horizontal, moveHorizontal);
        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Speed, direction.magnitude);
        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Vertical, moveVertical);
    }

    public void HandleInputToJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void AlignWithWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.GetRaycastOrigin().position, player.transform.forward, out hit, 1f))
        {
            wallNormal = hit.normal;
            Vector3 targetDirection = -hit.normal;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            player.transform.rotation =
                Quaternion.RotateTowards(player.transform.rotation, targetRotation, Time.deltaTime * 500);
        }
    }

    private void Jump()
    {
        player.GetRigidbody().isKinematic = false;
        Vector3 jumpForce = (wallNormal + Vector3.up * 2) * 5;
        player.GetRigidbody().AddForce(jumpForce, ForceMode.Impulse);
        player.ChangeState(new FallingState(player));
    }

    private void CheckForWall()
    {
        RaycastHit hit;
        if (!Physics.Raycast(player.GetRaycastOrigin().position, player.transform.forward, out hit, 1f))
        {
            Jump();
        }
    }
}