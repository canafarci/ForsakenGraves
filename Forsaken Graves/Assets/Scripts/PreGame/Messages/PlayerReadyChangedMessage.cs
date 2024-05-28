namespace ForsakenGraves.PreGame.Signals
{
    public readonly struct PlayerReadyChangedMessage
    {
        public bool IsReady { get; }
        public ulong ClientID { get; }

        public PlayerReadyChangedMessage(bool isReady, ulong clientID)
        {
            IsReady = isReady;
            ClientID = clientID;
        }
    }
}