using System.Collections.Generic;
using UnityEngine;

namespace kenneyjam2024 {
    public class UIHandler : MonoBehaviour {

        [Header("UI")]
        [SerializeField] private MenuView _menuView;
        [SerializeField] private GameplayView _gameplayView;
        [SerializeField] private ScoresView _scoresView;
        public List<ViewBase> views => new List<ViewBase>() { _menuView, _gameplayView, _scoresView };

        public void ViewRedirect(GameState gameState) {
            ViewsClose();
            switch (gameState) {
                case GameState.Gameplay:
                    _gameplayView.Show();
                    break;
                case GameState.ScoreShow:
                    _scoresView.Show();
                    break;
                case GameState.Win:
                case GameState.Lose:
                case GameState.Menu:
                    _menuView.InitWithState(gameState);
                    _menuView.Show();
                    break;
            }
        }

        public void ViewsClose() {
            views.ForEach(e => e.Hide());
        }
    }
}