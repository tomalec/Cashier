using Starcounter;
using System;

namespace Cashier {
    public class LauncherHooks {
        public void Register() {
            UriMapping.Map("/Cashier/menu", UriMapping.MappingUriPrefix + "/menu");
            UriMapping.Map("/Cashier/app-name", UriMapping.MappingUriPrefix + "/app-name");
            //UriMapping.Map("/Cashier/search?query={?}", UriMapping.MappingUriPrefix + "/search?query={?}");
        }
    }
}
