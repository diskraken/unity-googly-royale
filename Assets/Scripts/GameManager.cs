using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour {

    private static GameManager _instance = null;

    [Header("UI")]
    [SerializeField] private MenuView _menuView;
    [SerializeField] private GameplayView _gameplayView;
    [SerializeField] private ScoresView _scoresView;

    [SerializeField]
    private GameConfig _gameConfig;

    private List<ViewBase> views => new List<ViewBase>() { _menuView, _gameplayView, _scoresView};

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

        // initial view
        _gameplayView.Show();
    }


    public void Back() {
        views.ForEach(e => e.Hide());

        if (_gameState != GameState.Gameplay) {
            _gameplayView.Show();
            _gameState = GameState.Gameplay;
        } else {
            _scoresView.Show();
            _gameState = GameState.ScoreShow;
        }

    }

    public void DiceButtonOnClick(int diceId) {
        Debug.Log($"{nameof(DiceButtonOnClick)}({diceId})");
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
}