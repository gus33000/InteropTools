using System.Linq;
using System.Threading.Tasks;
using Plugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider;

namespace InteropTools.AppExtensibilityClient
{
    public abstract class Client
    {
        private Plugin plugin;

        public async Task<bool> Initialize()
        {
            var plugins = await InteropTools.AppExtensibilityDefinition.AppExtensibilityDefinition.ListAsync(InteropTools.AppExtensibilityDefinition.AppExtensibilityDefinition.PLUGIN_NAME);

            if (plugins.Plugins.Count() != 0)
            {
                plugin = plugins.Plugins.FirstOrDefault();
                return true;
            }

            return false;
        }
    }
}
