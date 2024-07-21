using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace kenneyjam2024 {
    public class ScoreColumnHandler : MonoBehaviour {

        [SerializeField] private GameObject _scoreOrbPrefab;
        [SerializeField] private Transform _scoreOrbsContainer;
        [SerializeField] private FaceHandler _faceHandler;

        private (int id, bool success) _nextOrb = (0, true);
        private List<Image> _scoreOrbs = new();

        public void SetFaceColour(Color color) => _faceHandler.SetFaceColour(color);

        public void SetScore(List<bool> scores) {
            // TODO: fix, pooler
            int i = 0;
            _scoreOrbs.ForEach(e => e.gameObject.SetActive(false));
            for (i = 0; i < scores.Count && i < _scoreOrbs.Count; i++) {
                _scoreOrbs[i].gameObject.SetActive(true);
                _scoreOrbs[i].color = GetScoreColor(scores[i]);
                _nextOrb = (i, scores[i]);
            }
            _scoreOrbs[_nextOrb.id].color = GameManager.Instance.GameConfig.scoreColorEmpty;
        }

        public void RevealLastScore() {
            _scoreOrbs[_nextOrb.id].color = GetScoreColor(_nextOrb.success);
            //TODO: fx
        }

        private Color GetScoreColor(bool success) {
            return success ? GameManager.Instance.GameConfig.scoreColorSuccess : GameManager.Instance.GameConfig.scoreColorFailure;
        }
    }
}