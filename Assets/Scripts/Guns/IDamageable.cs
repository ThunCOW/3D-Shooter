using UnityEngine;

public interface IDamageable
{
    public int CurrentHealth { get; }
    public int MaxHealth { get; }

    public delegate void TakeDamageEvent(Vector3 forceDir, float pushDistance);
    public event TakeDamageEvent OnTakeDamage;

    public delegate void DeathEvent(Vector3 forceDir);
    public event DeathEvent OnDeath;

    public void TakeDamage(int Damage, Vector3 forceDir, float pushDistance);
}
