using System;
using MVVMToolkit.DataBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    public enum BoolGrayscaleTarget
    {
        Itself,
        OriginGameObject,
        ListOfImages,
        TextMeshProComponents,
        OriginGameObjectTextMeshPro
    }

    public class BoolGrayscaleBinder : Binder
    {
        [SerializeField]
        private BoolGrayscaleTarget target = BoolGrayscaleTarget.Itself;

        [SerializeField]
        private Material positiveMaterial;

        [SerializeField]
        private Material negativeMaterial;

        [Tooltip("GameObject to search for child Image or TextMeshPro components (depending on target mode).")]
        [SerializeField]
        private GameObject originGO;

        [Tooltip("List of images to change material for.")]
        [SerializeField]
        private Image[] images;

        [Tooltip("List of TextMeshPro components to change color for.")]
        [SerializeField]
        private TMP_Text[] textComponents;

        private Color[] originalTextColors;
        private bool[] originalOverrideTags;
        
        private System.Collections.Generic.Dictionary<TMP_Text, (Color color, bool overrideTags)> dynamicTextOriginals;

        protected override bool StartSetFallbackValue()
        {
            if (!base.StartSetFallbackValue())
                return false;

            fallbackValue = false.ToString();
            return true;
        }

        public override void InitValue(object value)
        {
            base.InitValue(value);
            
            if (target == BoolGrayscaleTarget.TextMeshProComponents && textComponents != null)
            {
                InitializeOriginalTextColors();
            }
            else if (target == BoolGrayscaleTarget.OriginGameObjectTextMeshPro)
            {
                dynamicTextOriginals = new System.Collections.Generic.Dictionary<TMP_Text, (Color, bool)>();
            }
            
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value != null)
            {
                try
                {
                    if (bool.TryParse(value.ToString(), out bool val))
                    {
                        switch (target)
                        {
                            case BoolGrayscaleTarget.Itself:
                            {
                                Image[] result = GetComponentsInChildren<Image>();
                                SetMaterials(result, val);
                            }

                            break;
                            case BoolGrayscaleTarget.OriginGameObject:
                            {
                                if (originGO == null)
                                {
                                    Debug.LogError($"[{GetType().Name}] Failed - gameObject: {gameObject.name} - Trying to use origin game object but it is null!");
                                    return;
                                }

                                Image[] result = originGO.GetComponentsInChildren<Image>();
                                SetMaterials(result, val);
                            }

                            break;
                            case BoolGrayscaleTarget.ListOfImages:
                            {
                                if (images == null || images.Length == 0)
                                {
                                    Debug.LogError($"[{GetType().Name}] Failed - gameObject: {gameObject.name} - Trying to use array of images but it is either null or empty!");
                                    return;
                                }

                                SetMaterials(images, val);
                            }

                            break;
                            case BoolGrayscaleTarget.TextMeshProComponents:
                            {
                                if (textComponents == null || textComponents.Length == 0)
                                {
                                    Debug.LogError($"[{GetType().Name}] Failed - gameObject: {gameObject.name} - Trying to use array of TextMeshPro components but it is either null or empty!");
                                    return;
                                }

                                SetTextColors(textComponents, val);
                            }

                            break;
                            case BoolGrayscaleTarget.OriginGameObjectTextMeshPro:
                            {
                                if (originGO == null)
                                {
                                    Debug.LogError($"[{GetType().Name}] Failed - gameObject: {gameObject.name} - Trying to use origin game object for TextMeshPro but it is null!");
                                    return;
                                }

                                TMP_Text[] result = originGO.GetComponentsInChildren<TMP_Text>();
                                SetTextColorsFromGameObject(result, val);
                            }

                            break;
                        }
                    }
                    else
                    {
                        Debug.LogError($"[{GetType().Name}] Failed - gameObject: {gameObject.name} - invalid value type: '{value?.GetType()}' ({value})!");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[{GetType().Name}] Failed - gameObject: {gameObject.name}, exception: {ex}");
                }
            }
        }

        private void SetMaterials(Image[] images, bool val)
        {
            foreach (Image item in images)
                item.material = val ? positiveMaterial : negativeMaterial;
        }

        private void SetTextColors(TMP_Text[] textComponents, bool val)
        {
            if (originalTextColors == null || originalTextColors.Length != textComponents.Length)
            {
                InitializeOriginalTextColors();
            }

            for (int i = 0; i < textComponents.Length; i++)
            {
                if (textComponents[i] != null)
                {
                    if (!val)
                    {
                        textComponents[i].color = originalTextColors[i];
                        textComponents[i].overrideColorTags = originalOverrideTags[i];
                    }
                    else
                    {
                        Color originalColor = originalTextColors[i];
                        float gray = originalColor.r * 0.299f + originalColor.g * 0.587f + originalColor.b * 0.114f;
                        textComponents[i].color = new Color(gray, gray, gray, originalColor.a);
                        textComponents[i].overrideColorTags = true;
                    }
                }
            }
        }

        private void InitializeOriginalTextColors()
        {
            if (textComponents != null)
            {
                originalTextColors = new Color[textComponents.Length];
                originalOverrideTags = new bool[textComponents.Length];
                for (int i = 0; i < textComponents.Length; i++)
                {
                    if (textComponents[i] != null)
                    {
                        originalTextColors[i] = textComponents[i].color;
                        originalOverrideTags[i] = textComponents[i].overrideColorTags;
                    }
                }
            }
        }

        private void SetTextColorsFromGameObject(TMP_Text[] textComponents, bool val)
        {
            if (textComponents == null || textComponents.Length == 0)
                return;

            if (dynamicTextOriginals == null)
            {
                dynamicTextOriginals = new System.Collections.Generic.Dictionary<TMP_Text, (Color, bool)>();
            }

            foreach (TMP_Text textComponent in textComponents)
            {
                if (textComponent != null && !dynamicTextOriginals.ContainsKey(textComponent))
                {
                    dynamicTextOriginals[textComponent] = (textComponent.color, textComponent.overrideColorTags);
                }
            }

            foreach (TMP_Text textComponent in textComponents)
            {
                if (textComponent != null && dynamicTextOriginals.ContainsKey(textComponent))
                {
                    if (!val)
                    {
                        (Color color, bool overrideTags) original = dynamicTextOriginals[textComponent];
                        textComponent.color = original.color;
                        textComponent.overrideColorTags = original.overrideTags;
                    }
                    else
                    {
                        (Color color, bool overrideTags) original = dynamicTextOriginals[textComponent];
                        float gray = original.color.r * 0.299f + original.color.g * 0.587f + original.color.b * 0.114f;
                        textComponent.color = new Color(gray, gray, gray, original.color.a);
                        textComponent.overrideColorTags = true;
                    }
                }
            }
        }
    }
}
