using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Melee,
    Projectile
}
public class EnemyAnimationEvents : MonoBehaviour
{
    public delegate void OnDamage(AttackType AttackType);
    public event OnDamage Damage;

    public void DamageEvent(AttackType AttackType)
    {
        Damage?.Invoke(AttackType);
    }
}
