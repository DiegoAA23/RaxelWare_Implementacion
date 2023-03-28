using System;
using System.Collections;
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

namespace Sistema_ManejoInventario_
{
    public partial class ActualizarInventario : Form
    {
        public ActualizarInventario()
        {
            InitializeComponent();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_facturas;
        SqlCommand cmd;

        int codigo = 0;
        string nombre;
        string precioventa;
        string preciocompra;
        int stock = 0;
        int punto = 0;
        string fecha;

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

        private void ActualizarInventario_Load(object sender, EventArgs e)
        {
            dtp_Fecha.MaxDate = DateTime.Now;
            conexion.abrir();

            try
            {
                cmd = new SqlCommand("Select * from Producto_Factura", conexion.conectardb);

                SqlDataReader registro = cmd.ExecuteReader();
                cbxProductos.DisplayMember = "Text";

                while (registro.Read())
                {
                    cbxProductos.Items.Add(new { Text = registro["Nombre"], Value = registro["Codigo"].ToString() });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar dato en el ComboBox", "ERROR");
            }
            finally
            {
                conexion.cerrar();
            }
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

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void cbxProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            validarForm();
            try
            {
                if (cbxProductos.SelectedIndex != -1)
                {
                    conexion.abrir();
                    cmd = new SqlCommand("Select * from Productos", conexion.conectardb);

                    string consulta = "SELECT * from Productos Where Codigo = '" + (cbxProductos.SelectedItem as dynamic).Value + "'";
                    SqlCommand comando = new SqlCommand(consulta, conexion.conectardb);
                    SqlDataReader registro = comando.ExecuteReader();
                    while (registro.Read())
                    {
                        codigo = Convert.ToInt16((cbxProductos.SelectedItem as dynamic).Value);
                        nombre = Convert.ToString((registro["Nombre"]));
                        precioventa = Convert.ToString((registro["Precio de Venta"]));
                        preciocompra = Convert.ToString((registro["Precio de Compra"]));
                        stock = Convert.ToInt16(registro["Stock"]);
                        punto = Convert.ToInt16(registro["Punto de Reorden"]);
                        fecha = Convert.ToString(registro["Fecha de Compra"]);
                        txtVenta.Text = precioventa;
                        txtCompra.Text = preciocompra;
                        txtStock.Text = stock.ToString();
                        txtReorden.Text = punto.ToString();
                        dtp_Fecha.Text = fecha.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.cerrar();
            }
            validarForm();
        }

        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtReorden_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void txtVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void txtCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtVenta.Text == String.Empty || txtStock.Text == String.Empty || txtCompra.Text == String.Empty || txtReorden.Text == String.Empty || dtp_Fecha == null || cbxProductos.SelectedIndex == -1)
                {
                    MessageBox.Show("No se pueden ingresar campos vacios", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    validarForm();
                }
                else
                {
                    int s = Convert.ToInt16(txtStock.Text);
                    double compra = Convert.ToDouble(txtCompra.Text);
                    double venta = Convert.ToDouble(txtVenta.Text);
                    int reorden = Convert.ToInt16(txtReorden.Text);
                    string fecha = dtp_Fecha.Value.ToString("yyyy/MM/dd");

                    conexion.abrir();
                    string consulta = "UPDATE Productos SET Stock=@Stock, [Precio de Venta]=@Venta, [Precio de Compra]=@Compra, [Punto de Reorden]=@Reorden, [Fecha de Compra]=@Fecha WHERE Codigo=@Codigo";
                    SqlCommand comando = new SqlCommand(consulta, conexion.conectardb);
                    comando.Parameters.AddWithValue("@Nombre", nombre);
                    comando.Parameters.AddWithValue("@Stock", s);
                    comando.Parameters.AddWithValue("@Codigo", codigo);
                    comando.Parameters.AddWithValue("@Compra", compra);
                    comando.Parameters.AddWithValue("@Venta", venta);
                    comando.Parameters.AddWithValue("@Reorden", reorden);
                    comando.Parameters.AddWithValue("@Fecha", fecha);
                    int filas_actualizadas = comando.ExecuteNonQuery(); ;
                    conexion.cerrar();

                    if (filas_actualizadas > 0)
                    {
                        MessageBox.Show("Se actualizó la fila en la base de datos.");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar la fila en la base de datos.");
                    }
                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show("No se pudo actualizar", "ERROR");
            }
        }
        private void validarForm()
        {
            if (txtVenta.Text == String.Empty)
            {
                errorProvider1.SetError(txtVenta, "Campo Obligatorio");
            }
            else
            {
                errorProvider1.Clear();
            }


            if (txtCompra.Text == String.Empty)
            {
                errorProvider2.SetError(txtCompra, "Campo Obligatorio");
            }
            else
            {
                errorProvider2.Clear();
            }


            if (txtReorden.Text == String.Empty)
            {
                errorProvider3.SetError(txtReorden, "Campo Obligatorio");
            }
            else
            {
                errorProvider3.Clear();
            }

            if (dtp_Fecha == null)
            {
                errorProvider4.SetError(dtp_Fecha, "Campo Obligatorio");
            }
            else
            {
                errorProvider4.Clear();
            }


            if (cbxProductos.SelectedIndex == -1)
            {
                errorProvider5.SetError(cbxProductos, "Campo Obligatorio");
            }
            else
            {
                errorProvider5.Clear();
            }

            if (txtStock.Text == String.Empty)
            {
                errorProvider6.SetError(txtStock, "Campo Obligatorio");
            }
            else
            {
                errorProvider6.Clear();
            }
        }
    }
}
