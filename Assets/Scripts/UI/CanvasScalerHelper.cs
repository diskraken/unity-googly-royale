using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScalerHelper : MonoBehaviour {

    [SerializeField] private float targetAspectRatioThreshold = 1.6f;
    [SerializeField] private float updateStep = 0.5f;

    private CanvasScaler _canvasScaler;
    private float _screenWidthLast;
    private float _screenHeightLast;
    private float _aspectRatio;
    private Coroutine _coroutine;

    private void Start() {
        _canvasScaler = GetComponent<CanvasScaler>();
        _screenWidthLast = Screen.width;
        _screenHeightLast = Screen.height;
        _coroutine = StartCoroutine(UpdateCanvasScalerCoroutine());
    }

    private void OnDisable() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
    }

    private IEnumerator UpdateCanvasScalerCoroutine() {
        while (true) {
            if (Screen.width != _screenWidthLast || Screen.height != _screenHeightLast) {
                AdjustCanvasScaler();
            }
            yield return new WaitForSeconds(updateStep);
        }
    }

    private void AdjustCanvasScaler() {
        _screenWidthLast = Screen.width;
        _screenHeightLast = Screen.height;
        _aspectRatio = (float)Screen.width / Screen.height;
        _canvasScaler.matchWidthOrHeight = (_aspectRatio < targetAspectRatioThreshold)
            ? 0f  // set widths based
            : 1f; // set height based
    }
}