using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] Animator gunAnimator;


    void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
    }

    public void UpdateAnimatorValue(float horizontalMovement, float verticalMovement)
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

        playerAnimator.SetFloat("Horizontal", horizontalMovement, 0.1f, Time.deltaTime);
        playerAnimator.SetFloat("Vertical", verticalMovement, 0.1f, Time.deltaTime);
    }

    public void UpdateAimState(bool isAiming)
    {
        StopAllCoroutines();
        if (isAiming)
            StartCoroutine(AimWeightIncrease());
        else
            StartCoroutine(AimWeightDecrease());
    }

    float targetTime = 0.1f;
    float releaseTime = .4f;
    IEnumerator AimWeightIncrease()
    {
        float timePassed = 0;
        while(timePassed < targetTime)
        {
            timePassed += Time.deltaTime;
            
            playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Aim"), playerAnimator.GetLayerWeight(playerAnimator.GetLayerIndex("Aim")) + timePassed / targetTime);
            yield return null;
        }
        playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Aim"), 1);
    }
    IEnumerator AimWeightDecrease()
    {
        float timePassed = releaseTime;
        while (timePassed > 0)
        {
            timePassed -= Time.deltaTime;
            playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Aim"), timePassed / releaseTime);
            yield return null;
        }
        playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("Aim"), 0);
    }

    public void Fire()
    {
        playerAnimator.SetTrigger("Shoot");
        gunAnimator.SetTrigger("Shoot");
    }
}
