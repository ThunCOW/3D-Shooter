using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _MaxHealth = 100;
    [SerializeField]
    private float _Health;

    public float CurrentHealth { get => _Health; private set => _Health = value; }

    public float MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int Damage, Vector3 forceDir, float pushDistance)
    {
        float damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);
        //CurrentHealth -= damageTaken;
        float newHealth = Mathf.Clamp(CurrentHealth - Damage, 0, CurrentHealth);

        CurrentHealth = newHealth;

        if (CurrentHealth == 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(forceDir);
            return;
        }

        // Shooting somebody who is already dead
        if(CurrentHealth != 0)
        {
            OnTakeDamage?.Invoke(forceDir, pushDistance);
        }
    }

    public void RecoverHealth(float Recover)
    {
        float newHealth = Mathf.Clamp(CurrentHealth + Recover, 0, MaxHealth);
        CurrentHealth = newHealth;
    }
}
