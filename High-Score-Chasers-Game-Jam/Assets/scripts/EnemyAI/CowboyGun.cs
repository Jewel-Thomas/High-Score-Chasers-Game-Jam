using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowboyGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzle;

    public void Fire(Vector3 targetPosition)
    {
        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzle.position,
            muzzle.rotation);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        Vector3 direction =
            (targetPosition - muzzle.position).normalized;

        bulletScript.Fire(direction);
    }
}
