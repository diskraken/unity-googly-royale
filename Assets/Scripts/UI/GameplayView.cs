using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace kenneyjam2024 {
    public class GameplayView : ViewBase, IView {

        [Header("GameplayView")]
        [SerializeField] private GameObject _dicePrefab;
        [SerializeField] private Transform _playerHand;
        [SerializeField] private Transform _enemyHand;
        [SerializeField] private DiceButton _playerChoice;
        [SerializeField] private DiceButton _enemyChoice;
        [SerializeField] private List<DiceButton> _playerDice = new List<DiceButton>();
        [SerializeField] private List<DiceButton> _enemyDice = new List<DiceButton>();
        [SerializeField] private FaceHandler _faceHandlerPlayer;
        [SerializeField] private FaceHandler _faceHandlerEnemy;

        private (int x, int y) _lastChoices = (-1, -1);

        protected override void OnAwake() {
            base.OnAwake();
            GameManager.Instance.OnDuelUpdated += OnDuelUpdated;
            GameManager.Instance.OnDuelStarted += OnNewDuelStarted;
            Clear();
        }

        private void OnDisable() {
            if (GameManager.Instance != null) {
                GameManager.Instance.OnDuelUpdated -= OnDuelUpdated;
                GameManager.Instance.OnDuelStarted -= OnNewDuelStarted;
            }
        }

        protected override void OnBeforeShow() {
            this._canvasGroup.blocksRaycasts = false;
        }

        protected override void OnAfterShow() {
            this._canvasGroup.blocksRaycasts = true;
        }

        public void OnNewDuelStarted(DuelData duel) {
            _faceHandlerPlayer.SetFaceColour(duel.player.color);
            _faceHandlerEnemy.SetFaceColour(duel.opponent.color);

            StartCoroutine(NewDuelCoroutine());
            IEnumerator NewDuelCoroutine() {
                _canvasGroup.blocksRaycasts = false;
                Clear();
                yield return new WaitForSeconds(0.4f);
                for (var i = 0; i < duel.player.dice.Count; i++) {
                    _playerDice[i].Init(i);
                    _enemyDice[i].Init(-1);
                    yield return new WaitForSeconds(0.125f);
                }
                _canvasGroup.blocksRaycasts = true;
            }
        }

        private void OnDuelUpdated(DuelData duel) {
            if (_lastChoices.x != duel.choiceLast.x | _lastChoices.y != duel.choiceLast.y) {
                _playerChoice.Init(duel.choiceLast.x);
                _enemyChoice.Init(duel.choiceLast.y);
                _enemyDice.Where(e => e.IsVisible).First()?.Hide();

                _playerChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorFailure);
                _enemyChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorFailure);

                switch (duel.choiceLast.type) {
                    case Choice.Success:
                        OnDuelWin();
                        break;
                    case Choice.Failure:
                        OnDuelLose();
                        break;
                    case Choice.Miss:
                        OnChoiceIncorrect();
                        break;
                }

                _lastChoices = (duel.choiceLast.x, duel.choiceLast.y);
            }

            _playerDice.ForEach(e => {
                e.UpdateDiceColor(duel.diceUsed.Contains(e.Id));
            });
        }

        private void OnChoiceIncorrect() {
            // TODO: 
            // - add sfx
            // - add fx lite
        }

        private void OnDuelWin() {
            this._canvasGroup.blocksRaycasts = false;
            _playerChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorSuccess);
            _enemyChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorSuccess);
            OnDuelEnd();
        }

        private void OnDuelLose() {
            this._canvasGroup.blocksRaycasts = false;
            OnDuelEnd();
        }

        private void OnDuelEnd() {
            StartCoroutine(OnDuelEndCoroutine());
            IEnumerator OnDuelEndCoroutine() {

                // hide stuff
                List<DiceButton> dice = _playerDice.Where(e => e.IsVisible).ToList();
                dice.AddRange(_enemyDice.Where(e => e.IsVisible));
                dice.OrderBy(e => e.transform.position.x);
                for (var i = 0; i < dice.Count; i++) {
                    dice[i].Hide();
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(1f);

                // redirect
                GameManager.Instance.RedirectToScoresAfterDuel();
            }
        }

        private void Clear() {
            _playerDice.ForEach(e => e.Hide(true));
            _enemyDice.ForEach(e => e.Hide(true));
            _playerChoice.Hide(true);
            _enemyChoice.Hide(true);
            _lastChoices = (-1, -1);
        }
    }
}