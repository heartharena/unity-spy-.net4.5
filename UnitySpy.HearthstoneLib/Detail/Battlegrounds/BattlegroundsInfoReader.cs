﻿// ReSharper disable StringLiteralTypo
namespace HackF5.UnitySpy.HearthstoneLib.Detail.Battlegrounds
{
    using System;
    using System.Collections.Generic;

    internal static class BattlegroundsInfoReader
    {
        public static IBattlegroundsInfo ReadBattlegroundsInfo(HearthstoneImage image)
        {
            var battlegroundsInfo = new BattlegroundsInfo();

            var leaderboardMgr = image["PlayerLeaderboardManager"]?["s_instance"];
            var combatHistory = leaderboardMgr?["m_combatHistory"];
            // Also m_incomingHistory
            var numberOfPlayerTiles = leaderboardMgr?["m_playerTiles"]?["_size"];
            var playerTiles = leaderboardMgr?["m_playerTiles"]?["_items"];
            var playersList = new List<BattlegroundsPlayer>();
            for (int i = 0; i < numberOfPlayerTiles; i++)
            {
                var playerTile = playerTiles[i];
                // Info not available until the player mouses over the tile in the leaderboard, and there is no other way to get it
                string playerName = playerTile["m_mainCardActor"]?["m_playerNameText"]?["m_Text"];
                // Info not available until the player mouses over the tile in the leaderboard, and there is no other way to get it from memory
                //int triplesCount = playerTile["m_recentCombatsPanel"]?["m_triplesCount"] ?? -1;
                string playerCardId = playerTile?["m_entity"]?["m_cardIdInternal"];
                int playerHealth = playerTile["m_entity"]?["m_realTimeHealth"] ?? -1;
                int playerDamage = playerTile["m_entity"]?["m_realTimeDamage"] ?? -1;
                int leaderboardPosition = playerTile["m_entity"]?["m_realTimePlayerLeaderboardPlace"] ?? -1;
                int linkedEntityId = playerTile["m_entity"]?["m_realTimeLinkedEntityId"] ?? -1;
                int techLevel = playerTile["m_entity"]?["m_realTimePlayerTechLevel"] ?? -1;

                //int winStreak = playerTile["m_recentCombatsPanel"]?["m_winStreakCount"] ?? -1;
                var playerCombatHistoryIndex = -1;
                for (var j = 0; j < combatHistory["count"]; j++)
                {
                    if (combatHistory["keySlots"][j] == i)
                    {
                        playerCombatHistoryIndex = i;
                        break;
                    }
                }
                var currentWinStreak = 0;
                if (playerCombatHistoryIndex >= 0)
                {
                    var playerCombatHistory = combatHistory["valueSlots"][playerCombatHistoryIndex];
                    var numberOfBattles = playerCombatHistory["_size"];
                    // Keep that for later to build hte full battle history
                    //for (var j = 0; j < numberOfBattles; j++)
                    //{

                    //}
                    currentWinStreak = playerCombatHistory["_items"]?[numberOfBattles - 1]?["winStreak"];
                }

                // m_raceCounts is dangerous: it gives the exact race count for the board, so more info than what is available in game
                var numberOfRaces = playerTile["m_raceCounts"]?["count"] ?? 0;
                var highestNumber = 0;
                int highestRace = 0;
                for (var j = 0; j < numberOfRaces; j++)
                {
                    var race = playerTile["m_raceCounts"]["keySlots"][j];
                    var number = playerTile["m_raceCounts"]["valueSlots"][j];
                    if (number == highestNumber)
                    {
                        highestRace = 0;
                    }
                    else if (number > highestNumber)
                    {
                        highestNumber = number;
                        highestRace = race;
                    }
                }

                int boardCompositionRace = highestRace; // playerTile["m_recentCombatsPanel"]?["m_singleTribeWithCountName"]?["m_Text"];
                int boardCompositionNumber = highestNumber; // int.Parse(playerTile["m_recentCombatsPanel"]?["m_singleTribeWithCountNumber"]?["m_Text"] ?? "-1");

                //var recentCombatHistory = playerTile["m_recentCombatsPanel"]?["m_recentCombatEntries"]?["m_list"];
                //var numberOfRecentCombatHistory = recentCombatHistory?["_size"] ?? 0;
                //for (var j = 0; j < numberOfRecentCombatHistory; j++)
                //{
                //    var combatEntry = recentCombatHistory["_items"]?[j];
                //    var opponentId = combatEntry["m_opponentId"];
                //    var ownerId = combatEntry["m_ownerId"];
                //    var damage = combatEntry["m_splatAmount"];
                //}
                var player = new BattlegroundsPlayer
                {
                    Id = i + 1,
                    EntityId = linkedEntityId,
                    Name = playerName,
                    CardId = playerCardId,
                    MaxHealth = playerHealth,
                    Damage = playerDamage,
                    LeaderboardPosition = leaderboardPosition,
                    BoardCompositionRace = boardCompositionRace,
                    BoardCompositionCount = boardCompositionNumber,
                    //TriplesCount = triplesCount,
                    TechLevel = techLevel,
                    WinStreak = currentWinStreak,
                };
                playersList.Add(player);
            }
            battlegroundsInfo.Game = new BattlegroundsGame
            {
                Players = playersList,
            };

            var netCacheValues = image.GetService("NetCache")?["m_netCache"]?["valueSlots"];
            if (netCacheValues != null)
            {
                foreach (var netCache in netCacheValues)
                {
                    Console.WriteLine("" + netCache?.TypeDefinition.Name);
                    if (netCache?.TypeDefinition.Name == "NetCacheBaconRatingInfo")
                    {
                        battlegroundsInfo.Rating = netCache["<Rating>k__BackingField"] ?? -1;
                    }
                }
            }

            return battlegroundsInfo;
        }
    }
}