
using UnityEngine;

namespace kenneyjam2024 {

    public enum GameState {
        Invalid = 0,
        Menu = 1,
        ScoreShow = 2,
        Gameplay = 3,
        Lose = 4,
        Win = 5,
        About = 6,
        Help = 7,
    }

    public enum Choice {
        Success = 1,
        Failure = 2,
        Miss = 3
    }

    public enum Clip {
        bgmClip = 0,
        deathAudioClip = 1,
        onclickAudioClip = 2,
        scoreShowAudioClip = 3,

        winAudioClip = 4,
        loseAudioClip = 5
    }
}