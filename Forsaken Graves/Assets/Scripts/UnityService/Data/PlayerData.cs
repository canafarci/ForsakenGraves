namespace ForsakenGraves.UnityService.Data
{
    public struct PlayerData
    {
        public bool IsHost { get; set; }
        public string DisplayName { get; set; }
        public string ID { get; set; }

        public PlayerData(bool isHost, string displayName, string id)
        {
            IsHost = isHost;
            DisplayName = displayName;
            ID = id;
        }
    }
}