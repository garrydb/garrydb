using System.Diagnostics;

using Autofac;

namespace GarryDB.UI.Modules
{
    public class UIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Debug.WriteLine("Loading Autofac module");
        }
    }
}
