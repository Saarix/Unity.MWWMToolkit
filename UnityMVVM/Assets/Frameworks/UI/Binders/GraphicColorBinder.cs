using MVVMToolkit.DataBinding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    public enum GraphicComponent
    {
        Any,
        Text,
        Image,
        Outline,
        Shadow
    }

    public class GraphicColorBinder : Binder<Graphic>
    {
        [SerializeField]
        private GraphicComponent specificGraphicComponent = GraphicComponent.Any;

        public new Graphic Component
        {
            get
            {
                if (component == null)
                {
                    if (specificGraphicComponent == GraphicComponent.Any)
                    {
                        return component = GetComponent<Graphic>();
                    }
                    else
                    {
                        switch (specificGraphicComponent)
                        {
                            case GraphicComponent.Text:
                                return component = GetComponent<TextMeshProUGUI>();
                            case GraphicComponent.Image:
                                return component = GetComponent<Image>();
                        }
                    }
                }

                return component;
            }
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
                if (value is Color color)
                    Component.color = color;
                else if (ColorUtility.TryParseHtmlString(value.ToString(), out Color htmlColor))
                    Component.color = htmlColor;
                else
                    Debug.LogError($"Unable to parse color: '{value}'");
            }
        }
    }
}
