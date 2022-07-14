namespace Core.Services
{
    public interface IPlayerPrefsService : IService
    {
        bool HasKey(string key);
        void Save();
        
        object GetValue(string key, object defaultValue = null);
        
        float GetFloat(string key, float defaultValue = 0f);
        int GetInt(string key, int defaultValue = 0);
        string GetString(string key, string defaultValue = "");
        
        void SetFloat(string key, float value);
        void SetInt(string key, int value);
        void SetString(string key, string value);
                
        bool GetBool(string key, bool defaultValue = false);
        void SetBool(string key, bool value);
        
        bool SetField(string key, ref bool field, bool value);
        bool SetField(string key, ref int field, int value);
        bool SetField(string key, ref float field, float value);
        bool SetField(string key, ref string field, string value);
    }
}