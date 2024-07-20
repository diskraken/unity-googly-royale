using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayView : ViewBase, IView {

    [Header("GameplayView")]
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform enemyHand;

    [SerializeField] private DiceButton playerChoice;
    [SerializeField] private DiceButton enemyChoice;
    [SerializeField] private List<DiceButton> playerDice = new List<DiceButton>();
    [SerializeField] private List<DiceButton> enemyDice = new List<DiceButton>();

    private (int x, int y) lastChoices = (-1, -1);

    protected override void OnAwake() {
        base.OnAwake();

        GameManager.Instance.OnDuelUpdated += OnDuelUpdated;
        GameManager.Instance.OnDuelStarted += OnNewDuelStarted;
        Clear();
    }

    protected override void OnBeforeShow() {
        this._canvasGroup.blocksRaycasts = false;
    }

    protected override void OnAfterShow() {
        this._canvasGroup.blocksRaycasts = true;
    }

    private void OnDisable() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnDuelUpdated -= OnDuelUpdated;
            GameManager.Instance.OnDuelStarted -= OnNewDuelStarted;
        }
    }

    public void OnNewDuelStarted(GameManager.Duel duel) {
        StartCoroutine(NewDuelCoroutine());
        IEnumerator NewDuelCoroutine() {
            _canvasGroup.blocksRaycasts = false;
            Clear();
            yield return new WaitForSeconds(0.4f);
            for (var i = 0; i < duel.player.dice.Count; i++) {
                playerDice[i].Init(i);
                enemyDice[i].Init(-1);
                yield return new WaitForSeconds(0.125f);
            }
            _canvasGroup.blocksRaycasts = true;
        }
    }

    private void OnDuelUpdated(GameManager.Duel duel) {
        if (lastChoices.x != duel.choiceLast.x | lastChoices.y != duel.choiceLast.y) {
            playerChoice.Init(duel.choiceLast.x);
            enemyChoice.Init(duel.choiceLast.y);
            enemyDice.Where(e => e.IsVisible).First()?.Hide();

            playerChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorFailure);
            enemyChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorFailure);

            if (duel.choiceLast.x == duel.choiceLast.y) {
                OnDuelWin();
            } else if (duel.player.dice.Count == 0) {
                OnDuelLose();
            } else {
                OnChoiceIncorrect();
            }

            lastChoices = (duel.choiceLast.x, duel.choiceLast.y);
        }

        playerDice.ForEach(e => {
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
        playerChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorSuccess);
        enemyChoice.UpdateDiceColor(GameManager.Instance.GameConfig.diceColorSuccess);
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
            List<DiceButton> dice = playerDice.Where(e => e.IsVisible).ToList();
            dice.AddRange(enemyDice.Where(e => e.IsVisible));
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
        playerDice.ForEach(e => e.Hide());
        enemyDice.ForEach(e => e.Hide());
        playerChoice.Hide();
        enemyChoice.Hide();
        lastChoices = (-1, -1);
    }

    public void OnClickBack() {
        // TODO: redirect to main-menu
    }
}
