using System.Collections;
using UnityEngine;

namespace kenneyjam2024 {
    [RequireComponent(typeof(Animator), typeof(CanvasGroup))]
    public class ViewBase : MonoBehaviour, IView {

        [Header("ViewBase")]
        [SerializeField] protected Animator _animator;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] private string _showTrigger = "Show";
        [SerializeField] private string _hideTrigger = "Hide";
        [SerializeField] private string _hideNowTrigger = "HideNow";
        private State _state;

        public virtual void Show() {
            SetState(State.ShowAnimation);
            OnBeforeShow();
        }

        public virtual void Hide() {
            if (_state == State.Invalid) {
                return;
            }
            SetState(State.HideAnimation);
            OnBeforeHide();
        }

        public virtual void Restart() {
            SetState(State.Restart);
        }

        // Called within Mecanim
        public void OnAnimationDone() {
            switch (_state) {
                case State.HideAnimation:
                    SetState(State.Hidden);
                    OnAfterHide();
                    break;
                case State.ShowAnimation:
                    SetState(State.Shown);
                    OnAfterShow();
                    break;
            }
        }

        private void Awake() {
            _state = State.Hidden;
            //SetState(State.Restart);
            OnAwake();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnRestart() { }
        protected virtual void OnBeforeShow() { }
        protected virtual void OnAfterShow() { }
        protected virtual void OnBeforeHide() { }
        protected virtual void OnAfterHide() { }


        #region States

        private void SetState(State state) {
            State stateLast = _state;
            if (state == _state) {
                return;
            }

            Debug.Log($"{name}.State = {state}");
            _state = state;
            switch (state) {

                case State.Shown:
                    if (stateLast == State.ShowAnimation) {
                        _canvasGroup.blocksRaycasts = true;
                    }
                    break;

                case State.ShowAnimation:
                    if (stateLast == State.Shown) {
                        break;
                    }
                    _animator.SetTrigger(_showTrigger);
                    break;

                case State.HideAnimation:
                    if (stateLast == State.Hidden) {
                        break;
                    }
                    _canvasGroup.blocksRaycasts = false;
                    _animator.SetTrigger(_hideTrigger);
                    break;

                case State.Restart:
                    _canvasGroup.blocksRaycasts = false;
                    _animator.SetTrigger(_hideNowTrigger);
                    SetState(State.Hidden);
                    OnRestart();
                    break;
            }
        }

        public enum State {
            Invalid = 0,
            Shown = 1,
            Hidden = 2,
            ShowAnimation = 3,
            HideAnimation = 4,
            Restart = 5
        }

        #endregion
    }
}