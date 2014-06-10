namespace Services.UI.Dialogs
{
    using System;
    using System.Windows.Forms;
    using Chains.Play.Web;
    using Services.Communication.Protocol;

    public partial class Connect : Form
    {
        public Connect()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, System.EventArgs e)
        {
            if (Main.AdminConnection != null)
            {
                try
                {
                    Main.AdminConnection.Close();
                }
                catch
                {
                }
            }

            try
            {
                Main.AdminConnection = new Client(hostTextBox.Text, int.Parse(portNumericUpDown.Text)).Do(new OpenConnection());
                Close();
            }
            catch (Exception ex)
            {
                connectErrorLabel.Text = "Could not connect to server! Error: " + ex.Message;
                connectErrorLabel.Visible = true;
            }
        }
    }
}
