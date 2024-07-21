using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace kenneyjam2024 {
    public class ScoresView : ViewBase, IView {

        [Header("ScoresView")]
        [SerializeField] private Transform _columnContainer;
        [SerializeField] private GameObject _columnPrefab;

        private List<ScoreColumnHandler> _scores = new List<ScoreColumnHandler>();

        public void OnEnable() {
            GameManager.Instance.OnUpdateScores += OnUpdateScores;
            GameManager.Instance.OnUpdateScoresLast += OnUpdateScoresLast;
            GameManager.Instance.OnUpdateScoresDeathEdges += OnDeathEdges;
        }
        public void OnDisable() {
            if (GameManager.Instance == null) {
                return;
            }
            GameManager.Instance.OnUpdateScores -= OnUpdateScores;
            GameManager.Instance.OnUpdateScoresLast -= OnUpdateScoresLast;
            GameManager.Instance.OnUpdateScoresDeathEdges -= OnDeathEdges;
        }

        private void OnUpdateScores(MatchData matchData) {
            _scores.ForEach(e => e.gameObject.SetActive(false));
            var playersAlive = matchData.players.Where(e => !e.dead).ToArray();
            for (var i = 0; i < playersAlive.Count(); i++) {
                //get
                ScoreColumnHandler col;
                if (_scores.Count > i) {
                    col = _scores[i];
                } else {
                    col = Instantiate(_columnPrefab, _columnContainer).GetComponent<ScoreColumnHandler>();
                    _scores.Add(col);
                }
                //init
                col.gameObject.SetActive(true);
                col.SetFaceColour(playersAlive[i].color);
                col.SetScore(playersAlive[i].scores);
                col.SetPlayerCharacter(matchData.playerCharacter == playersAlive[i]);
                col.SetPlayerDead(false);
            }
        }

        private void OnUpdateScoresLast(MatchData matchData) {
            StartCoroutine(RevealLastScoreCoroutine());
            IEnumerator RevealLastScoreCoroutine() {
                var scores = _scores.Where(e => e.gameObject.activeInHierarchy).OrderBy(a => System.Guid.NewGuid()).ToArray();
                foreach (var score in scores) {
                    score.RevealLastScore();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        private void OnDeathEdges() {
            _scores.Where(e => e.gameObject.activeInHierarchy).First().SetPlayerDead();
            _scores.Where(e => e.gameObject.activeInHierarchy).Last().SetPlayerDead();
        }
    }
}