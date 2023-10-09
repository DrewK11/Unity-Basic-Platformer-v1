using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    public DetectionZone biteDetectionZone;
    public Collider2D deathCollider;
    public List<Transform> waypoints;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextWaypoint;
    int waypointNum = 0;

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
            // This gets the value from booleans that we set in the animator panel in Unity
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    private void Start() {
        nextWaypoint = waypoints[waypointNum];
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate() {
        if(damageable.IsAlive) {
            if(CanMove) {
                Flight();
            } else {
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void Flight() {
        // Important: Don't apply waypoints to prefab. The prefab doesn't know anything about how the scene is setup
        // Fly to next waypoint
        // A normalized vector is just a direction without speed magnitude, so this is how we just get the direction
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // Check if we have reached the waypoint already
        // This is the magnitude, and the top is the direction
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        // We set velocity here
        // We don't need to add Time.deltaTime here because the rigidbody already factors this in while moving
        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        // See if we need to switch waypoints
        if (distance <= waypointReachedDistance) {
            waypointNum++;

            if (waypointNum >= waypoints.Count) {

                // Loop back to original waypoint
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void UpdateDirection() {
        Vector3 locScale = transform.localScale;

        if(transform.localScale.x > 0) {
            if(rb.velocity.x < 0) {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        } else {
            if (rb.velocity.x > 0) {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    public void OnDeath() {
        // Dead flyer falls to the ground
        rb.gravityScale = 2f;
        rb.velocity = new Vector2(0, rb.velocity.y);
        deathCollider.enabled = true;
    }
}
