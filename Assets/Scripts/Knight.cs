using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Knight : MonoBehaviour
{
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection {
        get {
            return _walkDirection;
        } set {
            if (_walkDirection != value) {
                // Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right ) {
                    walkDirectionVector = Vector2.right;
                } else if(value == WalkableDirection.Left ) {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget {
        get {
            return _hasTarget;
        }
        private set {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove {
        get {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown {
        get {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }


    // We use update for functions that aren't a physics function
    void Update() {
        HasTarget = attackZone.detectedColliders.Count > 0;

        if(AttackCooldown > 0) {
            AttackCooldown -= Time.deltaTime;
        }
    }

    // This function runs independently of system frame time, so put physics functions here
    private void FixedUpdate() {
        if(touchingDirections.IsGrounded && touchingDirections.IsOnWall) {
            FlipDirection();
        }

        if(!damageable.LockVelocity) {
            if (CanMove && touchingDirections.IsGrounded) {
                // Accelerate towards max Speed
                // Clamp gets a value between 2 values
                // the Time.fixedDeltaTime so our increase in speed scales with the number of time btw frames

                // We start with current x velocity
                // Then we increase it based on whether we're facing left or right
                // As we increase the value we limit it with the Clamp value to the negative max speed or the max speed
                // the negative max speed is just the max speed but in the left direction
                // Now we have a limit on how fast we can move to the left and also to the right
                // we leave the rb.velocity.y alone cause that's just based on gravity
                rb.velocity = new Vector2(
                    Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
            }
            else {
                // The x velocity is dropped toward 0
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
            }
        }
    }

    private void FlipDirection() {
        if(WalkDirection == WalkableDirection.Right ) {
            WalkDirection = WalkableDirection.Left;
        } else if(WalkDirection == WalkableDirection.Left) {
            WalkDirection = WalkableDirection.Right;
        } else {
            Debug.LogError("Current walkable direction is not set to legal values of right or left");
        }
    }

    public void OnHit(int damage, Vector2 knockback) {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnCliffDetected() {
        if(touchingDirections.IsGrounded) {
            FlipDirection();
        }
    }
}
