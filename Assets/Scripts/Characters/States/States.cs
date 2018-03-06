﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterState
{
    public Color DebugColor = new Color(1, 1, 1, 1);
    public virtual void Enter(BaseCharacter character) { }
    public abstract void Update(BaseCharacter character);
    public virtual void Exit(BaseCharacter character) { }

    public void SetSpeed(BaseCharacter character)
    {
        character.Animator.SetFloat(
            "Speed", Mathf.Clamp(character.NavMeshAgent.velocity.magnitude / character.NavMeshAgent.speed, 0.5f, 1));
    }
}

public class IdleState : CharacterState
{
    public static IdleState Instance = new IdleState();

    public IdleState()
    {
        DebugColor = Color.green;
    }

    public override void Enter(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = true;
        character.Animator.SetInteger("State", 0);
    }

    public override void Update(BaseCharacter character)
    {
    }

    public override void Exit(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = false;
    }
}

public class MoveState : CharacterState
{
    public static MoveState Instance = new MoveState();

    private static float RotationThreshold = 5;

    public MoveState()
    {
        DebugColor = Color.blue;
    }

    public override void Enter(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = false;
        character.Animator.SetInteger("State", 1);
    }

    public override void Update(BaseCharacter character)
    {
        SetSpeed(character);
        if (character.NavMeshAgent.remainingDistance <= character.NavMeshAgent.stoppingDistance)
        {
            //if ((!character.NavMeshAgent.hasPath || Mathf.Abs(character.NavMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
            //    && Vector3.Angle(character.transform.forward, character.DestinationRotation) <= RotationThreshold)
            if (Vector3.Angle(character.transform.forward, character.DestinationRotation) <= RotationThreshold)
            {
                character.ChangeState(character.Idle);
            }
            else
            {
                float step = ((character.NavMeshAgent.angularSpeed / 2) * Time.deltaTime * Mathf.PI) / 180;
                Vector3 forward = new Vector3(character.transform.forward.x, character.DestinationRotation.y, character.transform.forward.z);
                Vector3 newDir = Vector3.RotateTowards(forward, character.DestinationRotation, step, 0);
                character.transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
    }

    public override void Exit(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = false;
    }
}

public class ChaseState : CharacterState
{

    private float RotationThreshold = 5;

    public static ChaseState Instance = new ChaseState();

    public ChaseState()
    {
        DebugColor = Color.magenta;
    }

    public override void Enter(BaseCharacter character)
    {
        character.NavMeshAgent.enabled = true;
        character.Animator.SetInteger("State", 2);
    }

    public override void Update(BaseCharacter character)
    {
        SetSpeed(character);
        if (character.NavMeshAgent.remainingDistance <= character.NavMeshAgent.stoppingDistance + character.Stats.Radius * 2)
        {
            if (Vector3.Angle(character.transform.forward, character.DestinationRotation) < RotationThreshold)
            {
                if (character.EnemiesInAttackShape().Count > 0)
                {
                    character.ChangeState(character.Combat);
                    return;
                }
                character.ChangeState(character.Idle);
            }
            else
            {
                float step = (character.NavMeshAgent.angularSpeed * Time.deltaTime * Mathf.PI) / 180;
                Vector3 forward = new Vector3(character.transform.forward.x, character.DestinationRotation.y, character.transform.forward.z);
                Vector3 newDir = Vector3.RotateTowards(forward, character.DestinationRotation, step, 0);
                character.transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
    }

    public override void Exit(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = true;
    }
}

public class CombatState : CharacterState
{
    public static readonly CombatState Instance = new CombatState();

    public CombatState()
    {
        DebugColor = Color.red;
    }

    public override void Enter(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = true;
        character.Animator.SetInteger("State", 3);
    }

    public override void Update(BaseCharacter character)
    {
        if (character.ScheduledAttack.IsActive)
        {
            character.ScheduledAttack.CurrentTime += Time.deltaTime;
            //character.AttackSlider.value = character.Data.ScheduledAttack.CurrentTime;
            if (character.ScheduledAttack.Progress() >= 0.5 && !character.ScheduledAttack.HitOccurred)
            {
                character.ScheduledAttack.Hit();
                //character.AttackSlider.fillRect.GetComponent<Image>().color = new Color(0f, 0f, 1f);
            }
            else if (character.ScheduledAttack.Progress() >= 1)
            {
                character.ChangeState(character.Idle);
            }
        }
        else
        {
            character.ScheduleAttack(character.CurrentAttackSkill);
        }
    }

    public override void Exit(BaseCharacter character)
    {
        character.ScheduledAttack.Stop();
        //character.AttackSlider.value = 0;
    }
}

//public class TakenOutState : CharacterState
//{
//    public static TakenOutState Instance = new TakenOutState();

//    public TakenOutState()
//    {
//        DebugColor = Color.grey;
//    }

//    public override void Enter(BaseCharacter character)
//    {
//        character.NavMeshAgent.enabled = false;
//        character.DropLoot();
//        GameMaster.DeRegisterCharacter(character.Data);
//        character.tag = "Untagged";
//    }

//    public override void Update(BaseCharacter character) { }

//    public override void Exit(BaseCharacter character) { }
//}

//public class MoveToNPCState : CharacterState
//{
//    public static MoveToNPCState Instance = new MoveToNPCState();

//    public MoveToNPCState()
//    {
//        DebugColor = Color.blue;
//    }

//    public override void Enter(BaseCharacter character)
//    {
//        character.NavMeshAgent.isStopped = false;
//    }

//    public override void Update(BaseCharacter character)
//    {
//        if (character.NavMeshAgent.pathPending)
//        {
//            return;
//        }

//        if (character.NavMeshAgent.remainingDistance <= character.NavMeshAgent.stoppingDistance)
//        {
//            if (Vector3.Angle(character.transform.forward, character.DestinationRotation) < 0.1)
//            {
//                character.ChangeState(character.Conversation);
//            }
//            else
//            {
//                Vector3 pos = new Vector3(character.transform.position.x, 0, character.transform.position.z);
//                Vector3 rotation = Vector3.RotateTowards(character.transform.forward, character.DestinationPosition - pos, 2 * Mathf.PI, 1);
//                character.SetDestination(character.DestinationPosition, rotation);

//                float step = (float)((character.NavMeshAgent.angularSpeed * Time.deltaTime * Math.PI) / 180);
//                Vector3 newDir = Vector3.RotateTowards(character.transform.forward, character.DestinationRotation, step, 0);
//                character.transform.rotation = Quaternion.LookRotation(newDir);
//            }
//        }
//    }

//    public override void Exit(BaseCharacter character)
//    {
//        character.NavMeshAgent.isStopped = false;
//    }
//}

//public class ConversationState : CharacterState
//{
//    public static ConversationState Instance = new ConversationState();

//    public ConversationState()
//    {
//        DebugColor = Color.white;
//    }

//    public override void Enter(BaseCharacter character)
//    {
//        character.NavMeshAgent.isStopped = true;
//        if (!ConversationUI.Instance.gameObject.activeSelf)
//        {
//            ConversationUI.Instance.Open(character.DestinationNPC.InkStory, character.DestinationNPC);
//        }
//    }

//    public override void Update(BaseCharacter character)
//    {
//    }

//    public override void Exit(BaseCharacter character)
//    {
//        character.DestinationNPC = null;
//    }
//}