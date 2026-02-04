using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletAbilitytManager : MonoBehaviour
{
    public static BulletAbilitytManager Instance { get; private set; }
    public List<BulletAbility> bulletAbilities;
    public List<BulletAbility> activeAbilities;

    private int bulletsToFire = 1;
}
