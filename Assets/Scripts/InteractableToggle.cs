using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction
{
    /// <summary>
    /// Visually displays the current state of an interactable.
    /// </summary>
    public class InteractableToggle : MonoBehaviour
    {
        [Tooltip("The interactable to monitor for state changes.")]
        /// <summary>
        /// The interactable to monitor for state changes.
        /// </summary>
        [SerializeField, Interface(typeof(IInteractableView))]
        private UnityEngine.Object _interactableView;

        [Tooltip("The mesh that will change color based on the current state.")]
        /// <summary>
        /// The mesh that will change color based on the current state.
        /// </summary>
        [SerializeField]
        private Renderer _renderer;

        public Toggle ToggleUI;

        private IInteractableView InteractableView;
        private Material _material;

        protected bool _started = false;

        protected virtual void Awake()
        {
            InteractableView = _interactableView as IInteractableView;
        }


        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            this.AssertField(InteractableView, nameof(InteractableView));
            this.AssertField(_renderer, nameof(_renderer));
            _material = _renderer.material;

            UpdateVisual();
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged += UpdateVisualState;
                UpdateVisual();
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                InteractableView.WhenStateChanged -= UpdateVisualState;
            }
        }

        private void OnDestroy()
        {
            Destroy(_material);
        }

        //public void SetNormalColor(Color color)
        //{
        //    _normalColor = color;
        //    UpdateVisual();
        //}

        private void UpdateVisual()
        {
            switch (InteractableView.State)
            {
                case InteractableState.Normal:
                    break;
                case InteractableState.Hover:
                    break;
                case InteractableState.Select:
                    if (ToggleUI != null)
                        ToggleUI.isOn = ToggleUI.isOn ? false : true;
                    break;
                case InteractableState.Disabled:
                    break;
            }
        }

        private void UpdateVisualState(InteractableStateChangeArgs args) => UpdateVisual();

        #region Inject

        public void InjectAllInteractableDebugVisual(IInteractableView interactableView, Renderer renderer)
        {
            InjectInteractableView(interactableView);
            InjectRenderer(renderer);
        }

        public void InjectInteractableView(IInteractableView interactableView)
        {
            _interactableView = interactableView as UnityEngine.Object;
            InteractableView = interactableView;
        }

        public void InjectRenderer(Renderer renderer)
        {
            _renderer = renderer;
        }

        #endregion
    }
}
