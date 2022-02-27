using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownBehavior : EnemyController
{
    public Animator animator;
    public readonly int isRunning = Animator.StringToHash("isRunning");

    public void Update()
    {
        SetAgents();
        RotateTowardsTarget();
        if (agent.remainingDistance <= 10.0f)
        {
            agent.isStopped = true;
            animator.SetBool(isRunning, false);
        }
        else if (inBoundingBox())
        {
            agent.isStopped = false;
            animator.SetBool(isRunning, true);
        }
    }

}
