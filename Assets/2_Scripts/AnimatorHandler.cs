using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator anim;
    public InputHandler inputHandler;
    public PlayerLocomotion playerLocomotion;

    public bool canRotate;

    private int _vertical;
    private int _horizontal;

    public void Initialize()
    {
        anim = GetComponent<Animator>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        _vertical = Animator.StringToHash("Vertical");
        _horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValue(float verticalMovement, float horizontalMovement)
    {
        #region Vertical

        float v = 0;

        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < 0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }

        #endregion

        #region Horizontal

        float h = 0;

        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }

        #endregion

        anim.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(targetAnimation, 0.2f);
    }

    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotate()
    {
        canRotate = false;
    }

    private void OnAnimatorMove()
    {
        if (inputHandler.isInteracting == false)
            return;

        float delta = Time.deltaTime;
        playerLocomotion.rigidbody.linearDamping = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerLocomotion.rigidbody.linearVelocity = velocity;
    }
}