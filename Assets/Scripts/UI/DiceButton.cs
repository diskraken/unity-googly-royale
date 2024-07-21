using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace kenneyjam2024 {
    public class DiceButton : MonoBehaviour {

        [SerializeField] private List<Transform> _values = new List<Transform>();
        [SerializeField] private Animator _animator;
        [SerializeField] private Image _diceImage;

        private int _id = 0;
        private bool _isVisible = false;

        public int Id => _id;
        public bool IsVisible => _isVisible;

        public void Init(int id) {
            _id = id;
            _values.ForEach(e => e.gameObject.SetActive(false));
            if (_id >= 0) {
                _values[_id].gameObject.SetActive(true);
            }
            GetComponent<CanvasGroup>().blocksRaycasts = id >= 0;
            UpdateDiceColor(Color.white);
            Show();
        }

        public void OnClick() {
            GameManager.Instance.PlayAudioClip(Clip.onclickAudioClip);
            GameManager.Instance.DiceButtonOnClick(_id);
            Hide();
        }

        public void Hide(bool instant = false) {
            _animator.SetTrigger("Hide");
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            _isVisible = false;
        }

        public void Show() {
            _animator.SetTrigger("Show");
            GetComponent<CanvasGroup>().blocksRaycasts = _id >= 0;
            _isVisible = true;
        }

        public void UpdateDiceColor(Color color) {
            _diceImage.color = color;
        }

        public void UpdateDiceColor(bool wasUsed) {
            _diceImage.color = wasUsed ? GameManager.Instance.GameConfig.diceColorFailure : Color.white;
        }
    }
}