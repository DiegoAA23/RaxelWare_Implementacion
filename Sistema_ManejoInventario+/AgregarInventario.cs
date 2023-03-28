using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace Sistema_ManejoInventario_
{
    public partial class AgregarInventario : Form
    {
        public AgregarInventario()
        {
            InitializeComponent();
        }

        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_inventario;
        SqlCommand cmd;

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

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnNormal.Visible = false;
            BtnMaximizar.Visible = true;
        }

        private void AgregarInventario_Load(object sender, EventArgs e)
        {
            dtpFechaCompra.MaxDate = DateTime.Now;
            conexion.abrir();

            try
            {
                cmd = new SqlCommand("Select * from Categorias", conexion.conectardb);

                SqlDataReader registro = cmd.ExecuteReader();
                cmbCategoria.DisplayMember = "Text";

                while (registro.Read())
                {
                    cmbCategoria.Items.Add(new { Text = registro["Nombre"], Value = registro["Codigo"].ToString() });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar dato en el ComboBox\n" + ex, "ERROR");
            }
            finally
            {
                conexion.cerrar();
            }

            conexion.abrir();
            try
            {
                cmd = new SqlCommand("Select * from Distribuidores", conexion.conectardb);

                SqlDataReader registro = cmd.ExecuteReader();
                cmbDistribuidor.DisplayMember = "Text";

                while (registro.Read())
                {
                    cmbDistribuidor.Items.Add(new { Text = registro["Nombre"], Value = registro["Codigo"].ToString() });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar dato en el ComboBox\n " + ex, "ERROR");
            }
            finally
            {
                conexion.cerrar();
            }
        }

        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void AgregarInventario_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNombre.Text == String.Empty || txtPrecioVenta.Text == String.Empty || txtStock.Text == String.Empty || txtPrecioCompra.Text == String.Empty || txtPuntoOrden.Text == String.Empty || dtpFechaCompra == null || cmbCategoria == null || cmbDistribuidor == null)
                {
                    MessageBox.Show("No se pueden ingresar campos vacios", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    validarForm();
                }
                else
                {
                    conexion.abrir();
                    string fecha = dtpFechaCompra.Value.ToString("yyyy/MM/dd");
                    string nombre = txtNombre.Text.ToString();
                    double precioventa = Convert.ToDouble(txtPrecioVenta.Text.ToString());
                    double preciocompra = Convert.ToDouble(txtPrecioCompra.Text.ToString());
                    int stock = Convert.ToInt16(txtStock.Text.ToString());
                    int puntore = Convert.ToInt16(txtPuntoOrden.Text.ToString());
                    bool estado = true;

                    cmd = new SqlCommand("Insert into Productos Values('" + nombre + "', '" + stock + "', '" + precioventa + "', '" + preciocompra + "', '" + fecha + "', '" + puntore + "', '" + (cmbCategoria.SelectedItem as dynamic).Value + "', '" + (cmbDistribuidor.SelectedItem as dynamic).Value + "', '" + estado + "')", conexion.conectardb);
                    cmd.ExecuteNonQuery();
                    conexion.cerrar();
                    MessageBox.Show("Registros agregados con exito", "COMPLETADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar datos\n" + ex);
            }
        }

        private void validarForm()
        {
            if (txtNombre.Text == String.Empty)
            {
                errorProvider1.SetError(txtNombre, "Campo Obligatorio");
            }
            else
            {
                errorProvider1.Clear();
            }

            if (txtPrecioVenta.Text == String.Empty)
            {
                errorProvider2.SetError(txtPrecioVenta, "Campo Obligatorio");
            }
            else
            {
                errorProvider2.Clear();
            }


            if (txtPrecioCompra.Text == String.Empty)
            {
                errorProvider3.SetError(txtPrecioCompra, "Campo Obligatorio");
            }
            else
            {
                errorProvider3.Clear();
            }


            if (txtPuntoOrden.Text == String.Empty)
            {
                errorProvider4.SetError(txtPuntoOrden, "Campo Obligatorio");
            }
            else
            {
                errorProvider4.Clear();
            }

            if (dtpFechaCompra == null)
            {
                errorProvider5.SetError(dtpFechaCompra, "Campo Obligatorio");
            }
            else
            {
                errorProvider5.Clear();
            }


            if (cmbCategoria.SelectedIndex == -1)
            {
                errorProvider6.SetError(cmbCategoria, "Campo Obligatorio");
            }
            else
            {
                errorProvider6.Clear();
            }

            if (cmbDistribuidor.SelectedIndex == -1)
            {
                errorProvider7.SetError(cmbDistribuidor, "Campo Obligatorio");
            }

            if (txtStock.Text == String.Empty)
            {
                errorProvider8.SetError(txtStock, "Campo Obligatorio");
            }
            else
            {
                errorProvider8.Clear();
            }
        }

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarForm();
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                e.Handled = true;  
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtPuntoOrden_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarForm();
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }

        }

        private void txtPrecioCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarForm();
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;

                return;
            }

            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }

        }

        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarForm();
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;

                return;
            }

            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }

        }

        private void dtpFechaCompra_ValueChanged(object sender, EventArgs e)
        {
            validarForm();
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            validarForm();
            int longitud = txtNombre.Text.Length;

            if (longitud == 25)
            {
                e.Handled = true;

                return;
            }
        }
    }
}
