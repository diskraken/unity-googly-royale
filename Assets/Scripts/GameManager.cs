using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace kenneyjam2024 {
    public class GameManager : MonoBehaviour {

        private static GameManager _instance = null;
        public event Action<DuelData> OnDuelStarted = (DuelData duel) => { };
        public event Action<DuelData> OnDuelUpdated = (DuelData duel) => { };

        [Header("UI")]
        [SerializeField] private UIHandler _uiHandler;
        [SerializeField] private GameConfig _gameConfig;

        private GameState _gameState;
        private MatchData _match;

        public GameConfig GameConfig => _gameConfig;
        public static GameManager Instance {
            get {
                _instance = GameObject.FindObjectOfType<GameManager>();
                return _instance;
            }
        }

        public void Awake() {
            Debug.Log($"{nameof(GameManager)}.Init");
            DontDestroyOnLoad(this);
            Application.targetFrameRate = 60;
            StartCoroutine(InitializeGame());
            IEnumerator InitializeGame() {
                yield return new WaitForSeconds(0.25f); // let em take breath
                _gameState = GameState.Invalid;
                RestartMatch(); // initial 
            }
        }

        public void SetState(GameState state) {
            var stateLast = this._gameState;
            _gameState = state;
            switch (state) {
                case GameState.Gameplay:
                    break;
                case GameState.ScoreShow:
                    break;
                case GameState.Menu:
                    break;
            }
            _uiHandler.ViewRedirect(state);
        }

        public void DiceButtonOnClick(int dieId) {

            // ai move + set dice
            var duel = _match.playerDuel;
            int dieIdAi = UnityEngine.Random.Range(0, 100) <= _gameConfig.aiSmartRatio
                ? AiMoveSmart(duel, duel.opponent)
                : AiMoveDumb(duel, duel.opponent);

            duel.opponent.dice.Remove(dieIdAi);
            duel.player.dice.Remove(dieId);
            duel.TryAddDice(dieIdAi);
            duel.TryAddDice(dieId);

            // check outcome
            var choice = Choice.Miss;
            if (dieId == dieIdAi) {
                choice = Choice.Success;
            } else if (duel.player.dice.Count == 0) {
                choice = Choice.Failure;
            }
            duel.choiceLast = (dieId, dieIdAi, choice);
            duel.TryAddScore(choice);
            //

            OnDuelUpdated?.Invoke(duel);
        }

        private void RestartMatch() {
            _match = new MatchData();

            // set players
            var colors = _gameConfig.playerColors.OrderBy(a => Guid.NewGuid()).ToList();
            for (var i = 0; i < _gameConfig.playersMax; i++) {
                var player = new PlayerData(colors[i % colors.Count]);
                player.DrawHand(_gameConfig.handSize);
                _match.players.Add(player);
            }
            _match.players[0].color = GameConfig.playerCharacterColor;
            _match.playerCharacter = _match.players[0];

            // set duels

            // tmp:
            _match.playerDuel = new DuelData(_match.players[0], _match.players[1]);

            // ev
            DuelStart(_match.playerDuel);
        }

        private void DuelStart(DuelData duel) {

            duel.player.DrawHand(_gameConfig.handSize);
            duel.opponent.DrawHand(_gameConfig.handSize);

            OnDuelStarted?.Invoke(_match.playerDuel);
            SetState(GameState.Gameplay);
        }

        private void DuelAutoPlay(DuelData duel) {

        }

        /*
        public int GetPlayerRank(Player player) {
            return 0;
        }

        private void OrderPlayersByScore() {

        }

        private void CutEdgePlayers() {

        }
        */

        private int AiMoveSmart(DuelData duel, PlayerData player) {
            if (player.scores == null || player.scores.Count == 0) {
                return AiMoveDumb(duel, player);
            }

            var direction = (double)player.scores.Count(e => e) / player.scores.Count;
            var set = player.dice.Where(e => (direction < 0.5f) ? !duel.diceUsed.Contains(e) : duel.diceUsed.Contains(e));
            Debug.Log($"AiMoveSmart - [dir:{direction}, choices:{set.Count()}]");
            return set.Any()
                ? set.ToArray()[UnityEngine.Random.Range(0, set.Count())]
                : AiMoveDumb(duel, player);
        }

        private int AiMoveDumb(DuelData duel, PlayerData player) {
            var dice = player.dice[UnityEngine.Random.Range(0, player.dice.Count)];
            return dice;
        }

        public void RedirectToScoresAfterDuel() {
            // TMP CODE

            StartCoroutine(RedirectToScores());

            IEnumerator RedirectToScores() {

                SetState(GameState.ScoreShow);

                yield return new WaitForSeconds(2f);

                // scores magic

                yield return new WaitForSeconds(1f);

                // TODO: highly tmp
                _match.playerDuel = new DuelData(_match.players[0], _match.players[1]);
                DuelStart(_match.playerDuel);
                //

                OnDuelStarted?.Invoke(_match.playerDuel);
                SetState(GameState.Gameplay);
            }

            // HOLD
            // TMP
        }

    }
}