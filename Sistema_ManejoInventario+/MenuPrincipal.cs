using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace Sistema_ManejoInventario_
{
    public partial class MenuPrincipal : Form
    {
        //Instancias de conexion a la BD
        Conexion conexion = new Conexion();
        SqlCommand cmd;
        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
     
            BtnMaximizar.Visible = true;
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnNormal.Visible = true;
            BtnMaximizar.Visible = false;
            
            
        }

        /*Funcion que define los elementos visibles en pantalla
         dependiendo del nivel de usuario que ha ingresado al sistema*/
        private void MenuPrincipal_Load(object sender, EventArgs e)
        {
            AbrirFormHijo(new VentanaMenuPrincipal());
            if (conexion.Codigo != 1) {
                BtnReporte.Visible = false;
                BtnVentas.Visible = false;
                panel6.Visible = false;
                panel8.Visible = false;
            }
            
        }

        /*Funciones que permiten el control de las ventanas, para que el usuario pueda 
        moverlas libremente*/
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PanelCentral_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MenuPrincipal_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /*Funcion que permite redimensionar los formularios dentro del panel del
        menu dependiendo de la opcion seleccionada*/
        private void AbrirFormHijo(object formhijo)
        {
            if (this.PanelCentro.Controls.Count > 0)
                this.PanelCentro.Controls.RemoveAt(0);
            Form fh = formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.PanelCentro.Controls.Add(fh);
            this.PanelCentro.Tag = fh;
            fh.Show();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (conexion.Codigo == 1) {
                AbrirFormHijo(new Inventario());
            }
            else
            {
                AbrirFormHijo(new InventarioEmpleado());
            }
        }

        private void PanelContenedor_Paint(object sender, PaintEventArgs e)
        {
            
        }

        /*Apertura de los formularios dependiendo de la opcion seleccionada*/
        private void BtnVentas_Click(object sender, EventArgs e)
        {
            AbrirFormHijo(new Ventas());
        }

        private void BtnReporte_Click(object sender, EventArgs e)
        {
            AbrirFormHijo(new CrearReporte());
        }

        private void BtnMenuPrincipal_Click(object sender, EventArgs e)
        {
            AbrirFormHijo(new VentanaMenuPrincipal());
        }

        private void horafecha_Tick(object sender, EventArgs e)
        {
          
        }

        private void lblHora_Click(object sender, EventArgs e)
        {

        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnNormal.Visible = false;
            BtnMaximizar.Visible = true;
        }

        //Permite el movimiento libre del formulario
        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BarraTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PanelCentro_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
