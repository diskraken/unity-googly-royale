using System;
using System.Collections.Generic;

namespace kenneyjam2024 {
    [Serializable]
    public class MatchData {
        public List<PlayerData> players = new List<PlayerData>();
        public List<DuelData> duels = new List<DuelData>();

        public DuelData playerDuel;
        public PlayerData playerCharacter;
        public int roundId = 0;
    }
}