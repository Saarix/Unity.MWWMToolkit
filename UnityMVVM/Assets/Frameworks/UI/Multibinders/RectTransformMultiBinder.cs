using UnityEngine;
using System;
using Geewa.Framework;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class RectTransformMultiBinder : MultiBinder
    {
        private Vector2? originSize;
        private RectTransform rectTra;
        public RectTransform RectTra => this.rectTra ??= GetComponent<RectTransform>();

        protected override void Awake()
        {
            base.Awake();

            if (!originSize.HasValue)
                originSize = RectTra.sizeDelta;
        }

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            if (value != null)
            {
                Vector2 result = Vector2.zero;
                result = result.Parse(value.ToString());

                switch (sourcePath)
                {
                    case "anchored_position":
                        RectTra.anchoredPosition = result;
                        break;
                    case "size_delta":
                        {
                            RectTra.sizeDelta = result;
                            originSize = result;
                        }

                        break;
                    case "local_scale":
                        RectTra.localScale = result;
                        break;
                    case "offset_min":
                        RectTra.offsetMin = result;
                        break;
                    case "offset_max":
                        RectTra.offsetMax = result;
                        break;
                    case "viewport_position":
                        RectTra.anchoredPosition = ViewPortPositionMap(result);
                        break;
                    case "scale_to_size":
                        RectTra.sizeDelta = ScaleToSizeDelta(result);
                        break;
                }

            }
        }

        private Vector2 ViewPortPositionMap(Vector2 viewportPos)
        {
            RectTransform parentTra = transform.parent.GetComponent<RectTransform>();
            if (parentTra == null)
                return Vector2.zero;

            Rect rect = parentTra.rect;
            float x = ((rect.xMax - rect.xMin) * viewportPos.x) + rect.xMin;
            float y = ((rect.yMax - rect.yMin) * viewportPos.y) + rect.yMin;

            return new Vector2(x, y);
        }

        private Vector2 ScaleToSizeDelta(Vector2 scale)
        {
            if (originSize.HasValue)
                return new Vector2(originSize.Value.x * scale.x, originSize.Value.y * scale.y);

            throw new NullReferenceException($"[{GetType().Name}] OriginSize is null.");
        }
    }
}
