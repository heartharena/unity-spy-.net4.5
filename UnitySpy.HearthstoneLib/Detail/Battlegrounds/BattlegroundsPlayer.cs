﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackF5.UnitySpy.HearthstoneLib.Detail.Battlegrounds
{
    internal class BattlegroundsPlayer : IBattlegroundsPlayer
    {
        public int Id { get; set; }
        
        public int EntityId { get; set; }

        public string CardId { get; set; }

        public string Name { get; set; }

        public int MaxHealth { get; set; }
        
        public int Damage { get; set; }

        public int LeaderboardPosition { get; set; }

        public int TriplesCount { get; set; }

        public int TechLevel { get; set; }

        public int WinStreak { get; set; }

        public int BoardCompositionRace { get; set; }

        public int BoardCompositionCount { get; set; }
    }
}
