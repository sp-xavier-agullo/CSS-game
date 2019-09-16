using System;
using UnityEngine;

[Serializable]
public class ShipModel
{
    public int Health => _health;
    public int MaxHealth;

    public float HoverForce = 65f;
    public float HoverHeight = 20f;

    public float MaxSpeed => _maxSpeed;
    public float MinSpeed => _minSpeed;

    public float Acceleration;

    public WeaponModel Weapon;

    public float WeaponEnergyMax;
    public float WeaponEnergyRegenerationPerSecond = 4.5f;

    public bool Unlocked;

    public int Cost;

    int _health = 20;

    [SerializeField]
    float _maxSpeed = 100;
    [SerializeField]
    float _minSpeed = 20;

    [SerializeField]
    float _turnSpeed = 0.5f;
    public float TurnSpeed => _turnSpeed;

    float _speed = 50f;
    public float Speed
    {
        get => _speed;
        set
        {
            _speed = Math.Min(value, _maxSpeed);
            _speed = Math.Max(_speed, _minSpeed);
        }
    }

    [SerializeField]
    float _weaponEnergy;

    public float WeaponEnergy
    {
        get => _weaponEnergy;
        set
        {
            _weaponEnergy = Math.Min(value, WeaponEnergyMax);
            _weaponEnergy = Math.Max(_weaponEnergy, 0);
        }
    }

    public bool IsDead => _health <= 0;

    public void DoDamage(int damage)
    {
        _health -= damage;
    }

    public void DoHeal(int healing)
    {
        _health += healing;
        _health = Math.Min(_health, MaxHealth);
    }

    public void Kill()
    {
        _health = 0;
    }
}