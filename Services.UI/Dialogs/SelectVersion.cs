namespace Services.UI.Dialogs
{
    using System;
    using System.Windows.Forms;
    using System.Linq;

    public partial class SelectVersion : Form
    {
        private readonly string[] versions;
        private readonly string currentVersion;

        public string SelectedValue
        {
            get
            {
                return versionsComboBox.SelectedItem.ToString();
            }
        }

        public SelectVersion(string currentVersion, string[] versions)
        {
            this.versions = versions;
            this.currentVersion = currentVersion;
            InitializeComponent();
        }

        private void SelectAssembly_Load(object sender, EventArgs e)
        {
            versionLabel.Text = currentVersion;
            versionsComboBox.Items.AddRange(
                versions.Except(new[]
                                {
                                    currentVersion
                                }).ToArray());
        }
    }
}
