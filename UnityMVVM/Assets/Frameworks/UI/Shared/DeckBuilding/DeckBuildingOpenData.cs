namespace MVVMToolkit.UI
{
    public struct DeckBuildingOpenData
    {
        public int DeckId { get; }
        public int? DeckTabIndex { get; }
        public int? MiscTabIndex { get; }

        public DeckBuildingOpenData(int deckId, int? deckTabIndex, int? miscTabIndex)
        {
            DeckId = deckId;
            DeckTabIndex = deckTabIndex;
            MiscTabIndex = miscTabIndex;
        }
    }
}
