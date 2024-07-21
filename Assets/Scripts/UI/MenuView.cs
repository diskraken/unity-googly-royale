using UnityEngine;

namespace kenneyjam2024 {
    public class MenuView : ViewBase, IView {

        [Header("MenuView")]
        [SerializeField] private Transform _logo;
        [SerializeField] private Transform _about;
        [SerializeField] private Transform _help;
        [SerializeField] private Transform _win;
        [SerializeField] private Transform _lose;

        GameState _lastState = GameState.Invalid;

        public void Awake() {
            InitWithState(GameState.Menu);
        }

        public void InitWithState(GameState state) {
            _lastState = state;

            _logo.gameObject.SetActive(false);
            _about.gameObject.SetActive(false);
            _help.gameObject.SetActive(false);
            _win.gameObject.SetActive(false);
            _lose.gameObject.SetActive(false);

            switch (state) {
                case GameState.Menu:
                    _logo.gameObject.SetActive(true);
                    break;
                case GameState.About:
                    _about.gameObject.SetActive(true);
                    break;
                case GameState.Help:
                    _help.gameObject.SetActive(true);
                    break;
                case GameState.Lose:
                    _lose.gameObject.SetActive(true);
                    break;
                case GameState.Win:
                    _win.gameObject.SetActive(true);
                    break;
            }
        }

        public void OnClickNewGame() {
            GameManager.Instance.PlayAudioClip(Clip.onclickAudioClip);
            GameManager.Instance.NewMatch();
        }

        public void OnClickHelp() {
            GameManager.Instance.PlayAudioClip(Clip.onclickAudioClip);
            if (_lastState == GameState.About) {
                InitWithState(GameState.Help);
            } else {
                InitWithState(GameState.About);
            }
        }
    }
}