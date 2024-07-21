using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace kenneyjam2024 {
    public class GameManager : MonoBehaviour {

        private static GameManager _instance = null;
        public event Action<DuelData> OnDuelStarted = (DuelData duel) => { };
        public event Action<DuelData> OnDuelUpdated = (DuelData duel) => { };
        public event Action<MatchData> OnUpdateScores = (MatchData match) => { };
        public event Action<MatchData> OnUpdateScoresLast = (MatchData match) => { };
        public event Action OnUpdateScoresDeathEdges = () => { };

        [Header("UI")]
        [SerializeField] private UIHandler _uiHandler;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _audioSourceBgm;

        private GameState _gameState;
        private MatchData _match;

        public GameConfig GameConfig => _gameConfig;
        public AudioSource AudioSource => _audioSource;
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
                SetState(GameState.Menu);
            }
        }

        public void Update() {
            if (Application.isEditor) {
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    // win duel
                    var duel = _match.playerDuel;
                    int dieIdAi = GetAiMove(duel, duel.opponent);
                    var choice = OnDuelFight(duel, 1, 1);
                    // update ui
                    OnDuelUpdated?.Invoke(duel);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    // win duel
                    var duel = _match.playerDuel;
                    duel.player.dice = new() { 1 };
                    int dieIdAi = GetAiMove(duel, duel.opponent);
                    var choice = OnDuelFight(duel, 1, 3);
                    // update ui
                    OnDuelUpdated?.Invoke(duel);
                }
            }
        }

        public void PlayAudioClip(Clip clip) {
            var audioClip = clip switch {
                Clip.winAudioClip => GameConfig.winAudioClip,
                Clip.loseAudioClip => GameConfig.loseAudioClip,
                Clip.onclickAudioClip => GameConfig.onclickAudioClip,
                Clip.deathAudioClip => GameConfig.deathAudioClip,
                Clip.scoreShowAudioClip => GameConfig.scoreShowAudioClip,
            };
            AudioSource.PlayOneShot(audioClip);
        }

        public void SetState(GameState state) {
            var stateLast = this._gameState;
            _gameState = state;
            /*
            switch (state) {
                case GameState.Gameplay:
                    break;
                case GameState.ScoreShow:
                    break;
                case GameState.Menu:
                    break;
            }*/
            _uiHandler.ViewRedirect(state);
        }

        public void DiceButtonOnClick(int dieId) {
            // ai move + set dice
            var duel = _match.playerDuel;
            int dieIdAi = GetAiMove(duel, duel.opponent);
            var choice = OnDuelFight(duel, dieId, dieIdAi);

            // update ui
            OnDuelUpdated?.Invoke(duel);
        }

        private Choice OnDuelFight(DuelData duel, int dieP1, int dieP2) {
            duel.opponent.dice.Remove(dieP2);
            duel.player.dice.Remove(dieP1);
            duel.TryAddDice(dieP2);
            duel.TryAddDice(dieP1);

            // check outcome
            Choice result = Choice.Miss;
            if (dieP1 == dieP2) {
                result = Choice.Success;
            } else if (duel.player.dice.Count == 0) {
                result = Choice.Failure;
            }
            duel.choiceLast = (dieP1, dieP2, result);
            duel.TryAddScore(result);
            return result;
        }

        public void NewMatch() {
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
            SetAndStartNewDuels();
        }

        private void SetAndStartNewDuels() {
            _match.roundId++;
            var set = _match.players.Where(e => !e.dead).ToList();

            // set player duel
            set.Remove(_match.playerCharacter);
            var opponent = set[UnityEngine.Random.Range(0, set.Count)];
            set.Remove(opponent);
            _match.playerDuel = new DuelData(_match.playerCharacter, opponent);

            // set other duels
            _match.duels.Clear();
            set = set.OrderBy(e => Guid.NewGuid()).ToList();
            while (set.Count > 1) {
                _match.duels.Add(new(set[0], set[1]));
                set.RemoveAt(0);
                set.RemoveAt(0);
            }
            if (set.Count > 0) {
                // odd remainder instawins
                set[0].scores.Add(true);
            }

            // autobattle cpu's
            _match.duels.ForEach(duel => DuelAutoPlay(duel));
            // player duel
            DuelStart(_match.playerDuel);
        }


        private void DuelStart(DuelData duel) {
            duel.player.DrawHand(_gameConfig.handSize);
            duel.opponent.DrawHand(_gameConfig.handSize);

            OnDuelStarted?.Invoke(_match.playerDuel);
            SetState(GameState.Gameplay);
        }

        private void DuelAutoPlay(DuelData duel) {
            var choice = Choice.Miss;
            duel.player.DrawHand(_gameConfig.handSize);
            duel.opponent.DrawHand(_gameConfig.handSize);

            while (true) {
                int dieP1 = GetAiMove(duel, duel.player);
                int dieP2 = GetAiMove(duel, duel.opponent);
                choice = OnDuelFight(duel, dieP1, dieP2);
                if (choice != Choice.Miss || duel.player.dice.Count == 0 || duel.opponent.dice.Count == 0) {
                    break;
                }
            }
        }

        private void OrderPlayersByScore(MatchData match) {
            _match.players = _match.players.Where(e => !e.dead).OrderByDescending(e => e.Wins).ToList();
        }

        private int GetAiMove(DuelData duel, PlayerData player) {
            return UnityEngine.Random.Range(0, 100) <= _gameConfig.aiSmartRatio
                ? AiMoveSmart(duel, player)
                : AiMoveDumb(duel, player);
        }

        private int AiMoveSmart(DuelData duel, PlayerData player) {
            if (player.scores == null || player.scores.Count == 0) {
                return AiMoveDumb(duel, player);
            }

            var direction = (double)player.Wins / player.scores.Count;
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

            StartCoroutine(OnDuelCompleted());

            IEnumerator OnDuelCompleted() {

                SetState(GameState.ScoreShow);
                OrderPlayersByScore(_match);
                OnUpdateScores?.Invoke(_match);

                yield return new WaitForSeconds(0.75f);

                OnUpdateScoresLast?.Invoke(_match);

                yield return new WaitForSeconds(3f);

                // kill time / death time
                if (_match.roundId >= _gameConfig.warmupRounds) {

                    // save player from death if tied
                    if (_match.players.First() == _match.playerCharacter
                        && _match.playerCharacter.scores == _match.players[1].scores) {
                        var opponent = _match.players[1];
                        _match.players.RemoveAt(1);
                        _match.players.Insert(0, opponent);
                    } else if (_match.players.Last() == _match.playerCharacter
                        && _match.playerCharacter.scores == _match.players[_match.players.Count - 2].scores) {
                        var opponent = _match.players[_match.players.Count - 2];
                        _match.players.RemoveAt(_match.players.Count - 2);
                        _match.players.Add(opponent);
                    }

                    // kill 
                    _match.players.First().dead = true;
                    _match.players.Last().dead = true;
                    OnUpdateScoresDeathEdges?.Invoke();
                    GameManager.Instance.PlayAudioClip(Clip.deathAudioClip);
                    yield return new WaitForSeconds(1.5f);
                }

                if (_match.players.Count == 1) {
                    // win condition
                    yield return new WaitForSeconds(1.5f);
                    GameManager.Instance.PlayAudioClip(Clip.winAudioClip);
                    SetState(GameState.Win);
                } else if (_match.playerCharacter.dead) {
                    // lose conditions met
                    yield return new WaitForSeconds(1.5f);
                    GameManager.Instance.PlayAudioClip(Clip.loseAudioClip);
                    SetState(GameState.Lose);
                } else {
                    // if player alive -> reroll new duels and start
                    SetAndStartNewDuels();
                }
            }
        }
    }
}