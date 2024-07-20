using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameConfig), menuName = "Custom/"+nameof(GameConfig))]
public class GameConfig : ScriptableObject {

    [Header("Game rules")]
    public int diceCountMax = 6;
    public int playersMax = 15;
    public int warmupRounds = 3;

}
