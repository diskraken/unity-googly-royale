using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace kenneyjam2024 {
    public class ScoreColumnHandler : MonoBehaviour {

        [SerializeField] private GameObject _scoreOrbPrefab;
        [SerializeField] private Transform _scoreOrbsContainer;
        [SerializeField] private FaceHandler _faceHandler;
        [SerializeField] private GameObject _playerCharacterIndicator;

        private (int id, bool success) _nextOrb = (-1, true);
        private List<Image> _orbs = new();

        public void SetFaceColour(Color color) => _faceHandler.SetFaceColour(color);

        public void SetPlayerCharacter(bool player) {
            _playerCharacterIndicator?.SetActive(player);
            //_scoreOrbsContainer.GetComponent<VerticalLayoutGroup>().reverseArrangement = player;
        }

        public void SetPlayerDead(bool dead = true) {
            _faceHandler.SetDead(dead);
        }

        public void SetScore(List<bool> scores) {
            _nextOrb = (-1, true);
            _orbs.ForEach(e => e.gameObject.SetActive(false));
            for (int i = 0; i < scores.Count; i++) {
                Image orb;
                if (_orbs.Count > i) {
                    orb = _orbs[i];
                } else {
                    orb = Instantiate(_scoreOrbPrefab, _scoreOrbsContainer).GetComponent<Image>();
                    _orbs.Add(orb);
                }

                _faceHandler.SetDead(false);
                orb.gameObject.SetActive(true);
                orb.color = GetScoreColor(scores[i]);
                _nextOrb = (i, scores[i]);
            }

            if (_nextOrb.id < 0) {
                return;
            }
            _orbs[_nextOrb.id].color = GameManager.Instance.GameConfig.scoreColorEmpty;
        }

        public void RevealLastScore() {
            if (_nextOrb.id < 0) {
                return;
            }
            _orbs[_nextOrb.id].color = GetScoreColor(_nextOrb.success);
            GameManager.Instance.PlayAudioClip(Clip.scoreShowAudioClip);
        }

        private Color GetScoreColor(bool success) {
            return success ? GameManager.Instance.GameConfig.scoreColorSuccess : GameManager.Instance.GameConfig.scoreColorFailure;
        }
    }
}