using Umbraco.Core.Services;
using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using umbraco.interfaces;
using umbraco.BusinessLogic;
using umbraco.cms.presentation.Trees;
using Umbraco.Core;
using Umbraco.Web.UI;
using Umbraco.Web.UI.Pages;
using Action = Umbraco.Web.LegacyActions.Action;

namespace umbraco.cms.presentation.user
{
    public partial class EditUserType : UmbracoEnsuredPage
    {
        public EditUserType()
        {
            CurrentApp = Constants.Applications.Users.ToString();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            pnlUmbraco.Text = Services.TextService.Localize("usertype");

            var save = pnlUmbraco.Menu.NewButton();
            save.Click += save_Click;
            save.ID = "save";
            save.ToolTip = Services.TextService.Localize("save");
            save.Text = Services.TextService.Localize("save");

            pp_alias.Text = Services.TextService.Localize("usertype") + " " + Services.TextService.Localize("alias");
            pp_name.Text = Services.TextService.Localize("usertype") + " " + Services.TextService.Localize("name");

            pp_rights.Text = Services.TextService.Localize("default") + " " + Services.TextService.Localize("rights");
            
            //ensure we have a query string
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
                return;
            //ensuer it is an integer
            if (!int.TryParse(Request.QueryString["id"], out m_userTypeID))
                return;

			if (!IsPostBack)
			{
				BindActions();

				ClientTools
					.SetActiveTreeType(TreeDefinitionCollection.Instance.FindTree<UserTypes>().Tree.Alias)
					.SyncTree(m_userTypeID.ToString(), false);
			}

        }

        void save_Click(object sender, EventArgs e)
        {
            UserType userType = CurrentUserType;
            userType.Name = txtUserTypeName.Text;
            string actions = "";

            foreach (ListItem li in cbl_rights.Items) {
                if (li.Selected)
                    actions += li.Value;
            }

            userType.DefaultPermissions = actions;
            userType.Save();

            ClientTools.ShowSpeechBubble(SpeechBubbleIcon.Save, Services.TextService.Localize("speechBubbles/editUserTypeSaved"), "");
        }

        protected List<IAction> CurrentUserTypeActions
        {
            get
            {
                if (m_userTypeActions == null)
                    m_userTypeActions = Action.FromString(CurrentUserType.DefaultPermissions);
                return m_userTypeActions;
            }
        }

        protected UserType CurrentUserType
        {
            get
            {
                if (m_userType == null)
                    m_userType = UserType.GetUserType(m_userTypeID);
                return m_userType;
            }
        }
        private UserType m_userType;
        private List<IAction> m_userTypeActions;
        private int m_userTypeID;

        private void BindActions()
        {
            lblUserTypeAlias.Text = CurrentUserType.Alias;
            txtUserTypeName.Text = CurrentUserType.Name;
            hidUserTypeID.Value = CurrentUserType.Id.ToString();

            foreach (IAction ai in Action.GetPermissionAssignable()) {

                ListItem li = new ListItem(umbraco.ui.Text(ai.Alias, Security.CurrentUser), ai.Letter.ToString());

                if(CurrentUserTypeActions.Contains(ai))
                    li.Selected = true;

                cbl_rights.Items.Add(li);
            }
        }


        
    }
}
