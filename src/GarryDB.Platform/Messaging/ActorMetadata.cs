namespace GarryDB.Platform.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ActorMetadata
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public ActorMetadata(string name, ActorMetadata? parent = null)
        {
            Name = name;
            string parentPath = (parent == null) ? "/user" : parent.Path;
            Path = $"{parentPath}/{name}";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; }
    }
}