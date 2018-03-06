using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    public override void Awake()
    {
        //Idle = EnemyIdleState.Instance;
        //Chase = EnemyChaseState.Instance;
        base.Awake();
    }

#if UNITY_EDITOR

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Color oldColor = Gizmos.color;

            foreach(BaseCharacter player in BaseCharacter.CharactersOfType(Stats.EnemyTypes))
            {
                if (player.EnemiesInAttackShape().Contains(this))
                {
                    Gizmos.color = DebugColor;
                    Gizmos.DrawSphere(transform.position, 1);
                    break;
                }
            }
            Gizmos.color = oldColor;
        }
    }
#endif
}