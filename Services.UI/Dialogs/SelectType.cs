namespace Services.UI.Dialogs
{
    using System;
    using System.Windows.Forms;
    using Newtonsoft.Json;

    public partial class SelectType : Form
    {
        private readonly string[] files;

        public bool IsAutoId
        {
            get
            {
                return autoIdCheckBox.Checked;
            }
        }

        public string IdText
        {
            get
            {
                return idTextBox.Text;
            }
        }

        public string ValueSelected
        {
            get
            {
                return typeComboBox.SelectedItem.ToString();
            }
        }

        public bool IsServed
        {
            get
            {
                return servedCheckBox.Checked;
            }
        }

        public string ServedHost
        {
            get
            {
                return servedHostTextBox.Text;
            }
        }

        public int ServedPort
        {
            get
            {
                return (int)servedPortNumericUpDown.Value;
            }
        }

        public object[] Parameters
        {
            get
            {
                return JsonConvert.DeserializeObject<object[]>("[" + parametersTextBox.Text + "]");
            }
        }

        public object[] Modules
        {
            get
            {
                return JsonConvert.DeserializeObject<object[]>("[" + modulesTextBox.Text + "]");
            }
        }

        public SelectType(string[] files)
        {
            this.files = files;
            InitializeComponent();
        }

        private void SelectType_Load(object sender, EventArgs e)
        {
            typeComboBox.Items.AddRange(files);
        }

        private void autoIdCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            idTextBox.Enabled = !autoIdCheckBox.Checked;
        }

        private void servedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            servedHostTextBox.Enabled = servedPortNumericUpDown.Enabled = servedCheckBox.Checked;
        }
    }
}
