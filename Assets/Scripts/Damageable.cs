using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    // We create a Unity event straight on the damageable component and then other scripts inside of the
    // same game object we drag and dropped on the inspector which function we want to call
    // However wrt the UIManager and the damageable characters exist as 2 completely separate entities so we can't
    // set the UIManager Unity events in the inspector so we need to make a static (global) Unity event
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;

    Animator animator;
    [SerializeField]
    private int _maxHealth = 100;

    public int MaxHealth {
        get {
            return _maxHealth;
        } set {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;

    public int Health {
        get {
            return _health;
        } set {
            _health = value;

            // If health drops below 0, character is no longer alive
            if(_health <= 0) {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    public bool _isAlive = true;

    [SerializeField]
    private bool isInvincible = false;

    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public bool IsAlive {
        get {
            return _isAlive;
        }
        private set {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);

            if(value == false) {
                damageableDeath.Invoke();
            }
        }
    }

    // The velocity shold not be changed while this is true but needs to be respected by other physics components
    // like the player controller
    public bool LockVelocity {
        get {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if(isInvincible) {
            if(timeSinceHit > invincibilityTime) {
                // Remove invincibility
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 knockback) {
        if(IsAlive && !isInvincible) {
            Health -= damage;
            isInvincible = true;

            // Notify other subscribed components that the damageable was hit to handle the knockback and such
            // We don't want to add knocknback here because it's physics related
            // The ? is a null check
            animator.SetTrigger(AnimationStrings.hitTrigger);
            LockVelocity = true;
            
            // This local event is to let other components know for physics reasons
            damageableHit?.Invoke(damage, knockback);

            // Now anything that subscribes to this event can be notified with this information
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);

            return true;
        }

        // Unable to be hit
        return false;
    }

    public bool Heal(int healthRestore) {
        if(IsAlive && Health < MaxHealth) {
            // If the Health is greater than or equal to MaxHealth, then this is going to return a negative value
            // then we will default the heal value to 0 since we shouldn't be able to heal past max health
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;
            CharacterEvents.characterHealed(gameObject, actualHeal);

            return true;
        }

        return false;
    }
}
