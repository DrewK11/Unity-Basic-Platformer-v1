using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthRestore = 20;
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Damageable damageable = collision.GetComponent<Damageable>();

        // The HealthPickup detects that a damageable character walked into its box collider zone
        // Then we call the heal function on the damageable component
        // When the damageable component gets healed we invoke the event CharacterEvenrs.characterHealed event
        // which means that any other component which has a function subscribed to the characterHealed event
        // are going to run that function with gameObject and healthRestore as their parameters (see Damageable)
        if(damageable) {
            bool wasHealed = damageable.Heal(healthRestore);

            if (wasHealed) {
                Destroy(gameObject);
            }
        }
    }

    // We want to apply the spinRotationSpeed * time.deltaTime and apply that to the rotation of our transform
    private void Update() {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}
