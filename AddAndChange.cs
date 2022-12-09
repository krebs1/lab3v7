using System;
using System.Windows.Forms;

namespace lab3v7
{
    public partial class AddAndChange : Form
    {
        public int RecordId;
        
        public AddAndChange()
        {
            InitializeComponent();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                switch (radioButton.Text)
                {
                    case "Списаное по другой причине":
                        DecomReason.Enabled = true;
                        DecomReason.Text = "";
                        break;
                    case "Списаное по времени":
                        DecomReason.Enabled = false;
                        DecomReason.Text = "";
                        break;
                }
            }     
        }
    }
}