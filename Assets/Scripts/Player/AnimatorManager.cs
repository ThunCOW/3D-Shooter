using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [SerializeField] Animator PlayerAnimator;

    [HideInInspector]
    private string AimLayerNameOld;
    [SerializeField] private string _AimLayerName;
    public string AimLayerName
    {
        get { return _AimLayerName; }
        set 
        {
            AimLayerNameOld = AimLayerName == "" ? value : AimLayerName;
            _AimLayerName = value;
        }
    }

    private bool isAiming;

    void Awake()
    {
        PlayerAnimator = GetComponentInChildren<Animator>();
    }

    public void UpdateAnimatorValue(float horizontalMovement, float verticalMovement)               // Character Turn Animations
    {
        //Animation Snapping
#pragma warning disable CS0219 // Variable is assigned but its value is never used
        float snappedHorizontal;
        float snappedVertical;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if(horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if(horizontalMovement < 0 && horizontalMovement > 0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if(horizontalMovement < -0.55f )
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }
        #endregion

        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > 0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion

        PlayerAnimator.SetFloat("Horizontal", horizontalMovement, 0.1f, Time.deltaTime);
        PlayerAnimator.SetFloat("Vertical", verticalMovement, 0.1f, Time.deltaTime);
    }

    public void UpdateAimState(bool isAiming)
    {
        this.isAiming = isAiming;

        StopAllCoroutines();
        if (isAiming)
            StartCoroutine(AimWeightIncrease());
        else
            StartCoroutine(AimWeightDecrease());
    }
    public void UpdateAimLayer()
    {
        if (isAiming)
        {
            UpdateAimState(true);
        }
        ResetOldAimState();
    }

    float targetTime = 0.1f;
    float releaseTime = .4f;
    IEnumerator AimWeightIncrease()
    {
        float timePassed = 0;
        while(timePassed < targetTime)
        {
            timePassed += Time.deltaTime;
            
            PlayerAnimator.SetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerName), PlayerAnimator.GetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerName)) + timePassed / targetTime);
            yield return null;
        }
        PlayerAnimator.SetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerName), 1);
    }
    IEnumerator AimWeightDecrease()
    {
        float timePassed = releaseTime;
        while (timePassed > 0)
        {
            timePassed -= Time.deltaTime;
            PlayerAnimator.SetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerName), timePassed / releaseTime);
            yield return null;
        }
        PlayerAnimator.SetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerName), 0);
    }
    private void ResetOldAimState()
    {
        PlayerAnimator.SetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerNameOld), 0);
    }

    public void Fire()
    {
        //playerAnimator.SetTrigger("Shoot");
        //gunAnimator.SetTrigger("Shoot");
    }

    public void Hurt()
    {
        StopAllCoroutines();
        PlayerAnimator.SetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerName), 0);
        PlayerAnimator.SetTrigger("Hurt");
    }

    public void Death()
    {
        StopAllCoroutines();
        PlayerAnimator.SetLayerWeight(PlayerAnimator.GetLayerIndex(AimLayerName), 0);
        PlayerAnimator.SetTrigger("Death");
    }
}
