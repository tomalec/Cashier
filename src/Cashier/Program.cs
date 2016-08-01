using System;
using Starcounter;

namespace Cashier {
    class Program {
        static void Main() {
            if (Db.SQL("SELECT s FROM Cashier.Settings s").First == null) {
                Db.Transact(() => {
                    new Settings();
                });
            }

            MainHandlers main = new MainHandlers();
            LauncherHooks launcher = new LauncherHooks();
            OntologyHooks ontology = new OntologyHooks();

            main.Register();
            launcher.Register();
            ontology.Register();
        }
    }
}