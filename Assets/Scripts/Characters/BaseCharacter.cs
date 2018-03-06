using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct AttackShapeMarker
{
    public Vector3 Position;
    public int Radius;
    public float Angle;
    public Vector3 Forward;

    public static AttackShapeMarker Default = new AttackShapeMarker(new Vector3(), 2, 45);

    public AttackShapeMarker(Vector3 position, int radius, int angle)
    {
        Position = position;
        Radius = radius;
        Angle = angle;
        Forward = new Vector3(0, 0, 1);
    }

    /// <summary>
    /// Apply shape with the current Transform values.
    /// This occurs when the Attack has been scheduled.
    /// </summary>
    public void Apply(Vector3 forward)
    {
        Forward = forward;
    }

    public float Area()
    {
        return (float)(Math.PI * Radius * Radius * (Angle / 360f));
    }

    public Vector3 StartVector
    {
        get
        {
            float angle = 180 - (Angle * 0.5f) * Mathf.Rad2Deg;
            return Position + Quaternion.AngleAxis(angle, Vector3.up) * Forward;
        }
    }

    public Vector3 EndVector
    {
        get
        {
            float angle = 180 - (Angle * 0.5f) * Mathf.Rad2Deg;
            return Position + Quaternion.AngleAxis(-angle, Vector3.up) * Forward;
        }
    }

    public bool PointInArea(Transform parent, Vector3 point)
    {
        // Check if in range
        float distance = Vector3.Distance(parent.position, point);
        if (distance > Radius)
        {
            return false;
        }

        Apply(parent.forward); // TODO: Maybe remove later

        // Check if in circle sector
        Vector3 vector = (point - parent.position + Position).normalized;
        float angle = Vector3.Angle(Forward, vector);
        if (angle <= Angle * 0.5)
        {
            return true;
        }
        return false;
    }
}

public struct AttackMarker
{
    public BaseCharacter Attacker;
    public AttackShapeMarker[] Shape;
    public Skill Skill;
    public float PreTime;
    public float PostTime;
    public bool HitOccurred;
    public float CurrentTime;
    public bool IsActive;

    public float TotalTime
    {
        get
        {
            return PreTime + PostTime;
        }
    }

    public AttackMarker(BaseCharacter attacker)
    {
        Attacker = attacker;
        Shape = null;
        Skill = null;
        PreTime = 0;
        PostTime = 0;
        CurrentTime = 0;
        IsActive = false;
        HitOccurred = false;
    }

    public void Start(AttackShapeMarker[] shape, Skill skill, float preTime, float postTime)
    {
        Shape = shape;
        Skill = skill;
        PreTime = preTime;
        PostTime = postTime;
        CurrentTime = 0;
        IsActive = true;
        HitOccurred = false;
    }

    public void Hit()
    {
        HitOccurred = true;
        foreach (BaseCharacter enemy in Attacker.EnemiesInAttackShape())
        {
            Attacker.Attack(enemy, Skill);
        }
    }

    public void Stop()
    {
        CurrentTime = 0;
        IsActive = false;
    }

    public float Progress()
    {
        if (CurrentTime <= PreTime)
        {
            return (CurrentTime / PreTime) * 0.5f;
        }
        else
        {
            return 0.5f + ((CurrentTime - PreTime) / PostTime) * 0.5f;
        }
    }
}

public class BaseCharacter : MonoBehaviour
{
    [Header("Components")]
    public UnityEngine.AI.NavMeshAgent NavMeshAgent;
    public Animator Animator;

    [Header("Data")]
    public Stats Stats;
    public Inventory Inventory;

    [Header("States")]
    public CharacterState Idle = IdleState.Instance;
    public CharacterState Move = MoveState.Instance;
    public CharacterState Chase = ChaseState.Instance;
    public CharacterState Combat = CombatState.Instance;
    //public CharacterState TakenOut = TakenOutState.Instance;
    //public CharacterState MoveToNPC = MoveToNPCState.Instance;
    //public CharacterState Conversation = ConversationState.Instance;
    protected CharacterState CurrentState;

    // Logic
    public AttackMarker ScheduledAttack;
    public int Spin;

    // Internals
    [HideInInspector]
    public Vector3 DestinationRotation = new Vector3();
    public static List<BaseCharacter> Characters = new List<BaseCharacter>();

    public virtual void Awake()
    {
        NavMeshAgent.radius = Stats.Radius;
        CurrentState = Idle;
        Characters.Add(this);
        ScheduledAttack = new AttackMarker(this);
    }

    private void Start()
    {
        NavMeshAgent.enabled = true;
        NavMeshAgent.updateRotation = true;
    }

    public virtual void Update()
    {
        CurrentState.Update(this);
    }

    public static List<BaseCharacter> CharactersOfType(string[] types)
    {
        List<BaseCharacter> characters = new List<BaseCharacter>();
        foreach (BaseCharacter character in Characters)
        {
            if (Array.Exists(types, element => element == character.Stats.Type))
            {
                characters.Add(character);
            }
        }
        return characters;
    }

    #region Aspects and Skills

    public int[] SkillValueModifiers(Skill skill, string[] tags)
    {
        List<int> modifiers = new List<int>();
        foreach (Aspect aspect in AspectsAffectingSkill(skill))
        {
            if (aspect.Matches(tags) > 0)
            {
                modifiers.Add(aspect.Bonus);
            }
        }
        return modifiers.ToArray();
    }

    public int SkillValue(Skill skill, string[] tags)
    {
        int value = Stats.SkillValue(skill);
        int[] modifiers = SkillValueModifiers(skill, tags);
        foreach (var item in modifiers)
        {
            value += item;
        }
        return value;
    }

    public Aspect[] AspectsAffectingSkill(Skill skill)
    {
        List<Aspect> aspects = new List<Aspect>();
        foreach (Aspect aspect in AllAspects)
        {
            if (Array.Exists(aspect.Skills, element => element == skill))
            {
                aspects.Add(aspect);
            }
        }
        return aspects.ToArray();
    }

    [JsonIgnore]
    public List<Aspect> AllAspects
    {
        get
        {
            List<Aspect> aspects = new List<Aspect>();

            // Basic Aspects
            if (Stats.Aspects != null)
            {
                foreach (Aspect aspect in Stats.Aspects)
                {
                    aspects.Add(aspect);
                }
            }

            //// Aspects of all taken Consequences
            //foreach (Consequence consequence in AllConsequences)
            //{
            //    if (consequence.IsTaken)
            //    {
            //        aspects.Add(consequence.Effect);
            //    }
            //}

            //// Aspects from the equipped Items
            //foreach (string itemIdentifier in Equipment.Values)
            //{
            //    if (itemIdentifier != null)
            //    {
            //        foreach (Aspect aspect in Inventory.Item(itemIdentifier).Aspects)
            //        {
            //            aspects.Add(aspect);
            //        }
            //    }
            //}

            return aspects;
        }
    }

    #endregion

    #region Combat

    public Skill CurrentAttackSkill
    {
        get
        {
            return Stats.SkillDatabase.SkillByName("MeleeWeapons");
        }
    }

    public int Damage
    {
        get
        {
            int damage = 0;
            //foreach (Weapon weapon in Weapons)
            //{
            //    damage += weapon.Damage;
            //}
            return damage;
        }
    }

    public float AttackSpeed
    {
        get
        {
            float speed = 0;
            //foreach (Weapon weapon in Weapons)
            //{
            //    if (weapon.Speed > speed)
            //    {
            //        speed = weapon.Speed;
            //    }
            //}
            return speed + 1;
        }
    }

    public AttackShapeMarker[] AttackShape
    {
        get
        {
            List<AttackShapeMarker> attackShape = new List<AttackShapeMarker>();
            //
            // TODO: Implement Weapons
            //
            //foreach (Weapon weapon in Weapons)
            //{
            //    foreach (AttackShapeMarker shape in weapon.AttackShape)
            //    {
            //        attackShape.Add(shape);
            //    }
            //}
            if (attackShape.Count == 0)
            {
                attackShape.Add(AttackShapeMarker.Default);
            }
            return attackShape.ToArray();
        }
    }

    public List<BaseCharacter> EnemiesInAttackShape()
    {
        List<BaseCharacter> characters = new List<BaseCharacter>();
        foreach (BaseCharacter enemy in CharactersOfType(Stats.EnemyTypes))
        {
            foreach (AttackShapeMarker shape in AttackShape)
            {
                shape.Apply(transform.forward);
                if (shape.PointInArea(transform, enemy.transform.position))
                {
                    characters.Add(enemy);
                    break;
                }
            }
        }
        return characters;
    }

    public void ScheduleAttack(Skill attackSkill)
    {
        if (ScheduledAttack.IsActive)
        {
            return;
        }
        float speed = 1 / AttackSpeed;
        Debug.Log(attackSkill);
        ScheduledAttack.Start(AttackShape, attackSkill, speed * 0.5f, speed * 0.5f);
        //OnAttackScheduled?.Invoke(this, null);
    }

    public void Attack(BaseCharacter defender, Skill attackSkill)
    {
        Debug.Log(attackSkill);
        //GameEventsLogger.LogSeparator("Attack");
        List<string> tags = new List<string>();
        for (int i = 0; i < defender.Stats.Tags.Count; i++)
        {
            if (!tags.Contains(defender.Stats.Tags[i]))
            {
                tags.Add(defender.Stats.Tags[i]);
            }
        }
        //for (int i = 0; i < GameMaster.CurrentTags.Length; i++)
        //{
        //    if (!tags.Contains(GameMaster.CurrentTags[i]))
        //    {
        //        tags.Add(GameMaster.CurrentTags[i]);
        //    }
        //}
        int skillValue = SkillValue(attackSkill, tags.ToArray());
        int diceValue = Dice.Roll();
        int totalValue = skillValue + diceValue;
        //if (stunt != null)
        //{
        //    totalValue += stunt.Bonus;
        //    GameEventsLogger.LogUsesStunt(this, stunt);
        //}
        if (Spin > 0)
        {
            totalValue += Spin;
            //GameEventsLogger.LogUsesSpin(this, Spin);
            Spin = 0;
        }
        //GameEventsLogger.LogAttack(this, defender, attackSkill, totalValue, skillValue, diceValue);
        int shifts = defender.Defend(this, attackSkill, totalValue);
        if (shifts > 0)
        {
            defender.ReceiveDamage(shifts + Damage);
            //if (defender.IsTakenOut)
            //{
            //    ReceiveXP(defender.Cost);
            //}
        }
    }

    public int Defend(BaseCharacter attacker, Skill attackSkill, int attackValue)
    {
        List<string> tags = new List<string>();
        for (int i = 0; i < attacker.Stats.Tags.Count; i++)
        {
            if (!tags.Contains(attacker.Stats.Tags[i]))
            {
                tags.Add(attacker.Stats.Tags[i]);
            }
        }
        //for (int i = 0; i < GameMaster.CurrentTags.Length; i++)
        //{
        //    if (!tags.Contains(GameMaster.CurrentTags[i]))
        //    {
        //        tags.Add(GameMaster.CurrentTags[i]);
        //    }
        //}

        // Get the best defend skill
        int defendValue = 0;
        Skill defendSkill = null;
        foreach (Skill skill in attackSkill.OpposingSkills)
        {
            int skillValue = SkillValue(skill, tags.ToArray());
            if (skillValue >= defendValue)
            {
                defendValue = skillValue;
                defendSkill = skill;
            }
        }
        int diceValue = Dice.Roll();
        int totalDefendValue = defendValue + diceValue;
        int shifts = attackValue - totalDefendValue;
        //GameEventsLogger.LogDefend(attacker, this, defendSkill, totalDefendValue, defendValue, diceValue);
        if (shifts < -1)
        {
            int spin = shifts / -2;
            Spin += spin;
            //GameEventsLogger.LogGainsSpin(this, spin);
        }

        return shifts;
    }

    [JsonIgnore]
    public int Protection
    {
        get
        {
            int protection = 0;
            //foreach (string itemName in Equipment.Values)
            //{
            //    Item item = Inventory.Item(itemName);
            //    if (item is Armour)
            //    {
            //        Armour armour = item as Armour;
            //        protection += armour.Protection;
            //    }
            //}
            return protection;
        }
    }

    public void ReceiveDamage(int damage)
    {
        damage = Math.Max(damage - Protection, 0);
        Stats.Health.Value -= damage;
        //OnPhysicalStressChanged?.Invoke(this, null);
    }

    #endregion

    #region State

    public void ChangeState(CharacterState state)
    {
        if (state != CurrentState)
        {
            CurrentState.Exit(this);
            CurrentState = state;
            CurrentState.Enter(this);
        }
    }

    public void SetDestination(Vector3 position)
    {
        NavMeshAgent.SetDestination(position);
        Vector3 forward = new Vector3(transform.forward.x, position.y, transform.forward.z);
        Vector3 characterPos = new Vector3(transform.position.x, position.y, transform.position.z);
        Vector3 rotation = Vector3.RotateTowards(forward, position - characterPos, 2 * Mathf.PI, 1);
        DestinationRotation.Set(rotation.x, rotation.y, rotation.z);

    }

    #endregion

    #region Serialization

    public Dictionary<string, object> SerializeToData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["Stats"] = Stats.SerializeToData();
        data["Inventory"] = Inventory.SerializeToData();
        data["Transform"] = new float[] { gameObject.transform.position.x,
                                          gameObject.transform.position.y,
                                          gameObject.transform.position.z };
        return data;
    }

    public void DeserializeFromJson(string json)
    {
        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        Stats.DeserializeFromData(SerializationUtilitites.DeserializeFromObject(data["Stats"]));
        Inventory.DeserializeFromData(SerializationUtilitites.DeserializeFromObject(data["Inventory"]));

        float[] transformData = JsonConvert.DeserializeObject<float[]>(JsonConvert.SerializeObject(data["Transform"]));
        transform.position = new Vector3(transformData[0], transformData[1], transformData[2]);
    }

    #endregion

    #region Debug

    #if UNITY_EDITOR
    [Header("Debug")]
    public Color DebugColor;

    public virtual void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Color oldColor = Gizmos.color;

            // Position
            Gizmos.color = DebugColor;
            Vector3 center = transform.position;
            Vector3 size = new Vector3(1, 0.01f, 1);
            Gizmos.DrawCube(center, size);

            // AttackShape
            Gizmos.color = Color.red;
            foreach (AttackShapeMarker shape in AttackShape)
            {
                shape.Apply(transform.forward);
                center = transform.position + transform.TransformVector(shape.Position);
                size = new Vector3(1, 0.01f, 1);
                DebugExtension.DebugCircle(center, Gizmos.color, shape.Radius);

                float angle = shape.Angle * Mathf.Rad2Deg;
                float rotation = (Mathf.PI / 180f) * (transform.eulerAngles.y - 90);
                Vector3 to = Quaternion.AngleAxis(angle / 2 + rotation * Mathf.Rad2Deg, Vector3.up) * Vector3.left;
                //Gizmos.DrawLine(center, center + to * shape.Radius);
                Gizmos.DrawLine(center, center + shape.StartVector * shape.Radius);

                to = Quaternion.AngleAxis(-angle / 2 + rotation * Mathf.Rad2Deg, Vector3.up) * Vector3.left;
                //Gizmos.DrawLine(center, center + to * shape.Radius);
                Gizmos.DrawLine(center, center + shape.EndVector * shape.Radius);

                // Forward
                Gizmos.DrawLine(center, center + shape.Forward * shape.Radius);
            }

            // AlertnessRadius
            DebugExtension.DebugCircle(transform.position, Gizmos.color, Stats.AlertnessRadius);

            // States
            DebugExtension.DebugCircle(transform.position, CurrentState.DebugColor, NavMeshAgent.radius);

            Gizmos.color = oldColor;
        }
    }
    #endif

    #endregion
}

