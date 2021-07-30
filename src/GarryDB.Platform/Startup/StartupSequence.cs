using System;
using System.Reactive.Subjects;

using GarryDB.Platform.Plugins;

namespace GarryDB.Platform.Startup
{
    /// <summary>
    ///     The sequence of steps to take before Garry is started.
    /// </summary>
    public sealed class StartupSequence : IObservable<StartupProgressUpdated>
    {
        private const int NumberOfStages = 4;

        private readonly Subject<StartupProgressUpdated> progress;
        private int currentStep;
        private int numberOfPlugins;

        /// <summary>
        ///     Intializes a new <see cref="StartupSequence" />.
        /// </summary>
        public StartupSequence()
        {
            progress = new Subject<StartupProgressUpdated>();
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<StartupProgressUpdated> observer)
        {
            return progress.Subscribe(observer);
        }

        internal void Inspect(PluginIdentity pluginIdentity)
        {
            numberOfPlugins++;
            ReportProgress("Inspecting", pluginIdentity);
        }

        internal void Load(PluginIdentity pluginIdentity)
        {
            ReportProgress("Loading", pluginIdentity);
        }

        internal void Configure(PluginIdentity pluginIdentity)
        {
            ReportProgress("Configuring", pluginIdentity);
        }

        internal void Start(PluginIdentity pluginIdentity)
        {
            ReportProgress("Starting", pluginIdentity);
        }

        internal void Complete()
        {
            progress.OnCompleted();
        }

        private void ReportProgress(string stage, PluginIdentity pluginIdentity)
        {
            currentStep++;
            progress.OnNext(new StartupProgressUpdated(stage, pluginIdentity, currentStep, numberOfPlugins * NumberOfStages));
        }
    }
}
