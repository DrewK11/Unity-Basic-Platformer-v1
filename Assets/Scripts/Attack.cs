using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision) {
        // See if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null) {
            Vector2 deliverKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            // Hit the target
            // Invincibility is checked in the Hit method, so no need to check it here
            bool gotHit = damageable.Hit(attackDamage, deliverKnockback);

            if (gotHit) {
                Debug.Log(collision.name + " hit for " + attackDamage);
            }
        }
    }
}
