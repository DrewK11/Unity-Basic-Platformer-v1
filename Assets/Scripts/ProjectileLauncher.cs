using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectilePrefab;

    // This is a function we use in the Animator events
    // the 2nd parameter is the position of the parent's transform 
    public void FireProjectile() {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 origScale = projectile.transform.localScale;

        // The transform used below belongs to the parent game object
        // Since the ProjectileLauncher script is attached to the Player for the arrow, it will use the Player's transform

        // Flip the projectile's facing direction and movement based on the direction the Player is facing at time of launch
        projectile.transform.localScale = new Vector3(
            origScale.x * transform.localScale.x > 0 ? 1 : -1,
            origScale.y,
            origScale.z
        );
    }
}
