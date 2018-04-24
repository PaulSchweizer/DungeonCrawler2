using UnityEngine;

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

    private float RotationThreshold = 1;

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

        if (character.EnemiesInAttackShape().Contains(character.MarkedEnemy))
        {
            character.ChangeState(character.Combat);
            return;
        }

        float stoppingDistance = character.NavMeshAgent.stoppingDistance + character.Stats.Radius * 2;
        if (character.NavMeshAgent.remainingDistance <= stoppingDistance)
        {
            if (Vector3.Angle(character.transform.forward, character.DestinationRotation) < RotationThreshold)
            {
                character.MarkedEnemy = null;
                character.ChangeState(character.Idle);
            }
            else
            {
                character.transform.LookAt(character.NavMeshAgent.destination);
                // float speed = (character.NavMeshAgent.angularSpeed * Mathf.PI) / 180;
                //Vector3 to = character.NavMeshAgent.destination - character.transform.position;
                //Quaternion _lookRotation = Quaternion.LookRotation(to);
                //character.transform.rotation = Quaternion.Slerp(character.transform.rotation, _lookRotation, speed * Time.deltaTime);
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
        character.Animator.SetFloat("Speed", character.AttackSpeed);
    }

    public override void Update(BaseCharacter character)
    {
        if (character.ScheduledAttack.IsActive)
        {
            character.ScheduledAttack.CurrentTime += Time.deltaTime;
            DebugColor = Color.red;
            if (character.ScheduledAttack.Progress() >= 0.5 && !character.ScheduledAttack.HitOccurred)
            {
                character.ScheduledAttack.Hit();
                DebugColor = Color.yellow;
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
        character.MarkedEnemy = null;
    }
}

public class TakenOutState : CharacterState
{
    public static TakenOutState Instance = new TakenOutState();

    public TakenOutState()
    {
        DebugColor = Color.grey;
    }

    public override void Enter(BaseCharacter character)
    {
        character.Animator.SetInteger("State", 4);
        character.NavMeshAgent.enabled = false;
        //character.DropLoot();
        character.Stats.CharacterSet.Remove(character);
        character.tag = "Untagged";
    }

    public override void Update(BaseCharacter character) { }

    public override void Exit(BaseCharacter character) { }
}

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