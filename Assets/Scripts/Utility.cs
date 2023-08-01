using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public enum ItemType
    {
        Equipment,
        Gun,
        Melee,
        Value,
        Environment
    }


    /// <summary>
    /// AI
    /// </summary>
    public enum Factions
    {
        Civilian, //generally peaceful and will flee combat unless angered, and armed
        Security, //Responders are always of this faction unless bribed
        Criminal, //Acts the same as security but is hostile to the security faction
        Operator //the players faction
    }

    public enum Attitude
    {
        Afraid,
        Friendly, //helpful, suspicion rises slowly
        Calm, //Not obstructive, suspicion rises normally
        Angry //obstructive, suspicion rises quickly
    }

    public enum StatusEffects
    {
        LethalPoisoned, //they have been poisoned and will soon die, can be cured by a medic(only a medic notices if someone has been poisoned)
        SleepPoisoned, //they have been poisoned and will soon fall unconsious, can be cured by a medic(only a medic notices if someone has been poisoned)
        Stunned, //they are stunned and cannot see, hear, attack, or move
    }

    public enum healthLevel
    {
        Healthy,
        Wounded, //they have been injured severly, movement speed reduced, flees combat
        Unconsious, //they are neutralized non-lethaly, can be awoken by a medic
        Dead //they are neutralized lethaly, cant cure death
    }

    public enum Suspicion
    {
        Unsuspecting, //they dont suspect a thing
        Guarded, //they are a little on guard and will question, or capture strange individuals
        Alert, //very on guard and will capture, or kill strange individuals
        Searching, //activly looking for strange individuals
        Hunting //activly pusuing a strange individual
    }

    public enum audioType
    {
        Talking,
        Walking,
        Useing, 
    }
}
