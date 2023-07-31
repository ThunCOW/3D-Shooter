using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float DamageReactionTime;
    public AnimationCurve DamageReactionMovementCurve;
    public float HurtResetTimerDefault;

    // ************ Private Fields ************
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    EnemyHealth Health;
    HealthBar HealthBar;
    AnimatorManager animatorManager;

    bool isHurt;
    private Coroutine ReactionRoutine;
    private float hurtResetTimer;
    private bool canRecover;
    private Coroutine RecoveryRoutine;

    void Awake()
    {
        Health = GetComponent<EnemyHealth>();
        HealthBar = GetComponentInChildren<HealthBar>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion= GetComponent<PlayerLocomotion>();
        animatorManager = GetComponent<AnimatorManager>();
    }

    void Start()
    {
        Health.OnTakeDamage += HandlePain;
        Health.OnDeath += Death;
    }

    void Update()
    {
        if (!GameManager.Instance.PauseGame)
        {
            if (!isHurt)
                inputManager.HandleAllInputs();

            inputManager.SelectGunKeyboard();
        }

        if (hurtResetTimer > 0)
            hurtResetTimer -= Time.deltaTime;

        SlowlyIncreaseHealth();
    }

    void FixedUpdate()
    {
        if (!isHurt)
            playerLocomotion.HandleAllMovement();
    }

    // ***************** Player Health And Pain **************************
    private IEnumerator StartRecovery(float Delay)
    {
        yield return new WaitForSeconds(Delay);

        canRecover = true;
    }
    private void SlowlyIncreaseHealth()
    {
        if (GameManager.Instance.PauseGame)
            return;

        if (canRecover)
        {
            if (Health.CurrentHealth == Health.MaxHealth)
            {
                canRecover = false;
            }
            float recoverFor = HealthBar.RecoverHealthPerSecond * Time.deltaTime;
            Health.RecoverHealth(recoverFor);
            HealthBar.UpdateHealthBar(Health.MaxHealth, Health.CurrentHealth);
        }
    }
    private void HandlePain(Vector3 forceDir, float pushDistance)
    {
        if (hurtResetTimer > 0)
            return;

        transform.forward = forceDir * -1;
        animatorManager.Hurt();
        HealthBar.UpdateHealthBar(Health.MaxHealth, Health.CurrentHealth);

        if (ReactionRoutine != null) StopCoroutine(ReactionRoutine);
        StartCoroutine(HandlePainPushRoutine(forceDir, pushDistance));

        canRecover = false;
        if (RecoveryRoutine != null) StopCoroutine(RecoveryRoutine);
        RecoveryRoutine = StartCoroutine(StartRecovery(HealthBar.WaitBeforeStartRecovery));
    }

    private IEnumerator HandlePainPushRoutine(Vector3 forceDir, float pushDistance)             // Direction is normalized
    {
        isHurt = true;

        float time = 0;
        float curveValueDifferenceOld = 0;
        float curveValueDifference = 1;                                                      // Will calculate the value difference between two point in curve to multiply with the pushDistance
        while (time < DamageReactionTime)
        {
            curveValueDifference = DamageReactionMovementCurve.Evaluate(time / DamageReactionTime);
            transform.position += forceDir * pushDistance * (curveValueDifference - curveValueDifferenceOld);
            curveValueDifferenceOld = curveValueDifference;

            time = time + Time.deltaTime > DamageReactionTime ? DamageReactionTime : time + Time.deltaTime;
            yield return null;
        }

        isHurt = false;

        hurtResetTimer = HurtResetTimerDefault;
    }

    private void Death(Vector3 forceDir)
    {
        isHurt = true;
        transform.forward = forceDir * -1;
        HealthBar.UpdateHealthBar(Health.MaxHealth, Health.CurrentHealth);

        StopAllCoroutines();
        animatorManager.Death();
        GameManager.Instance.GameOver();
    }
}