using System.Collections;
using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorBoolBinder : Binder
    {
        // Debt: This sould be made as a AnimatorBinder class and inherited, since all animator binders should support these parameters!
        // it's also in AnimatorIntBinder duplicit
        [SerializeField] private bool applyDefaultValue = false;
        [SerializeField] private bool defaultValue = false;
        [SerializeField] private bool useDelay = false;
        [SerializeField] private float transitionDelay = 0f;

        private Animator animator;
        public Animator Animator => animator ?? (animator = GetComponent<Animator>());

        private Coroutine delayCoroutine;

        protected override void Awake()
        {
            base.Awake();

            if (applyDefaultValue)
                Animator.SetBool(propertyName, defaultValue);
        }

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value != null)
            {
                if (bool.TryParse(value.ToString(), out bool result))
                {
                    if (useDelay)
                    {
                        if (delayCoroutine != null)
                        {
                            StopCoroutine(delayCoroutine);
                            delayCoroutine = null;
                        }

                        delayCoroutine = StartCoroutine(DelayTransition(result));
                    }
                    else
                        Animator.SetBool(propertyName, result);
                }
                else
                {
                    Debug.LogError($"[{GetType().Name}] value is in unsupported format! Path: {Path}, value: {value.GetType()}");
                }
            }
        }

        private IEnumerator DelayTransition(bool result)
        {
            yield return new WaitForSeconds(transitionDelay);

            Animator.SetBool(propertyName, result);
        }
    }
}
