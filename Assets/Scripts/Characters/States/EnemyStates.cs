using UnityEngine;

public class EnemyIdleState : CharacterState
{
    public static EnemyIdleState Instance = new EnemyIdleState();

    public EnemyIdleState()
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

        for (int i = 0; i < character.Stats.EnemySet.Items.Count; i++)
        {
            if (Vector3.Distance(character.transform.position, character.Stats.EnemySet.Items[i].transform.position) < character.Stats.AlertnessRadius)
            {
                // Angle Rotation 
                Vector3 pos = new Vector3(character.transform.position.x, 0, character.transform.position.z);
                Vector3 rotation = Vector3.RotateTowards(character.transform.forward, character.Stats.EnemySet.Items[i].transform.position - pos, 2 * Mathf.PI, 1);
                character.SetDestination(character.Stats.EnemySet.Items[i].transform.position);
                character.NavMeshAgent.SetDestination(character.Stats.EnemySet.Items[i].transform.position);
                character.MarkedEnemy = character.Stats.EnemySet.Items[i];
                character.ChangeState(character.Chase);
                return;
            }
        }
    }

    public override void Exit(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = false;
    }
}


public class EnemyChaseState : CharacterState
{

    public static EnemyChaseState Instance = new EnemyChaseState();

    public EnemyChaseState()
    {
        DebugColor = Color.magenta;
    }

    public override void Enter(BaseCharacter character)
    {
        character.Animator.SetInteger("State", 2);
    }

    public override void Update(BaseCharacter character)
    {
        if (character.EnemiesInAttackShape().Count> 0)
        {
            character.ChangeState(character.Combat);
            return;
        }
        if (character.NavMeshAgent.remainingDistance > character.NavMeshAgent.stoppingDistance)
        {
            for (int i = 0; i < character.Stats.EnemySet.Items.Count; i++)
            {
                if (Vector3.Distance(character.transform.position, character.Stats.EnemySet.Items[i].transform.position) < character.Stats.AlertnessRadius)
                {
                    Vector3 pos = new Vector3(character.transform.position.x, 0, character.transform.position.z);
                    Vector3 rotation = Vector3.RotateTowards(character.transform.forward, character.Stats.EnemySet.Items[i].transform.position - pos, 2 * Mathf.PI, 1);
                    character.SetDestination(character.Stats.EnemySet.Items[i].transform.position);
                    character.NavMeshAgent.SetDestination(character.Stats.EnemySet.Items[i].transform.position);
                    return;
                }
            }
        }
        character.MarkedEnemy = null;
        character.ChangeState(character.Idle);
    }

    public override void Exit(BaseCharacter character)
    {
        character.NavMeshAgent.isStopped = true;
    }
}