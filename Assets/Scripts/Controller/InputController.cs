using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    public PlayerCharacter[] PlayerCharacters;

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
        for (int i = 0; i < PlayerCharacters.Length; i++)
        {
            ApplyCharacterDestinations(PlayerCharacters[i], hit.point);
            PlayerCharacters[i].ChangeState(PlayerCharacters[i].Move);
        }
    }

    private void HitEnemy(RaycastHit hit)
    {
        BaseCharacter enemy = hit.transform.GetComponent<BaseCharacter>();
        //Vector3 point = enemy.transform.position;
        for (int i = 0; i < PlayerCharacters.Length; i++)
        {
            if (!PlayerCharacters[i].ScheduledAttack.IsActive)
            {
                Vector3 from_pc_to_enemy = enemy.transform.position - PlayerCharacters[i].transform.position;
                float mag = from_pc_to_enemy.magnitude;
                from_pc_to_enemy.Normalize();
                from_pc_to_enemy = from_pc_to_enemy * (mag - PlayerCharacters[i].NavMeshAgent.radius - enemy.NavMeshAgent.radius);
                Vector3 point = PlayerCharacters[i].transform.position + from_pc_to_enemy;

                ApplyCharacterDestinations(PlayerCharacters[i], point);
                PlayerCharacters[i].ChangeState(PlayerCharacters[i].Chase);
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

    private void ApplyCharacterDestinations(PlayerCharacter character, Vector3 position)
    {
        if (!character.NavMeshAgent.isOnNavMesh)
        {
            return;
        }
        character.SetDestination(position);
    }
}