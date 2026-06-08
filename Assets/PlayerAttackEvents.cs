using UnityEngine;

public class PlayerAttackEvents : MonoBehaviour
{
    public WeaponHitbox weaponHitbox;

    public void EnableHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.EnableHitbox();
    }

    public void DisableHitbox()
    {
        if (weaponHitbox != null)
            weaponHitbox.DisableHitbox();
    }
}
