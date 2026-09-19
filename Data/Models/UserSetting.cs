namespace NetworkSharp.Data.Models
{
    /// <summary>
    /// Represents a user setting
    /// </summary>
    public class UserSetting
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
