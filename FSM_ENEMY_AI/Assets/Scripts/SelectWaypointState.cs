using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectWaypointState : StateMachineBehaviour {


    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        EnemyAI enemyAI = animator.gameObject.GetComponent<EnemyAI>();
        enemyAI.SetNextPoint();
        
    }
    
}
