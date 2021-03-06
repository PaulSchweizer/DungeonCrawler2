﻿using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public CharacterSet PlayerCharacters;

    private bool _pointerIsDown;

    public void OnPointerDown(PointerEventData data)
    {
        _pointerIsDown = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        _pointerIsDown = false;
    }

    void Update()
    {
        if (_pointerIsDown)
        {
            HandleInput();
        }
    }

    public void HandleInput()
    {
        RaycastHit hit = HitFromInput();
        if (hit.collider == null)
        {
            return;
        }
        else if (hit.collider.CompareTag("Navigation"))
        {
            HitNavigation(hit);
        }
        else if (hit.collider.CompareTag("Enemy"))
        {
            HitEnemy(hit);
        }
        //else if (hit.collider.CompareTag("NPC"))
        //{
        //    HitNPC(hit);
        //}
    }

    private RaycastHit HitFromInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Navigation", "Enemies")); // , "Default"));
        return hit;
    }

    private void HitNavigation(RaycastHit hit)
    {
        for (int i = 0; i < PlayerCharacters.Items.Count; i++)
        {
            ApplyCharacterDestinations(PlayerCharacters.Items[i], hit.point);
            PlayerCharacters.Items[i].ChangeState(PlayerCharacters.Items[i].Move);
        }
    }

    private void HitEnemy(RaycastHit hit)
    {
        BaseCharacter enemy = hit.transform.GetComponent<BaseCharacter>();
        Vector3 point = enemy.transform.position;
        for (int i = 0; i < PlayerCharacters.Items.Count; i++)
        {
            if (!PlayerCharacters.Items[i].ScheduledAttack.IsActive)
            {
                PlayerCharacters.Items[i].MarkedEnemy = enemy;
                Vector3 from_pc_to_enemy = enemy.transform.position - PlayerCharacters.Items[i].transform.position;
                float mag = from_pc_to_enemy.magnitude;
                from_pc_to_enemy.Normalize();
                from_pc_to_enemy = from_pc_to_enemy * (mag - PlayerCharacters.Items[i].NavMeshAgent.radius - enemy.NavMeshAgent.radius);
                ApplyCharacterDestinations(PlayerCharacters.Items[i], point);
                if (PlayerCharacters.Items[i].EnemiesInAttackShape().Contains(enemy))
                {
                    PlayerCharacters.Items[i].ChangeState(PlayerCharacters.Items[i].Combat);
                }
                else
                {
                    PlayerCharacters.Items[i].ChangeState(PlayerCharacters.Items[i].Chase);
                }
            }
        }
    }

    private void HitNPC(RaycastHit hit)
    {
        //ApplyCharacterDestinations(Tabletop.PlayerParty, hit.collider.transform.position);
        //for (int i = 0; i < Tabletop.PlayerParty.Length; i++)
        //{
        //    PlayerCharacter pc = Tabletop.PlayerParty[i];
        //    NPCCharacter destinationNPC = hit.collider.gameObject.GetComponent<NPCCharacter>();
        //    pc.DestinationNPC = destinationNPC;
        //    pc.ChangeState(pc.MoveToNPC);
        //}
    }

    private void ApplyCharacterDestinations(BaseCharacter character, Vector3 position)
    {
        if (!character.NavMeshAgent.isOnNavMesh)
        {
            return;
        }
        character.SetDestination(position);
    }
}