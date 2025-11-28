namespace MVVMToolkit.UI
{
    public struct WindowOpenData
    {
        public WindowType Type { get; }
        public object Data { get; }
        public bool UseAnimation { get; }
        public bool OpenAsPrimary { get; }

        public WindowOpenData(WindowType type, object data = null, bool useAnimation = false, bool openAsPrimary = false)
        {
            Type = type;
            Data = data;
            UseAnimation = useAnimation;
            OpenAsPrimary = openAsPrimary;
        }
    }
}
