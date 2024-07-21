using UnityEngine;
using UnityEngine.UI;

namespace kenneyjam2024 {
    public class FaceHandler : MonoBehaviour {
        [SerializeField] private Image _faceBack;
        [SerializeField] private Animator _deadAnimator;

        public void SetFaceColour(Color color) {
            _faceBack.color = color;
        }

        public void SetDead(bool dead) {
            _deadAnimator.GetComponent<CanvasGroup>().alpha = 0;
            _deadAnimator.ResetTrigger("Rip");
            _deadAnimator.enabled = false;
            if (dead) {
                _deadAnimator.enabled = true;
                _deadAnimator.SetTrigger("Rip");
            }
        }
    }
}