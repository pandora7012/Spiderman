using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public PlayerController player;
    public float minFOV = 40f;
    public float maxFOV = 90f;
    public float zoomSpeed = 10f;

    [SerializeField] private Rigidbody playerRigidbody;
    public CinemachinePostProcessing postProcessing;
    public MotionBlur motionBlur;

    private float targetFOV;

    private void Start()
    {
        targetFOV = freeLookCamera.m_Lens.FieldOfView;
        postProcessing.m_Profile.TryGetSettings(out motionBlur);
        this.motionBlur = motionBlur;
    }

    private void LateUpdate()
    {
        AdjustFOV();
        MotionBlurEffect();
    }

    private void AdjustFOV()
    {
        if (playerRigidbody.velocity.y > 5)
        {
            targetFOV = maxFOV;
        }
        else if (playerRigidbody.velocity.y < -5)
        {
            targetFOV = minFOV;
        }
        else
        {
            targetFOV = 40;
        }

        freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(freeLookCamera.m_Lens.FieldOfView, targetFOV, zoomSpeed * Time
            .deltaTime);
    }

    public void MotionBlurEffect()
    {
        float playerSpeed = player.GetRigidbody().velocity.magnitude;
        
        Debug.Log(playerSpeed);

        if (playerSpeed > 10)
        {
            motionBlur.active = true;
            Debug.Log("AAAA");
            motionBlur.shutterAngle.value = Mathf.Lerp(0, 270, (playerSpeed - 10) / 10);
        }
        else
        {
            motionBlur.active = false;
        }
    }
}