namespace GarryDB.Platform.Bootstrapping.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class BootstrapperExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bootstrapper"></param>
        public static Bootstrapper ApplyDefault(this Bootstrapper bootstrapper)
        {
            return DefaultBootstrapperModifier.Modify(bootstrapper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bootstrapper"></param>
        public static Bootstrapper ApplyAkka(this Bootstrapper bootstrapper)
        {
            return ApplyAkkaBootstrapperModifier.Modify(bootstrapper);
        }
    }
}
