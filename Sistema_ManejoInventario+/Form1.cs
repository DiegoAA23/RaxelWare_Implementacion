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

namespace Sistema_ManejoInventario_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Conexion conexion = new Conexion();
        SqlCommand cmd;

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {

            if(txtUsuario.Text == String.Empty)
            {
                MessageBox.Show("Ingrese un Nombre de Usuario", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorProvider1.SetError(txtUsuario, "Campo Obligatorio");
            }
            else if(TxtContraseña.Text == String.Empty)
            {
                MessageBox.Show("Ingrese una Contrasena", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorProvider1.SetError(TxtContraseña, "Campo Obligatorio");
            }
            else
            {
                conexion.abrir();
                string consulta = "select * from Usuarios where Nombre='" + txtUsuario.Text + "'and Contrasena='" + TxtContraseña.Text + "'";
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
    }
}
