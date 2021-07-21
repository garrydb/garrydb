namespace GarryDB.Platform.Messaging
{
    /// <summary>
    /// </summary>
    public static class ActorPaths
    {
        /// <summary>
        /// </summary>
        public static readonly ActorMetadata Plugins = new ActorMetadata("plugins");

        /// <summary>
        /// </summary>
        public static readonly ActorMetadata Garry = new ActorMetadata("garry", Plugins);
    }
}
