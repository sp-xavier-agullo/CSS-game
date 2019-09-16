using System;

[Serializable]
public class WeaponModel
{
    public enum WeaponType
    {
        PulseLaser,
        Total
    }

    public WeaponType WeaponKind;

    public float FireRate;
    public float EnergyPerProjectile = 5;

    public ProjectileModel Projectile;
}

[Serializable]
public class ProjectileModel
{
    public enum ProjectileType
    {
        Laser,
        Total
    }

    public ProjectileType ProjectileKind;

    public int Damage;
    public float Speed = 1000f;
}