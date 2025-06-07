using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MotionFade;
using System.Collections.Generic;
using UnityEngine;

namespace Live2D.Cubism.Framework.Expression
{
    /// <summary>
    /// Expression controller with keyboard control.
    /// </summary>
    public class CubismExpressionControllerWithKeyboard : MonoBehaviour, ICubismUpdatable
    {
        #region variable

        /// <summary>
        /// Expressions data list.
        /// </summary>
        [SerializeField]
        public CubismExpressionList ExpressionsList;

        /// <summary>
        /// Use the expression calculation method.
        /// </summary>
        [SerializeField]
        public bool UseLegacyBlendCalculation = false;

        /// <summary>
        /// CubismModel cache.
        /// </summary>
        private CubismModel _model = null;

        /// <summary>
        /// Playing expressions.
        /// </summary>
        private List<CubismPlayingExpression> _playingExpressions = new List<CubismPlayingExpression>();

        /// <summary>
        /// Playing expressions index.
        /// </summary>
        [SerializeField]
        public int CurrentExpressionIndex = -1;

        /// <summary>
        /// Last playing expressions index.
        /// </summary>
        private int _lastExpressionIndex = -1;

        /// <summary>
        /// Model has update controller component.
        /// </summary>
        [HideInInspector]
        public bool HasUpdateController { get; set; }

        /// <summary>
        /// Value of each parameter to be applied to the model.
        /// </summary>
        private List<CubismExpressionParameterValue> _expressionParameterValues = new List<CubismExpressionParameterValue>();

        /// <summary>
        /// Default value for applying additive.
        /// </summary>
        private const float DefaultAdditiveValue = 0.0f;

        /// <summary>
        /// Initial value of multiply applied.
        /// </summary>
        private const float DefaultMultiplyValue = 1.0f;

        #endregion

        /// <summary>
        /// Add new expression to playing expressions.
        /// </summary>
        private void StartExpression()
        {
            // Fail silently...
            if (ExpressionsList == null || ExpressionsList.CubismExpressionObjects == null)
            {
                return;
            }

            // Backup expression.
            _lastExpressionIndex = CurrentExpressionIndex;

            // Set last expression end time
            if (_playingExpressions.Count > 0)
            {
                var playingExpression = _playingExpressions[_playingExpressions.Count - 1];
                playingExpression.ExpressionEndTime = playingExpression.ExpressionUserTime + playingExpression.FadeOutTime;
                _playingExpressions[_playingExpressions.Count - 1] = playingExpression;
            }

            // Fail silently...
            if (CurrentExpressionIndex < 0 || CurrentExpressionIndex >= ExpressionsList.CubismExpressionObjects.Length)
            {
                return;
            }

            var newPlayingExpression = CubismPlayingExpression.Create(_model, ExpressionsList.CubismExpressionObjects[CurrentExpressionIndex]);

            if (newPlayingExpression == null)
            {
                return;
            }

            // Add to PlayingExList.
            _playingExpressions.Add(newPlayingExpression);
        }

        /// <summary>
        /// Called by cubism update controller. Order to invoke OnLateUpdate.
        /// </summary>
        public int ExecutionOrder
        {
            get { return CubismUpdateExecutionOrder.CubismExpressionController; }
        }

        /// <summary>
        /// Called by cubism update controller. Needs to invoke OnLateUpdate on Editing.
        /// </summary>
        public bool NeedsUpdateOnEditing
        {
            get { return false; }
        }

        /// <summary>
        /// Called by cubism update manager.
        /// </summary>
        public void OnLateUpdate()
        {
            // Fail silently...
            if (!enabled || _model == null)
            {
                return;
            }

            // Start expression when current expression changed.
            if (CurrentExpressionIndex != _lastExpressionIndex)
            {
                StartExpression();
            }

            // Update of expressions.
            if (UseLegacyBlendCalculation)
            {
                UpdateExpressionLegacy();
            }
            else
            {
                UpdateExpression();
            }
        }

        /// <summary>
        /// Update of expressions. (old method)
        /// </summary>
        private void UpdateExpressionLegacy()
        {
            // Implementation of legacy expression update logic.
        }

        /// <summary>
        /// Update of expressions.
        /// </summary>
        private void UpdateExpression()
        {
            // Implementation of expression update logic.
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void OnEnable()
        {
            _model = this.FindCubismModel();

            // Get cubism update controller.
            HasUpdateController = (GetComponent<CubismUpdateController>() != null);
        }

        /// <summary>
        /// Called by Unity every frame.
        /// </summary>
        private void Update()
        {
            // Check for keyboard input to change expressions.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CurrentExpressionIndex = 0; // Set to the first expression.
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CurrentExpressionIndex = 1; // Set to the second expression.
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CurrentExpressionIndex = 2; // Set to the third expression.
            }
            // Add more keys as needed for other expressions.
        }

        /// <summary>
        /// Called by Unity.
        /// </summary>
        private void LateUpdate()
        {
            if (!HasUpdateController)
            {
                OnLateUpdate();
            }
        }
    }
}
