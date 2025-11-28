using UnityEngine.EventSystems;

namespace MVVMToolkit.UI
{
    public enum PointerEventAction
    {
        Click,
        Down,
        Up
    }

    public class ExtendedPointerEventData
    {
        public PointerEventData Data { get; private set; }
        public PointerEventAction Action { get; private set; }

        public ExtendedPointerEventData(PointerEventData data, PointerEventAction action)
        {
            Data = data;
            Action = action;
        }
    }
}
