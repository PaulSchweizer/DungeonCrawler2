using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;

[Serializable]
public class LogMessage
{
    public string Name = "";
    public string Template = "";
    public bool Enabled = true;
    public bool Analytics = false;
}

[CreateAssetMenu(fileName = "GameEventsLogger", menuName = "DungeonCrawler/GameEventsLogger")]
public class GameEventsLogger : ScriptableObject
{
    public bool ShowLogs = true;
    public bool SendAnalytics = false;

    public enum LogFormat { terminal = 0, markup = 1 };

    public static LogFormat Format = LogFormat.terminal;

    private List<string> Log = new List<string>();

    public LogMessage[] LogMessages;

    private LogMessage _defaultLogMessage = new LogMessage();

    //public string ReceivePhysicalStressTemplate = "\u2661 __{0}__ receives {1} _PhysicalStress_";
    //public string TakeConsequenceTemplate = "\u2661 __{0}__ takes _Consequence_ \"{1}\" {2}";
    //public string GetsTakenOutTemplate = "\u271D __{0}__ is _taken out_";
    //public string UsesStuntTemplate = "\u272A __{0}__ uses __Stunt__ _{1}_ ({2})";
    //public string ReceivesXPTemplate = "\u2795 __{0}__ receives {1} __XP__";
    //public string ReachesNextLevelTemplate = "\u2191 __{0}__ reaches __Level__ {1}";
    //public string HealingSuccessTemplate = "__{0}__ _heals_ __{1}__ of _{2}_ with {3} ({4}+D{5})";
    //public string HealingFailTemplate = "__{0}__ fails at _healing_ __{1}__ of _{2}_ with {3} ({4}+D{5})";

    private LogMessage LogMessageByName(string name)
    {
        if (LogMessages == null) return _defaultLogMessage;
        for (int i = 0; i < LogMessages.Length; i++)
        {
            if (LogMessages[i].Name == name)
            {
                return LogMessages[i];
            }
        }
        return _defaultLogMessage;
    }

    private void AddLog(string log, LogMessage logMessage)
    {
        if (Format == LogFormat.terminal)
        {
            log = log.Replace("_", "").Replace("#", "");
        }
        Log.Add(log);
        if (log != "" && logMessage.Enabled && ShowLogs)
        {
            Debug.Log(log);
        }
        if (logMessage.Analytics && SendAnalytics)
        {
            Analytics.CustomEvent(name, new Dictionary<string, object> { { "details", log } });
        }
    }

    //public static void Reset()
    //{
    //    Log = new List<string>();
    //    index = 0;
    //}

    public void LogSeparator(string title = "")
    {
        LogMessage logMessage = LogMessageByName("Separator");
        AddLog(string.Format(logMessage.Template, title), logMessage);
    }

    public void LogAttack(BaseCharacter attacker, BaseCharacter defender,
        Skill skill, int totalValue, int skillValue, int diceValue)
    {
        LogMessage logMessage = LogMessageByName("Attack");
        AddLog(string.Format(logMessage.Template, attacker.Stats.Name, defender.Stats.Name,
                             skill.Name, totalValue, skillValue, diceValue), logMessage);
    }

    public void LogDefend(BaseCharacter attacker, BaseCharacter defender,
        Skill skill, int totalDefendValue, int defendValue, int diceValue)
    {
        LogMessage logMessage = LogMessageByName("Defend");
        AddLog(string.Format(logMessage.Template, defender.Stats.Name, attacker.Stats.Name,
                             skill.Name, totalDefendValue, defendValue, diceValue), logMessage);
    }

    //public static void LogReceivePhysicalStress(Character.Character character, int damage)
    //{
    //    AddLog(string.Format(ReceivePhysicalStressTemplate, character.Name, damage), "ReceivePhysicalStress");
    //}

    //public static void LogTakeConsequence(Character.Character character, Consequence consequence)
    //{
    //    AddLog(string.Format(TakeConsequenceTemplate, character.Name, consequence.Name, consequence.Capacity), "TakePhysicalStress");
    //}

    //public static void LogGetsTakenOut(Character.Character character)
    //{
    //    AddLog(string.Format(GetsTakenOutTemplate, character.Name), "TakenOut");
    //}

    public  void LogGainsSpin(BaseCharacter character, int spin)
    {
        LogMessage logMessage = LogMessageByName("GainsSpin");
        AddLog(string.Format(logMessage.Template, character.Stats.Name, spin), logMessage);
    }

    //public static void LogUsesStunt(Character.Character character, Stunt stunt)
    //{
    //    AddLog(string.Format(UsesStuntTemplate, character.Name, stunt.Name, stunt.Bonus), "UsesStunt");
    //}

    public void LogUsesSpin(BaseCharacter character, int spin)
    {
        LogMessage logMessage = LogMessageByName("UsesSpin");
        AddLog(string.Format(logMessage.Template, character.Stats.Name, spin), logMessage);
    }

    //public static void LogReceivesXP(Character.Character character, int xp)
    //{
    //    AddLog(string.Format(ReceivesXPTemplate, character.Name, xp), "ReceivesXP");
    //}

    //public static void LogReachesNextLevel(Character.Character character)
    //{
    //    AddLog(string.Format(ReachesNextLevelTemplate, character.Name, character.Level), "ReachesNextLevel");
    //}

    //public static void LogHealing(Character.Character character, Character.Character patient,
    //    Consequence consequence, int totalValue, int skillValue, int diceValue, bool success)
    //{
    //    if (success)
    //    {
    //        AddLog(string.Format(HealingSuccessTemplate, character.Name, patient.Name,
    //                    consequence.Name, totalValue, skillValue, diceValue), "HealingSuccess");
    //    }
    //    else
    //    {
    //        AddLog(string.Format(HealingFailTemplate, character.Name, patient.Name,
    //                    consequence.Name, totalValue, skillValue, diceValue), "HealingFailure");
    //    }
    //}

    public void LogItemAdded(Inventory inventory, Item item, int amount)
    {
        LogMessage logMessage = LogMessageByName("ItemAdded");
        AddLog(string.Format(logMessage.Template, inventory.name, amount, item.Name), logMessage);
    }

    public void LogItemRemoved(Inventory inventory, Item item, int amount)
    {
        LogMessage logMessage = LogMessageByName("ItemRemoved");
        AddLog(string.Format(logMessage.Template, inventory.name, amount, item.Name), logMessage);
    }
}
