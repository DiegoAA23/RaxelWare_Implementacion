using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_ManejoInventario_
{
    public partial class ReportesAgregar : Form
    {
        
        public ReportesAgregar()
        {
            InitializeComponent();
        }

        /*Funciones que permiten el control de las ventanas, para que el usuario pueda 
        moverlas libremente*/
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        /*Funciones que agregan una sombra al formulario, para resaltarlo mejor en
        pantalla*/
        private const int SombraForm = 0x20000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= SombraForm;
                return cp;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Instancia de formulario
            ReporteVentas rv = new ReporteVentas();
            rv.Show();
            this.Enabled = false;
            rv.FormClosing += new FormClosingEventHandler(this.ReporteVentas_FormClosing);
        }

        /*Funcion que espera al cierre del formulario Reporte de Ventas, para poder
        activar de nuevo el formulario*/
        private void ReporteVentas_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //Instancia de formulario
            ReporteInventario ri = new ReporteInventario();
            ri.Show();
            this.Enabled = false;
            ri.FormClosing += new FormClosingEventHandler(this.ReporteInventario_FormClosing);
        }

        /*Funcion que espera al cierre del formulario Reporte de Inventario, para poder
         activar de nuevo el formulario*/
        private void ReporteInventario_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = true;
        }

        private void ReportesAgregar_Load(object sender, EventArgs e)
        {

        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnNormal.Visible = false;
            BtnMaximizar.Visible = true;
        }

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //Permite el movimiento libre del formulario
        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnNormal.Visible = true;
            BtnMaximizar.Visible = false;
        }
    }
}
