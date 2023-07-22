using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int _MaxHealth = 100;
    [SerializeField]
    private int _Health;

    public int CurrentHealth { get => _Health; private set => _Health = value; }

    public int MaxHealth { get => _MaxHealth; private set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int Damage, Vector3 forceDir, float pushDistance)
    {
        int damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);

        CurrentHealth -= damageTaken;

        // Shooting somebody who is already dead
        if(damageTaken != 0)
        {
            OnTakeDamage?.Invoke(forceDir, pushDistance);
        }

        // hp is 0 but dmg taken is not, As long as we took damage, then we are dead
        if(CurrentHealth == 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(forceDir);
        }
    }
}
