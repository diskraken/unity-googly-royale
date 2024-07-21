using System;
using System.Collections.Generic;
using UnityEngine;

namespace kenneyjam2024 {
    [Serializable]
    public class DuelData {
        public PlayerData player;
        public PlayerData opponent;
        public List<int> diceUsed;
        public (int x, int y, Choice type) choiceLast;

        public DuelData(PlayerData player, PlayerData opponent) {
            this.player = player;
            this.opponent = opponent;
            diceUsed = new();
            choiceLast = (-1, -1, Choice.Miss);
        }

        public void TryAddDice(int value) {
            if (!diceUsed.Contains(value)) {
                diceUsed.Add(value);
            }
        }

        public void TryAddScore(Choice choice) {
            if (choice == Choice.Miss) {
                return;
            }

            player.scores.Add(choice == Choice.Success);
            opponent.scores.Add(choice == Choice.Success);
        }
    }
}