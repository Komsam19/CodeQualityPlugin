using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using System.Windows.Forms;
using System;

namespace CodeQuality
{
    public class CodeQuality
    {
        // define menu constants
        const string menuHeader = "-&CodeQuality";
        const string menuMegaService = "&CodeQuality";


        public String EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            return "a string";
        }

        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            switch (MenuName)
            {
                // defines the top level menu option
                case "":
                    return menuHeader;
                // defines the submenu options
                case menuHeader:
                    string[] subMenus = { menuMegaService};
                    return subMenus;
            }
            return "";
        }
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
                switch (ItemName)
                {
                    case menuMegaService:
                        IsEnabled = true;
                        break;
                    default:
                        IsEnabled = false;
                        break;
                }
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            switch (ItemName)
            {
                case menuMegaService:
                    this.MegaService();
                    break;
            }
        }
        OleDbConnection cnn;
        OleDbCommand cmd;

        string connetionString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = 'D://MPhil CS/Semester 1/Reverse Engineering/Post Mid/Term Project/Queries.eapx';";

        private void MegaService()
        {
            string numberOfChildClassesQuery = "Select count (parentid) from t_object where t_object.ParentID > 0";
            string numberOfParentClassesQuery = "Select count (parentid) from t_object where t_object.ParentID = 0";
            string numberOfStrongEntitiesQuery = "Select count (*) from t_object where t_object.Object_ID in (Select t_attribute.Object_ID from t_attribute where t_attribute.Classifier='0') and t_object.Object_Type='Class'";
            string totalNumberOfEntitiesQuery = "Select count(*) from t_object where t_object.Object_Type = 'Class'";


            cnn = new OleDbConnection(connetionString);

            try
            {
                cnn.Open();
                cmd = new OleDbCommand(numberOfChildClassesQuery, cnn);
                decimal numberOfChildClasses = Convert.ToDecimal(cmd.ExecuteScalar());

                OleDbCommand cmd1 = new OleDbCommand(numberOfParentClassesQuery, cnn);
                decimal numberOfParentClasses = Convert.ToDecimal(cmd1.ExecuteScalar());

                OleDbCommand cmd2 = new OleDbCommand(numberOfStrongEntitiesQuery, cnn);
                decimal numberOfStrongEntities = Convert.ToDecimal(cmd2.ExecuteScalar());

                OleDbCommand cmd3 = new OleDbCommand(totalNumberOfEntitiesQuery, cnn);
                decimal totalNumberOfEntities = Convert.ToDecimal(cmd3.ExecuteScalar());

                cmd.Dispose();
                cmd1.Dispose();
                cmd2.Dispose();
                cmd3.Dispose();

                cnn.Close();

                string newLine = Environment.NewLine;

                // Hierarchical Complexity Ratio (HCR)
                decimal hcr = (numberOfChildClasses / (numberOfParentClasses + 1));

                // Strong Entity Ratio (SER)
                decimal ser = (numberOfStrongEntities / totalNumberOfEntities) * 100;

                // Check Conditions
                if (hcr >= 0.5m && ser >= 75)
                {
                    MessageBox.Show("Code quality is approximately good");
                }
                else
                {
                    MessageBox.Show("Code quality may have issues");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }
        }

        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}