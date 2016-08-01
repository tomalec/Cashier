using System;
using Simplified.Ring2;
using Starcounter;

namespace Cashier {
    partial class SettingsPage : Page, IBound<Settings> {

        protected override void OnData() {
            base.OnData();

            if (this.Data != null) {
                if (this.Data.OwnOrganization != null) {
                    this.OwnOrganizationSearch = this.Data.OwnOrganization.Name;
                }
            }
        }

        void Handle(Input.Save action) {
            Transaction.Commit();
        }

        public void Handle(Input.OwnOrganizationSearch action) {
            var searchTerm = action.Value == "*" ? "%" : $"%{action.Value}%";
            this.FoundOwnOrganizations = Db.SQL("SELECT o FROM Simplified.Ring2.Organization o WHERE Name LIKE ?", searchTerm);
            foreach (var OwnOrganization in FoundOwnOrganizations) {
                OwnOrganization.SelectAction = () => {
                    FoundOwnOrganizations.Clear();
                    OwnOrganizationSearch = OwnOrganization.Name;
                    Data.OwnOrganization = OwnOrganization.Data;
                };
            }
        }

        public void Handle(Input.ClearOwnOrganization _) {
            this.OwnOrganizationSearch = "";
            this.Data.OwnOrganization = null;
        }

        public void Handle(Input.ClearOwnOrganizations _) {
            this.FoundOwnOrganizations.Clear();
        }

        [SettingsPage_json.FoundOwnOrganizations]
        public partial class FoundOwnOrganizationsItem : Page, IBound<Organization> {
            public Action SelectAction { get; set; }
            void Handle(Input.Select action) => SelectAction();
        }
    }
}
