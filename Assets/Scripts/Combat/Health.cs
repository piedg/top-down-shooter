using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int health;
    public int CurrentHealth { get { return health; } }
    private bool isInvulnerable;

    public event Action OnTakeDamage;
    public event Action OnTakeHeal;
    public event Action OnDie;

    public bool IsDead => health == 0;

    private void Start()
    {
        health = maxHealth;
    }

    public void SetInvulnerable(bool isInvulnerable)
    {
        this.isInvulnerable = isInvulnerable;
    }

    public void SetFullHealth()
    {
        health = maxHealth;
    }

    public void Heal(int value)
    {
        health = Mathf.Min(health + value, maxHealth);

        OnTakeHeal?.Invoke();
    }

    public void DealDamage(int damage)
    {
        if (health == 0) { return; }
        if (isInvulnerable) { return; }

        health = Mathf.Max(health - damage, 0);

        OnTakeDamage?.Invoke();

        if (health == 0)
        {
            OnDie?.Invoke();
        }
    }
}
