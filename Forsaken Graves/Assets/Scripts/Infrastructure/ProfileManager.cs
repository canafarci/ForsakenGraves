using System;

namespace ForsakenGraves.Infrastructure
{
    public class ProfileManager
    {
        private const string PROFILE_DATA_PATH = "PROFILE_DATA_PATH";
        private const string PROFILE_ID_KEY = "PROFILE_ID_KEY";
        private string _profileID;
        
        public ProfileManager()
        {
#if !UNITY_EDITOR //different id for each editor instance            
            _profileID = ES3.Load(PROFILE_ID_KEY, PROFILE_DATA_PATH, CreateGUID());
            ES3.Save(PROFILE_ID_KEY, _profileID, PROFILE_DATA_PATH);
#else
            _profileID = CreateGUID();
#endif
        }

        public string GetUniqueProfileID() => _profileID;

        private string CreateGUID()
        {
            Guid uniqueID = Guid.NewGuid();
            string idText = uniqueID.ToString();
            //authentication service requires names to be 30 charaters long and GUIDs are 36 characters,
            //so remove the last 6 characters
            string slicedID = idText[..30];
            
            return slicedID;
        }
    }
}