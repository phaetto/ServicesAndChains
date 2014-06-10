namespace Services.UI.Dialogs
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    public partial class SelectAssembly : Form
    {
        private readonly string[] files;

        public string SelectedFile
        {
            get
            {
                return assemblyComboBox.SelectedItem.ToString();
            }
        }

        public SelectAssembly(string[] files)
        {
            this.files = files.Select(Path.GetFileName).ToArray();
            InitializeComponent();
        }

        private void SelectAssembly_Load(object sender, EventArgs e)
        {
            assemblyComboBox.Items.AddRange(files);
        }
    }
}
