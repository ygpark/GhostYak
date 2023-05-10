using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhostYak.Windows.Forms
{
    public class SubFormHelper
    {
        public static void Init(System.Windows.Forms.Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.BringToFront();
            form.Show();
        }
    }
}
