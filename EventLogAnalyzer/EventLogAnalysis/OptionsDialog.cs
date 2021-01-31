using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventLogAnalysis
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
        }

        public static void ShowOptions(object options, Form parentform)
        {
            var form = new OptionsDialog();
            form.propertyGrid.SelectedObject = options;
            if (parentform == null)
            {
                form.Show();
            }
            else
            {
                form.ShowDialog(parentform);
            }
        }

        private void OptionsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Options.Instance.OnCloseOptionsForm();
        }

        private void OptionsDialog_Load(object sender, EventArgs e)
        {
        }
    }
}