using System.Collections.Generic;
using UnityEngine;

namespace kenneyjam2024 {
    [CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Custom/" + nameof(GameConfig))]
    public class GameConfig : ScriptableObject {

        [Header("Game rules")]
        [Range(1, 6)]
        public int handSize = 6;
        public int playersMax = 15;
        public int warmupRounds = 3;
        [Tooltip("e.g. 80 would mean, AI is going to play at it's best 80% of times")]
        [Range(0, 100)]
        public float aiSmartRatio = 50;

        [Header("Colors")]
        public Color diceColorSuccess;
        public Color diceColorFailure;

        public Color scoreColorSuccess;
        public Color scoreColorFailure;
        public Color scoreColorEmpty;

        public Color playerCharacterColor = Color.white;
        public List<Color> playerColors = new List<Color>();

    }
}