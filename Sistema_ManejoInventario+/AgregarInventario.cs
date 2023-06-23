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

        //Instancias de conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_inventario;
        SqlCommand cmd;

        private const int SombraForm = 0x20000;

        /*Funciones que agregan una sombra al formulario, para resaltarlo mejor en
         pantalla*/
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= SombraForm;
                return cp;
            }
        }

        /*Funciones que permiten el control de las ventanas, para que el usuario pueda 
         moverlas libremente*/
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

        /*Funcion donde se cargan todos los datos al abrir el formulario, 
         para los combobox y el datetimepicker*/
        private void AgregarInventario_Load(object sender, EventArgs e)
        {
            dtpFechaCompra.MaxDate = DateTime.Now;
            dtpFechaCompra.Value = DateTime.Today;
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

        //Permite el movimiento libre del formulario
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
                validarForm();
                //Validaciones para evitar campos vacios o invalidos
                if (txtNombre.Text == String.Empty || txtNombre.TextLength < 5 || txtPrecioVenta.Text == String.Empty || txtStock.Text == String.Empty || txtPrecioCompra.Text == String.Empty || txtPuntoOrden.Text == String.Empty || dtpFechaCompra == null || cmbCategoria.SelectedIndex == -1 || cmbDistribuidor.SelectedIndex == -1)
                {
                    if (txtNombre.TextLength > 0 && txtNombre.TextLength < 5)
                    {
                        MessageBox.Show("El Nombre del Producto es muy Corto", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (txtNombre.Text == String.Empty || txtPrecioVenta.Text == String.Empty || txtStock.Text == String.Empty || txtPrecioCompra.Text == String.Empty || txtPuntoOrden.Text == String.Empty || dtpFechaCompra == null || cmbCategoria.SelectedIndex == -1 || cmbDistribuidor.SelectedIndex == -1)
                    {
                        MessageBox.Show("No se Pueden Ingresar Vampos Vacíos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    validarForm();
                }
                else if (txtNombre.TextLength <= 5)
                {
                    errorProvider1.SetError(txtNombre, "Nombre Muy Corto");
                }
                else if (Convert.ToDouble(txtPrecioCompra.Text) < 1 || Convert.ToDouble(txtPrecioVenta.Text) < 1 || Convert.ToInt16(txtPuntoOrden.Text) < 1 || Convert.ToInt16(txtStock.Text) < 1)
                {

                    if (Convert.ToDouble(txtPrecioCompra.Text) < 1)
                    {
                        errorProvider3.SetError(txtPrecioCompra, "Precio Invalido");
                    }

                    if (Convert.ToDouble(txtPrecioVenta.Text) < 1)
                    {
                        errorProvider2.SetError(txtPrecioVenta, "Precio Invalido");
                    }

                    if (Convert.ToDouble(txtPrecioCompra.Text) < 1 || Convert.ToDouble(txtPrecioVenta.Text) < 1)
                    {
                        MessageBox.Show("El Precio No Puede Ser 0", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if(Convert.ToInt16(txtPuntoOrden.Text) < 1)
                    {
                        errorProvider4.SetError(txtPuntoOrden, "Cantidad Invalida");
                        MessageBox.Show("El Punto de Reorden No Puede Ser 0", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (Convert.ToInt16(txtStock.Text) < 1)
                    {
                        errorProvider8.SetError(txtStock, "Cantidad Invalida");
                        MessageBox.Show("El Stock No Puede Ser 0", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }else if(Convert.ToDouble(txtPrecioCompra.Text) >= Convert.ToDouble(txtPrecioVenta.Text) || Convert.ToInt16(txtStock.Text) <= Convert.ToInt16(txtPuntoOrden.Text))
                {
                    if (Convert.ToDouble(txtPrecioCompra.Text) >= Convert.ToDouble(txtPrecioVenta.Text))
                    {
                        errorProvider3.SetError(txtPrecioCompra, "Precio Invalido");
                        MessageBox.Show("El Precio de Compra No Puede Ser Mayor o Igual al Precio de Venta", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (Convert.ToInt16(txtPuntoOrden.Text) >= Convert.ToInt16(txtStock.Text))
                    {
                        errorProvider4.SetError(txtPuntoOrden, "Cantidad Invalida");
                        MessageBox.Show("El Punto de Reorden No Puede Ser Mayor o Igual al Stock Comprado", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                        
                }
                else
                {
                    //Proceso de adaptar los datos para la BD
                    conexion.abrir();
                    string fecha = dtpFechaCompra.Value.ToString("yyyy/MM/dd");
                    string nombre = txtNombre.Text.ToString();
                    double precioventa = Convert.ToDouble(txtPrecioVenta.Text.ToString());
                    double preciocompra = Convert.ToDouble(txtPrecioCompra.Text.ToString());
                    int stock = Convert.ToInt16(txtStock.Text.ToString());
                    int puntore = Convert.ToInt16(txtPuntoOrden.Text.ToString());
                    bool estado = true;

                    //Proceso de insertar los datos en la BD
                    cmd = new SqlCommand("Insert into Productos Values('" + nombre + "', '" + stock + "', '" + precioventa + "', '" + preciocompra + "', '" + fecha + "', '" + puntore + "', '" + (cmbCategoria.SelectedItem as dynamic).Value + "', '" + (cmbDistribuidor.SelectedItem as dynamic).Value + "', '" + estado + "')", conexion.conectardb);
                    cmd.ExecuteNonQuery();
                    conexion.cerrar();
                    MessageBox.Show("Registros agregados con éxito", "COMPLETADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar datos\n" + ex);
            }
        }

        /*Funcion que contiene todos los errorprovider del formulario
         para verificar constantemente a medida se hacen cambios */
        private void validarForm()
        {
            if (txtNombre.Text == String.Empty)
            {
                errorProvider1.SetError(txtNombre, "Campo Obligatorio");
            }
            else if(txtNombre.TextLength <= 5)
            {
                errorProvider1.SetError(txtNombre, "Nombre Muy Corto");
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
            else
            {
                errorProvider7.Clear();
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

        //Validacion del Stock, para solo permitir numeros
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

        //Validacion del Punto de Reorden, para solo permitir numeros
        private void txtPuntoOrden_KeyPress(object sender, KeyPressEventArgs e)
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

        //Validacion del Precio, para solo permitir numeros y un unico punto decimal
        private void txtPrecioCompra_KeyPress(object sender, KeyPressEventArgs e)
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

        //Validacion del Precio, para solo permitir numeros y un unico punto decimal
        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
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

        private void dtpFechaCompra_ValueChanged(object sender, EventArgs e)
        {

        }

        //Validacion de la longitud del Nombre
        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitud = txtNombre.Text.Length;

            if (longitud == 25)
            {
                e.Handled = true;

                return;
            }

            if(longitud > 5)
            {
                errorProvider1.Clear();
            }
        }

        private void cmbDistribuidor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtPrecioCompra_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
