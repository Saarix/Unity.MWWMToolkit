using System.Collections;
using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorIntBinder : Binder
    {
        [SerializeField] private bool applyDefaultValue = false;
        [SerializeField] private int defaultValue = 0;
        [SerializeField] private bool useDelay = false;
        [SerializeField] private float transitionDelay = 0f;

        private Animator animator;
        public Animator Animator => animator ?? (animator = GetComponent<Animator>());

        private Coroutine delayCoroutine;

        protected override void Awake()
        {
            base.Awake();

            if (applyDefaultValue)
                Animator.SetInteger(propertyName, defaultValue);
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
                if (int.TryParse(value.ToString(), out int result))
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
                    {
                        Animator.SetInteger(propertyName, result);
                    }
                }
                else
                {
                    Debug.LogError($"[{GetType().Name}] value is in unsupported format! Path: {Path}, value: {value.GetType()}");
                }
            }
        }

        private IEnumerator DelayTransition(int result)
        {
            yield return new WaitForSeconds(transitionDelay);

            Animator.SetInteger(propertyName, result);
        }
    }
}
