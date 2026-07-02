namespace BOTB64.Runtime
{
    public static class LocalPlayerIdentity
    {
        public static int PlayerId { get; private set; }
        public static string DisplayName { get; private set; } = "Player";

        public static void Init()
        {
            // For now: generate a random-ish stable ID per session.
            // Later: load from a saved settings file so it persists across launches,
            // and matches whatever account/auth system you eventually add.
            PlayerId = Environment.TickCount ^ Guid.NewGuid().GetHashCode();
        }

        public static void SetDisplayName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                DisplayName = name.Trim();
        }
    }
}