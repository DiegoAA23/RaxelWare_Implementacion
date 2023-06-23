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

        /*Funciones que permiten el control de las ventanas, para que el usuario pueda 
        moverlas libremente*/
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        //Instancias de conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_facturas;
        SqlCommand cmd;

        //Variables globales necesarias para realizar las operaciones
        int codigo = 0;
        string nombre;
        string precioventa;
        string preciocompra;
        int stock = 0;
        int punto = 0;
        string fecha;
        int stockvalidar;


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


        /*Funcion donde se cargan todos los datos al abrir el formulario, 
         para los combobox y el datetimepicker*/
        private void ActualizarInventario_Load(object sender, EventArgs e)
        {
            autocompletar();
            dtp_Fecha.MaxDate = DateTime.Now;
            dtp_Fecha.Value = DateTime.Today;
            conexion.abrir();

            txtCompra.Enabled = false;
            txtReorden.Enabled = false;
            txtStock.Enabled = false;
            txtVenta.Enabled = false;
            dtp_Fecha.Enabled = false;
            btn_limpiar.Enabled = false;
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

        //Permite el movimiento libre del formulario
        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
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

        //Validacion del Precio, para solo permitir numeros y un unico punto decimal
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

        //Validacion del Precio, para solo permitir numeros y un unico punto decimal
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
                //Validaciones para evitar campos vacios o invalidos

                if (txtProducto.Text != String.Empty && txtVenta.Enabled == false)
                {
                    MessageBox.Show("Seleccione un Producto Para Actualizar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    errorProvider7.SetError(btnAgregar, "Seleccione el Producto");
                }
                else if (txtVenta.Text == String.Empty || txtStock.Text == String.Empty || txtCompra.Text == String.Empty || txtReorden.Text == String.Empty || dtp_Fecha == null || txtProducto.Text == String.Empty)
                {
                    MessageBox.Show("No se pueden ingresar campos vacios", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    validarForm();
                }
                else
                {
                    validarForm();
                    int s = Convert.ToInt16(txtStock.Text);
                    double compra = Convert.ToDouble(txtCompra.Text);
                    double venta = Convert.ToDouble(txtVenta.Text);
                    int reorden = Convert.ToInt16(txtReorden.Text);
                    string fecha = dtp_Fecha.Value.ToString("yyyy/MM/dd");
                    errorProvider7.Clear();
                    if (s < stockvalidar)
                    {
                        MessageBox.Show("El Stock No Puede Disminuirse Manualmente","ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        errorProvider6.SetError(txtStock, "No Se Puede Disminuir");
                    }
                    else if(compra < 1 || venta < 1 || s < 1 || reorden < 1)
                    {
                        if (compra < 1)
                        {
                            errorProvider2.SetError(txtCompra, "Precio Inválido");
                            MessageBox.Show("El Precio No Puede Ser 0", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if(venta < 1)
                        {
                            errorProvider1.SetError(txtVenta, "Precio Inválido");
                            MessageBox.Show("El Precio No Puede Ser 0", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if(s < 1)
                        {
                            errorProvider6.SetError(txtStock, "Cantidad Inválida");
                            MessageBox.Show("El Stock No Puede Ser 0", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        if (reorden < 1)
                        {
                            errorProvider3.SetError(txtReorden, "Cantidad Inválida");
                            MessageBox.Show("El Punto de Reorden No Puede Ser 0", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if(compra >= venta || reorden >= s)
                    {
                        if(compra >= venta)
                        {
                            MessageBox.Show("El Precio de Compra No Puede Ser Mayor o Igual al Precio de Venta", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            errorProvider2.SetError(txtCompra, "Precio Inválido");
                        }

                        if(reorden >= s)
                        {
                            MessageBox.Show("El Punto de Reorden No Puede Ser Mayor o Igual al Stock Comprado", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            errorProvider3.SetError(txtReorden, "Cantidad Inválida");
                        }
                    }
                    else
                    {
                        //Proceso de insertar los datos en la BD
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
                            MessageBox.Show("Se Actualizó el Registro Exitosamente", "COMPLETADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar la fila en la base de datos.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        this.Close();
                    }     
                }
            }
            catch
            {
                MessageBox.Show("No se pudo actualizar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*Funcion que contiene todos los errorprovider del formulario
        para verificar constantemente a medida se hacen cambios */
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


            if (txtProducto.Text == String.Empty)
            {
                errorProvider5.SetError(txtProducto, "Campo Obligatorio");
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

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
            errorProvider4.Clear();
            errorProvider5.Clear();
            errorProvider6.Clear();
            txtVenta.Clear();
            txtStock.Clear();
            txtCompra.Clear();
            txtVenta.Clear();
            txtReorden.Clear();
            txtCompra.Enabled = false;
            txtReorden.Enabled = false;
            txtStock.Enabled = false;
            txtVenta.Enabled = false;
            dtp_Fecha.Enabled = false;
            btn_limpiar.Enabled = false;
            txtProducto.Enabled = true;
            btnAgregar.Enabled = true;
            txtProducto.Clear();
        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //autocompletar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(" "+ ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // Funcion para autocompletar los valores del textbox de Productos
        DataTable datos = new DataTable();
        void autocompletar()
        {
            conexion.abrir();
            SqlCommand com = new SqlCommand("select * from Productos where Estado != 0", conexion.conectardb);
            SqlDataReader dr;
            dr = com.ExecuteReader();
            
            AutoCompleteStringCollection produ = new AutoCompleteStringCollection();

            while (dr.Read())
            {
                produ.Add(dr["Nombre"].ToString());
            }

            txtProducto.AutoCompleteCustomSource = produ;
            dr.Close();
            conexion.cerrar();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if(txtProducto.Text == String.Empty)
            {
                MessageBox.Show("Nombre de Producto Vacio", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorProvider5.SetError(txtProducto, "Campo Obligatorio");
            }
            else
            {
                try
                {
                    conexion.abrir();
                    string cons = "SELECT * from Productos Where Nombre COLLATE Latin1_General_CS_AS = '" + txtProducto.Text + "'";
                    SqlCommand coma = new SqlCommand(cons, conexion.conectardb);
                    SqlDataReader reg = coma.ExecuteReader();
                    if (reg.Read())
                    {
                        conexion.cerrar();
                        /*Rellenar los campos con la informacion actual del producto seleccionado
                    dependiendo del indice del combobox*/
                        errorProvider5.Clear();
                        txtCompra.Enabled = true;
                        txtReorden.Enabled = true;
                        txtStock.Enabled = true;
                        txtVenta.Enabled = true;
                        dtp_Fecha.Enabled = true;
                        btn_limpiar.Enabled = true;
                        btnAgregar.Enabled = false;
                        txtProducto.Enabled = false;
                        errorProvider7.Clear();
                        conexion.abrir();
                        cmd = new SqlCommand("Select * from Productos", conexion.conectardb);
  
                        string consulta = "SELECT * from Productos Where Nombre COLLATE Latin1_General_CS_AS = '" + txtProducto.Text + "'";
                        SqlCommand comando = new SqlCommand(consulta, conexion.conectardb);
                        SqlDataReader registro = comando.ExecuteReader();

                        while (registro.Read())
                        {
                            codigo = Convert.ToInt16((registro["Codigo"]));
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
                            stockvalidar = stock;
                        }
                        dtp_Fecha.Value = DateTime.Today;

                        validarForm();
                    }
                    else
                    {
                        MessageBox.Show("Nombre de Producto No Valido", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtProducto.Clear();
                        errorProvider5.SetError(txtProducto, "Nombre Invalido");
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
                
            }    
        }

        private void txtProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitud = txtProducto.Text.Length;

            if (longitud >= 25)
            {
                if (e.KeyChar != 8)
                {
                    e.Handled = true;
                }
            }
        }
    }
}
