﻿using AdventureQuestGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureQuestGame.Services
{
    [Serializable]
    public class SpellException : Exception
    {
        public SpellException() { }
        public SpellException(string message) : base(message) { }
        public SpellException(string message, Exception inner) : base(message, inner) { }
        protected SpellException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class CombatOnlySpellException : SpellException
    {
        private static readonly string CombatOnlyMsg = "This spell can only be used in combat.";

        public CombatOnlySpellException()
            : base(CombatOnlyMsg)
            {}
        public CombatOnlySpellException(string message) : base(message) { }
        public CombatOnlySpellException(string message, Exception inner) : base(message, inner) { }
        protected CombatOnlySpellException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    //The action names that a spell can do
    static class SpellStatics
    {

        public static string Damage(Player p, Spell theSpell)
        {
            if (!p.isInCombat)
                throw new CombatOnlySpellException();

            int damage = theSpell.damage;
            p.engaging.attribute.currentHealth -= damage;
            p.attributes.currentMana -= theSpell.manaCost;
            return String.Format("I cast {0} against {1}, causing {2} damage! {1} has {3} health remaining.", theSpell.name, p.engaging.name, damage, Math.Max(0, p.engaging.attribute.currentHealth));
        }

        public static string Heal(Player p, Spell theSpell)
        {
            int value = theSpell.damage;
            int diff = p.attributes.health - p.attributes.currentHealth;
            if (value <= diff)
            {
                p.attributes.currentHealth += value;
                return String.Format("I have healed myself for {0} health points.", value);
            }
            p.attributes.currentHealth = p.attributes.health;
            return String.Format("I have healed myself for {0} health points.", diff);
        }

        public static string BuffStrength(Player p, Spell theSpell)
        {
            if (!p.isInCombat)
                 throw new CombatOnlySpellException();

            int value = theSpell.damage;
            p.attributes.currentStrength += value;
            return String.Format("My Strength has been increased by {0}.", value);
        }

        public static string BuffToughness(Player p, Spell theSpell)
        {
            if (!p.isInCombat)
                throw new CombatOnlySpellException();

            int value = theSpell.damage;
            p.attributes.currentToughness += value;
            return String.Format("My Toughness has been increased by {0}.", value);
        }

        public static string DebuffStrength(Player p, Spell theSpell)
        {
            if (!p.isInCombat)
                throw new CombatOnlySpellException();

            int value = theSpell.damage;
            value = Math.Max(0, p.engaging.attribute.currentStrength - value);
            p.engaging.attribute.currentStrength -= value;
            return String.Format("{0} Strength has been decreased by {1}.", p.engaging.name, value);
        }

        public static string DebuffToughness(Player p, Spell theSpell)
        {
            if (!p.isInCombat)
                throw new CombatOnlySpellException();

            int value = theSpell.damage;
            value = Math.Max(0, p.engaging.attribute.currentToughness - value);
            p.engaging.attribute.currentToughness -= value;
            return String.Format("{0} Toughness has been decreased by {1}.", p.engaging.name, value);
        }
    }
}
