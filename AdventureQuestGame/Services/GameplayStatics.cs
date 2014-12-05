﻿using AdventureQuestGame.Contexts;
using AdventureQuestGame.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureQuestGame.Services
{
    public static class GameplayStatics
    {
        public static IList<string> DetectCompletedQuests(GameContext GameCtx, Player player)
        {
            List<string> results = new List<string>();
            if (player.quests.Quests != null && player.quests.Quests.Count > 0)
            {
                if (player.navigation.currentRoom != null)
                {
                    var quest = player.quests.Quests.FirstOrDefault(q => !q.Complete && q.Quest.Type == QuestType.GoTo && q.Quest.NameOfObject.Equals(player.navigation.currentRoom.name));
                    if (quest != null)
                        quest.Count++;
                }

                var qq = player.quests.Quests.FirstOrDefault(qs => !qs.Complete && qs.Quest.Type == QuestType.GoTo && qs.Quest.NameOfObject.Equals(player.navigation.currentLocation.name));
                if (qq == null)
                    qq = player.quests.Quests.FirstOrDefault(qs => !qs.Complete && qs.Quest.Type == QuestType.GoTo && qs.Quest.NameOfObject.Equals(player.navigation.currentArea.name));

                if (qq != null)
                    qq.Count++;

                var check = player.quests.Quests.Where(q => !q.Complete).ToList();
                if (check != null)
                {
                    foreach (var q in check)
                    {
                        if (q.Quest.IsComplete(q.Count))
                        {
                            q.MakeComplete(player);
                            results.Add(String.Format("Completed {0}! Received {1} gold and {2} experience.", q.Quest.Title, q.Quest.Gold, q.Quest.Experience));
                            GameCtx.achievements.Add(new Acheivement(String.Format("Quest Completed: {0}", q.Quest.Title), "Completing a quest has this wonderful feeling behind it. A sense of accomplishment, a rush from the excitement, and the bulging purse from the extra coins. Indeed, all these things make a quest worth the work.", player));

                            if (q.Quest.NextQuest != null)
                            {
                                player.quests.Quests.Add(new PlayerQuestQuest(q.Quest.NextQuest));
                                results.Add(String.Format("New Quest Started: {0}. {1}. {2}", q.Quest.NextQuest.Title, q.Quest.NextQuest.Description, q.Quest.NextQuest.Instructions));
                            }
                        }
                    }
                }
            }
            return results;
        }


        public static void DetectLevelUpEvent(GameContext GameCtx, Player player)
        {
            if (player.attributes.leveledUp)
            {
                List<Title> sorted = GameCtx.titles
                    .Where(t => t.levelToAcheive <= player.attributes.level)
                    .OrderBy(t => t.levelToAcheive)
                    .ToList();

                sorted.Reverse();

                player.title = sorted
                    .First()
                    .Copy();
                
                Spell[] spells = GameCtx.spells.Where(s => s.minLevel <= player.attributes.level).ToArray();
                foreach(Spell s in spells)
                {
                    bool has = false;
                    foreach(Spell ps in player.spells)
                        has |= ps.name.Equals(s.name);
                    
                    if (!has)
                        player.spells.Add(s/*.Copy()*/);
                }

                GameCtx.achievements.Add(new Acheivement("Has Grown More Wiser", String.Format("Gained another point to their level, making them stronger, braver, faster than before. This is granted for reaching level {0}.", player.attributes.level), player));
            }
            player.attributes.leveledUp = false;
        }

        public static void DetectOneToOneRemovals(GameContext GameCtx, Player player)
        {
            if (player.equipment.feet != null && player.equipment.feet.durability <= 0)
            {
                GameCtx.Entry(player.equipment.feet).State = EntityState.Deleted;
            }

            if (player.equipment.head != null && player.equipment.head.durability <= 0)
            {
                GameCtx.Entry(player.equipment.head).State = EntityState.Deleted;
            }

            if (player.equipment.torso != null && player.equipment.torso.durability <= 0)
            {
                GameCtx.Entry(player.equipment.torso).State = EntityState.Deleted;
            }

            if (player.equipment.arm != null && player.equipment.arm.durability <= 0)
            {
                GameCtx.Entry(player.equipment.arm).State = EntityState.Deleted;
            }

            if (player.equipment.legs != null && player.equipment.legs.durability <= 0)
            {
                GameCtx.Entry(player.equipment.legs).State = EntityState.Deleted;
            }

            if (player.navigation.currentRoom != null && player.expireRoom)
            {
                GameCtx.players.First(p => p.Id == player.Id).navigation.currentRoom = null;
                player.expireRoom = false;
            }
        }
    }
}
