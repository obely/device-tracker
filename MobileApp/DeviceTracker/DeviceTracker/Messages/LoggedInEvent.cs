namespace DeviceTracker.Messages
{
    public class LoggedInEvent
    {
        public string Name { get; }
        public string Email { get; }
        public string Picture { get; }

        public LoggedInEvent(string name, string email, string picture)
        {
            Name = name;
            Email = email;
            Picture = picture;
        }
    }
}
