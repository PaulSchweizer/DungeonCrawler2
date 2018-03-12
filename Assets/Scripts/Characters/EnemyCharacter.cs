using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{

    public override void Awake()
    {
        Idle = EnemyIdleState.Instance;
        Chase = EnemyChaseState.Instance;
        base.Awake();
    }

#if UNITY_EDITOR

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Color oldColor = Gizmos.color;
            for (int i = Stats.EnemySet.Items.Count - 1; i >= 0; i--)
            {
                if (Stats.EnemySet.Items[i].EnemiesInAttackShape().Contains(this))
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