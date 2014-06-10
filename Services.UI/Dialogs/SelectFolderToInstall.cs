namespace Services.UI.Dialogs
{
    using System;
    using System.Windows.Forms;

    public partial class SelectFolderToInstall : Form
    {
        public string ServiceName
        {
            get
            {
                return serviceNameTextBox.Text;
            }
        }

        public string FolderSelected
        {
            get
            {
                return folderTextBox.Text;
            }
        }

        public SelectFolderToInstall()
        {
            InitializeComponent();
        }

        private void SelectFolderToInstall_Load(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folderTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
