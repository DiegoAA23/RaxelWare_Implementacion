using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Contexts;
using System.Runtime.InteropServices;

namespace Sistema_ManejoInventario_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Instancias para conexion a la Base de Datos
        Conexion conexion = new Conexion();
        SqlCommand cmd;


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

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            if(txtUsuario.Text == String.Empty)
            {
                MessageBox.Show("Ingrese un Nombre de Usuario", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorProvider1.SetError(txtUsuario, "Campo Obligatorio");
            }
            else if(TxtContraseña.Text == String.Empty)
            {
                MessageBox.Show("Ingrese una Contraseña", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorProvider1.SetError(TxtContraseña, "Campo Obligatorio");
            }
            else
            {

                /*Comprobacion con la Base de Datos para verificar la existencia del usuario
                 y el nivel de acceso del mismo*/

                conexion.abrir();
                string consulta = "SELECT * FROM Usuarios WHERE Nombre COLLATE Latin1_General_CS_AS = '" + txtUsuario.Text + "' COLLATE Latin1_General_CS_AS AND Contrasena COLLATE Latin1_General_CS_AS = '" + TxtContraseña.Text + "' COLLATE Latin1_General_CS_AS";
                SqlCommand comando = new SqlCommand(consulta, conexion.conectardb);
                SqlDataReader lector;
                lector = comando.ExecuteReader();

                if (lector.HasRows == true)
                {
                    conexion.cerrar();
                    conexion.abrir();
                    cmd = new SqlCommand(consulta, conexion.conectardb);
                    SqlDataAdapter user = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    user.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            conexion.Codigo = 1;
                        }

                        MenuPrincipal menu = new MenuPrincipal();
                        menu.Show();
                        this.Hide();
                    }
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrecto.");
                    txtUsuario.Text = "";
                    TxtContraseña.Text = "";
                    txtUsuario.Focus();
                    errorProvider1.Clear();
                    errorProvider2.Clear();
                }
                conexion.cerrar();

            }
        }


        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        //Permite el movimiento del formulario
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }
}
