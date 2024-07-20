using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour {

    private static GameManager _instance = null;

    public event Action<Duel> OnDuelStarted = (Duel duel) => { };
    public event Action<Duel> OnDuelUpdated = (Duel duel) => { };

    [Header("UI")]
    [SerializeField] private MenuView _menuView;
    [SerializeField] private GameplayView _gameplayView;
    [SerializeField] private ScoresView _scoresView;

    [SerializeField]
    private GameConfig _gameConfig;
    public GameConfig GameConfig => _gameConfig;

    private List<ViewBase> views => new List<ViewBase>() { _menuView, _gameplayView, _scoresView };
    private GameState _gameState = GameState.Invalid;


    public static GameManager Instance {
        get {
            _instance = GameObject.FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    public void Awake() {
        Debug.Log($"{nameof(GameManager)}.Init");
        DontDestroyOnLoad(this);
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize() {
        // let em take breath
        yield return new WaitForSeconds(0.25f);

        Application.targetFrameRate = 60;

        // initial view
        _gameState = GameState.Gameplay;
        _gameplayView.Show();

        RestartGame();
        playerDuel = new Duel(players[0], players[1]);

        OnDuelStarted?.Invoke(playerDuel);
    }


    public void Back() {
        // TMP CODE
        /*
        views.ForEach(e => e.Hide());

        if (_gameState != GameState.Gameplay) {
            _gameplayView.Show();
            _gameState = GameState.Gameplay;

        } else {
            _scoresView.Show();
            _gameState = GameState.ScoreShow;
        }
        */
        // TMP placehodler
    }

    public void DiceButtonOnClick(int dieId) {

        int aiRoll = UnityEngine.Random.Range(0, 100) <= _gameConfig.aiSmartRatio
            ? AiMoveSmart(playerDuel, playerDuel.opponent)
            : AiMoveDumb(playerDuel, playerDuel.opponent);
        playerDuel.TryAddDice(aiRoll);

        playerDuel.player.dice.Remove(dieId);
        playerDuel.TryAddDice(dieId);

        playerDuel.choiceLast = (dieId, aiRoll);
        OnDuelUpdated?.Invoke(playerDuel);
    }

    public enum GameState {
        Invalid = 0,
        Menu = 1,
        ScoreShow,
        Gameplay,
        Paused,
        Help,
    }

    public enum GameplayState {
        Invalid = 0,
        Start = 1,
        DiceChoose = 2,
        DiceShow = 3,
        Exit = 4,
    }

    private void RestartGame() {
        players.Clear();

        var colors = _gameConfig.playerColors.OrderBy(a => Guid.NewGuid()).ToList();
        for (var i = 0; i < _gameConfig.playersMax; i++) {
            var player = new Player(colors[i % colors.Count]);
            player.DrawHand(_gameConfig.handSize);
            players.Add(player);
        }
    }
    public int GetScoreMiddle() {
        return 0;
    }

    public int GetPlayerRank(Player player) {
        return 0;
    }

    private void OrderPlayersByScore() {

    }

    private void CutEdgePlayers() {

    }

    public class Player {
        public List<int> dice = new List<int>();
        public int wins = 0;
        public int loses = 0;
        public Color color;

        public Player(Color color) {
            this.color = color;
        }

        public void DrawHand(int size) {
            dice.Clear();
            for (var i = 0; i < size; i++) {
                dice.Add(i);
            }
        }
    }

    public class Duel {
        public Player player;
        public Player opponent;
        public List<int> diceUsed;
        public (int x, int y) choiceLast;

        public Duel(Player player, Player opponent) {
            this.player = player;
            this.opponent = opponent;
            diceUsed = new();
            choiceLast = (-1, -1);
        }

        public void TryAddDice(int value) {
            if (!diceUsed.Contains(value)) {
                diceUsed.Add(value);
            }
        }
    }

    private List<Player> players = new List<Player>();
    private Player playerCharacter;
    private Duel playerDuel;

    private int AiMoveSmart(Duel duel, Player player) {
        // TODO:
        return AiMoveDumb(duel, player);
    }

    private int AiMoveDumb(Duel duel, Player player) {
        var dice = player.dice[UnityEngine.Random.Range(0, player.dice.Count)];
        player.dice.Remove(dice);
        return dice;
    }

    public void RedirectToScoresAfterDuel() {
        // TMP CODE

        StartCoroutine(RedirectToScores());

        IEnumerator RedirectToScores() {

            views.ForEach(e => e.Hide());
            _scoresView.Show();
            _gameState = GameState.ScoreShow;

            yield return new WaitForSeconds(2f);

            // scores magic

            yield return new WaitForSeconds(1f);

            StartCoroutine(Initialize());
            views.ForEach(e => e.Hide());
            _gameplayView.Show();
            _gameState = GameState.Gameplay;
        }

        // HOLD
        // TMP
    }

}