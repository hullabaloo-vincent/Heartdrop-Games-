using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour 
{
    [Header("Spell Defaults")]
    public float SpellDelay;
    public float Damage;
    public float DestroyTime;
    public enum SpellLocation 
    {
        RightHand,
        LeftHand,
        LeftFoot,
        RightFoot,
        Floor
    }

    public enum CastingAnimation 
    {
        HandCastSmall,
        HandCastLarge,
        Stomp,
    }

    public (SpellLocation, CastingAnimation, float) SpellInit() 
    {
        return (Location, Animation, SpellDelay);
    }

    public float GetSpellDelay() 
    {
        return SpellDelay;
    }

    public float GetSpellDamage() 
    {
        return Damage;
    }

    public float GetSpellDestroyTime() 
    {
        return DestroyTime;
    }
    [Tooltip("Where the spell with spawn in relation to the player.")]
    public SpellLocation Location;
    [Tooltip("What casting animation should play.")]
    public CastingAnimation Animation;
}
