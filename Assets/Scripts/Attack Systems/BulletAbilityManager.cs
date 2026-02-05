using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletAbilityManager : MonoBehaviour
{
    public static BulletAbilityManager Instance { get; private set; }
    //public List<BulletAbility> bulletAbilities;
    //public List<BulletAbility> activeAbilities;
    
    public bool BA_DamageIncrease = false;
    public bool BA_DamageMultiplier = false;
    public bool BA_Stun = false;
    public bool BA_AOE = false;
    public bool BA_Slow = false;

    private int bulletsToFire = 1;
    private int bulletDamage;
    private float bulletSpeed;

    public void BulletBuilder(Bullet bullet)
    {
        bulletDamage = 1;
        bulletSpeed = 10f;
        if (BA_DamageIncrease == true)
        {
            bulletDamage += 1;
        }
        if (BA_DamageMultiplier == true)
        {
            bulletDamage *= 2;
        }
        if (BA_Stun == true)
        {
            bullet.isStunBullet = true;
        }
        if (BA_AOE == true)
        {
            bullet.isAOEBullet = true;
            bullet.AOERadius = 2f;
        }
        if (BA_Slow == true)
        {
            bullet.isSlowBullet = true;
        }
        
        bullet.bulletSpeed = bulletSpeed;
        bullet.bulletDamage = bulletDamage;

        BA_DamageIncrease = false;
        BA_DamageMultiplier = false;
        BA_Stun = false;
        BA_AOE = false;
        BA_Slow = false;
}
}
