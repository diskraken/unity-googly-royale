using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kenneyjam2024 {
    public class ScoresView : ViewBase, IView {

        [Header("ScoresView")]
        [SerializeField] private Transform _menuContainer;
        [SerializeField] private Transform _columnContainer;
        [SerializeField] private GameObject _columnPrefab;

        private List<ScoreColumnHandler> _scoreColumnHandlers = new List<ScoreColumnHandler>();

        public void Init() {

        }
    }
}