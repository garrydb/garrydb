using GarryDB.Platform.Plugins;

namespace GarryDb.Platform
{
    /// <summary>
    ///     Is raised when the progress of the startup has changed.
    /// </summary>
    public sealed class StartupProgressUpdated
    {
        /// <summary>
        ///     Initializes a new <see cref="StartupProgressUpdated" />.
        /// </summary>
        /// <param name="stage">The name of the stage.</param>
        /// <param name="pluginIdentity">The identity of the plugin.</param>
        /// <param name="currentStep">The current step.</param>
        /// <param name="totalSteps">The total number of steps.</param>
        public StartupProgressUpdated(string stage, PluginIdentity pluginIdentity, int currentStep, int totalSteps)
        {
            Stage = stage;
            PluginIdentity = pluginIdentity;
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
        }

        /// <summary>
        ///     Gets the name of the stage.
        /// </summary>
        public string Stage { get; }

        /// <summary>
        ///     Gets the identity of the plugin.
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        ///     Gets the current step.
        /// </summary>
        public int CurrentStep { get; }

        /// <summary>
        ///     Gets the total number of steps.
        /// </summary>
        public int TotalSteps { get; }
    }
}
