using UnityEngine;
using UnityEngine.UI;

namespace kenneyjam2024 {
    public class FaceHandler : MonoBehaviour {
        [SerializeField] private Image _faceBack;

        public void SetFaceColour(Color color) {
            _faceBack.color = color;
        }
    }
}