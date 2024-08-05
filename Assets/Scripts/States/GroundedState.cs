using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : IState
{
    private PlayerController player;
    public float moveSpeed = 20f;
    private float rotationSpeed = 700f;

    public GroundedState(PlayerController player)
    {
        this.player = player;
    }

    public void StateEnter()
    {
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsGrounded, true);
        player.GetRigidbody().velocity = Vector3.zero;
    }

    public void HandleInput()
    {
        HandleMovement();
        HandleJumpInput();
    }

    public void LogicUpdate()
    {
        CheckForClimbable();
        CheckForGrounding();
    }

    public void UpdatePhysics()
    {
    }

    public void StateExit()
    {
        player.GetAnimator().SetBool(Constants.Animation.Parameters.IsGrounded, false);
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis(Constants.Input.Horizontal);
        float moveVertical = Input.GetAxis(Constants.Input.Vertical);

        Vector3 direction = new Vector3(moveHorizontal, 0, moveVertical);
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0;
        direction.Normalize();

        if (direction != Vector3.zero)
        {
            Vector3 move = direction * moveSpeed * Time.deltaTime;
            player.GetRigidbody().MovePosition(player.transform.position + move);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation,
                Time.deltaTime *
                rotationSpeed);
        }

        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Speed, direction.magnitude);
        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Horizontal, moveHorizontal);
        player.GetAnimator().SetFloat(Constants.Animation.Parameters.Vertical, moveVertical);
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ChangeState(new JumpingState(player));
        }
    }

    private void CheckForClimbable()
    {
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position + new Vector3(0, 1, 0), player.transform.forward, out hit, 0.75f))
        {
            if (hit.collider.CompareTag(Constants.Tags.Wall))
            {
                player.ChangeState(new ClimbingState(this.player));
            }
        }

        Debug.DrawRay(player.transform.position + new Vector3(0, 1, 0), player.transform.forward * 0.75f, Color.red);
    }

    private void CheckForGrounding()
    {
        RaycastHit hit;
        if (!Physics.Raycast(player.transform.position, Vector3.down, out hit, 0.5f))
        {
            player.ChangeState(new FallingState(player));
        }
    }
}