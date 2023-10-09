using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    public float fadeDelay = 0.0f;
    private float timeElapsed = 0f;
    private float fadeDelayElapsed = 0f;
    SpriteRenderer spriteRenderer;
    GameObject objToRemove;
    Color startColor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        objToRemove = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(fadeDelay > fadeDelayElapsed) {
            fadeDelayElapsed += Time.deltaTime;
        } else {
            // They normal fade will only occur after the fade delay time that we set in the Inspector
            timeElapsed += Time.deltaTime;

            // As time elapsed gets more the timeElapsed / fadeTime part becomes higher, and the new alpha value gets smaller
            // At an alpha value of 1 nothing changes. As it gets lesser it becomes more transparent
            // then the newAlpha is startColor times smaller parts (2nd part becomes smaller) so it will be less
            float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime));

            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            if (timeElapsed > fadeTime) {
                Destroy(objToRemove);
            }
        } 
    }
}
