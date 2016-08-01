using Starcounter;
using System;
using Simplified.Ring3;
using Simplified.Ring2;
using System.Linq;

namespace Cashier {
    public class MainHandlers {
        public void Register() {
            /* Launcher/Launchpad specific handlers */
            Handle.GET("/cashier/app-name", () => { return new AppName(); });

            Handle.GET("/cashier/menu", () => {
                return new Page() { Html = "/Cashier/viewmodels/MenuPage.html" };
            });

            Handle.GET("/cashier/standalone", () => {
                Session session = Session.Current;

                if (session != null && session.Data != null)
                    return session.Data;

                var standalone = new StandalonePage();

                if (session == null) {
                    session = new Session(SessionOptions.PatchVersioning);
                    standalone.Html = "/Cashier/viewmodels/StandalonePage.html";
                }

                standalone.Session = session;
                return standalone;
            });
            
            Handle.GET("/cashier/shop/{?}", (string objectId) => {
                StandalonePage master = (StandalonePage)Self.GET("/Cashier/standalone");
                master.CurrentPage = Db.Scope<Json>(() => Self.GET<ShopPage>("/Cashier/partials/shop/" + objectId));
                return master;
            });

            Handle.GET("/cashier/settings", () => {
                StandalonePage master = (StandalonePage)Self.GET("/Cashier/standalone");

                master.CurrentPage = Db.Scope<Json>(() => {
                    return Self.GET<SettingsPage>("/Cashier/partials/settings");
                });

                return master;
            });

            Handle.GET("/cashier", () => {
                Organization shop = Helpers.Organizations.GetMyShops().FirstOrDefault();
                StandalonePage master = (StandalonePage)Self.GET("/Cashier/standalone");
                
                if(shop != null)
                    master.CurrentPage = Self.GET("/cashier/partials/shop/" + shop.Key);

                return master;
            });

            Handle.GET("/cashier/partials/shop/{?}", (string objectId) => {
                return Db.Scope(() => {
                    var shop = (Organization) DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                    var page = new ShopPage() { Data = shop };

                    return page;
                });
            });

            Handle.GET("/cashier/partials/settings", () => {
                var page = new SettingsPage();

                page.Data = (Settings)Db.SQL("SELECT s FROM Cashier.Settings s").First;

                return page;
            });

        }
    }
}
