namespace ForsakenGraves.PreGame.Data
{
    public class AvatarSelectModel
    {
        private const string AVATAR_DATA_PATH = "AVATAR_DATA";
        private const string AVATAR_INDEX_KEY = "AVATAR_INDEX";
        private const int DEFAULT_AVATAR_VALUE = 0;
        
        public int AvatarIndex { get; private set; }
        
        public AvatarSelectModel()
        {
            AvatarIndex = ES3.Load(AVATAR_INDEX_KEY, AVATAR_DATA_PATH, DEFAULT_AVATAR_VALUE);
        }

        public void ChangeAvatarIndex(int index)
        {
            AvatarIndex = index;
            ES3.Save(AVATAR_INDEX_KEY, AvatarIndex, AVATAR_DATA_PATH);
        }
    }
}