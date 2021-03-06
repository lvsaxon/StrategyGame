﻿using UnityEngine;


public class BehaviorAnimationStates: StateMachineBehaviour {

    Soldier_FieldOfView fieldView;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	/*override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    
    }*/

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        fieldView = animator.GetComponent<Soldier_FieldOfView>();
        
        //Shooting Animation
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("soldierReadyToFire")){
           fieldView.ShootTarget();                                                                  
        }

        //Sprinting Animation
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("soldierSprint")){
           fieldView.SearchTarget();                                                                
        }
        
        //Idle Animation
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("soldierIdle")){
           fieldView.StandBy();
        }
        
        //Dead Animation
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("soldierDieFront") || animator.GetCurrentAnimatorStateInfo(0).IsName("soldierDieBack")){
           fieldView.SuspendAllBehaviors();
        } 
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	/*override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    
	}*/
}
