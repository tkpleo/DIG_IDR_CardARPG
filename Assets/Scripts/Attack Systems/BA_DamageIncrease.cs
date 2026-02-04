using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "BA_DamageIncrease", menuName = "Bullets/BA_DamageIncrease")]
public class BA_DamageIncrease : BulletAbility
{
    bool isActive;
    int damageIncreaseAmount = 2;
    int ammoCount = 1;
}
