using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace kenneyjam2024 {
    [Serializable]
    public class PlayerData {
        public List<int> dice = new List<int>();
        public List<bool> scores = new List<bool>();
        public int Wins => scores.Count(e => e);
        public Color color;
        public bool dead = false;

        public PlayerData(Color color) {
            this.color = color;
        }

        public void DrawHand(int size) {
            dice.Clear();
            for (var i = 0; i < size; i++) {
                dice.Add(i);
            }
        }
    }
}