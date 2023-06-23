using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sistema_ManejoInventario_
{
    public partial class DetalleFactura : Form
    {
        public DetalleFactura()
        {
            InitializeComponent();
        }

        //Instancias de la conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_facturas;
        SqlCommand cmd;

        int codigo = 0;
        string nombre = "";
        string precio = "";
        int stock = 0;
        int j = 0;
        int rtn = 0;
        int dni = 0;

        /*Funciones que permiten el control de las ventanas, para que el usuario pueda 
        moverlas libremente*/
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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

        //Funcion que permite el autocompletado en el textbox de productos
        void autocompletar()
        {
            conexion.abrir();
            SqlCommand com = new SqlCommand("select * from Productos where Stock > 0 and Estado != 0", conexion.conectardb);
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


        /*Funcion donde se cargan todos los datos al abrir el formulario, 
        para los combobox y el datetimepicker*/
        private void DetalleFactura_Load(object sender, EventArgs e)
        {
            autocompletar();
            dtp_fecha.MaxDate = DateTime.Now;
            dtp_fecha.Value = DateTime.Today;
            conexion.abrir();
            lblDNI.Hide();
            lblRTN.Hide();

            try
            {
                int numero = 0;
                cmd = new SqlCommand("SELECT MAX(Codigo) AS Codigo FROM Factura", conexion.conectardb);
                string query = ("SELECT MAX(Codigo) AS Codigo FROM Factura");
                SqlCommand com = new SqlCommand(query, conexion.conectardb);
                SqlDataReader reg = com.ExecuteReader();
                while (reg.Read())
                {
                    numero = Convert.ToInt16((reg["Codigo"]));
                }
                numero = numero + 1;

                lblNumero.Text = "No. de Factura: " + numero.ToString();

            }
            catch
            {
                MessageBox.Show("Error al cargar numero de factura", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.cerrar();
            }

            conexion.abrir();

           
            conexion.abrir();
            
            try
            {
                cmd = new SqlCommand("Select * from MetodoPago", conexion.conectardb);

                SqlDataReader registro = cmd.ExecuteReader();
                cbxPago.DisplayMember = "Text";

                while (registro.Read())
                {
                    cbxPago.Items.Add(new { Text = registro["Nombre"], Value = registro["Codigo"].ToString() });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar dato en el ComboBox\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conexion.cerrar();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

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

        }

        private void BtnCerrar_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNormal_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnNormal.Visible = false;
            BtnMaximizar.Visible = true;
        }

        private void BtnMinimizar_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BarraTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BarraTop_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        //Permite el movimiento libre del formulario
        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void dgvProductosV_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgv_agregados_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                conexion.abrir();
                cmd = new SqlCommand("Select * from Productos", conexion.conectardb);
                string prod = txtProducto.Text;
                string consult = "SELECT * from Productos Where Nombre COLLATE Latin1_General_CS_AS = '" + prod + "'";
                SqlCommand coman = new SqlCommand(consult, conexion.conectardb);
                SqlDataReader regis = coman.ExecuteReader();

                bool comprobacion = regis.Read();
                errorProvider2.Clear();

                //Validacion de campos vacios o invalidos
                if (comprobacion is false)
                {
                    errorProvider2.SetError(txtProducto, "Nombre Inválido");
                    MessageBox.Show("Nombre de Producto Inválido", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtProducto.Clear();
                    txtCantidad.Clear();
                    lblStock.Text = "Stock Disponible: " + txtProducto.Text.ToString();
                }
                else if (txtCantidad.Text == String.Empty || txtProducto.Text == String.Empty)
                {
                    if(txtProducto.Text == String.Empty)
                    {
                        errorProvider2.SetError(txtProducto, "Elija un Producto");
                    }

                    if(txtCantidad.Text == String.Empty)
                    {
                        errorProvider2.SetError(txtCantidad, "Escriba una cantidad");
                    }

                    MessageBox.Show("No se puede agregar campos vacíos", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (Convert.ToInt16(txtCantidad.Text) > stock)
                {
                    MessageBox.Show("Stock Insuficiente", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCantidad.Clear();
                    txtProducto.Clear();
                    lblStock.Text = "Stock Disponible: ";
                }
                else if (Convert.ToInt16(txtCantidad.Text) == 0)
                {
                    MessageBox.Show("Ingrese una cantidad válida", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    errorProvider2.SetError(txtCantidad, "Escriba una cantidad válida");
                }
                else
                {
                    int filas = dgv_agregados.Rows.Count;
                    int comp = 0;

                    for (int x = 0; x < filas; x++)
                    {
                        if(nombre == dgv_agregados.Rows[x].Cells[0].Value.ToString())
                        {
                            comp = 1;
                            break;
                        }
                    }

                    if(comp == 1)
                    {
                        MessageBox.Show("El Producto ya Está Agregado", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblStock.Text = "Stock Disponible: ";
                    }
                    else
                    {
                        //Agregado de productos a la lista de compra 
                        dgv_agregados.Rows.Add();
                        string cantidad = txtCantidad.Text;
                        dgv_agregados.Rows[j].Cells[0].Value = nombre;
                        dgv_agregados.Rows[j].Cells[1].Value = precio;
                        dgv_agregados.Rows[j].Cells[2].Value = Convert.ToInt32(cantidad);
                        j++;
                        errorProvider2.Clear();
                        lblStock.Text = "Stock Disponible: ";
                    }
                    txtCantidad.Clear();
                    txtProducto.Clear();
                }
                conexion.cerrar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al Insertar\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

   
        private void button3_Click(object sender, EventArgs e)
        {

            try
            {
                //Validacion de campos vacios o invalidos
                if(txtDNI.Text == String.Empty || cbxPago.SelectedIndex == -1 || dtp_fecha.Value.ToString() == "")
                {
                    if(txtDNI.Text == String.Empty)
                    {
                        errorProvider3.SetError(txtDNI,"Campo Obligatorio");
                    }

                    if(cbxPago.SelectedIndex == -1)
                    {
                        errorProvider4.SetError(cbxPago, "Campo Obligatorio");
                    }
                    MessageBox.Show("No puede ingresar campos vacios","ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if((lblDNI.Text.Length > 0 && lblDNI.Text != "Valido") || (lblRTN.Text.Length > 0 && rtn == 1))
                {
                    if (lblDNI.Text.Length > 0 && lblDNI.Text != "Valido")
                    {
                        MessageBox.Show("DNI Invalido, Por Favor Reingrese el Dato", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtDNI.Clear();
                    }

                    if (lblRTN.Text.Length > 0 && rtn == 1)
                    {
                        MessageBox.Show("RTN Invalido, Por Favor Reingrese el Dato", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtRTN.Clear();
                    }
                }
                else if(dgv_agregados.Rows.Count == 0) 
                {
                    MessageBox.Show("Debe Ingresar Productos en la Factura", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    errorProvider1.SetError(dgv_agregados, "Debe Agregar Productos");
                }
                else
                {
                    // INSERT PARA TABLA FACTURA
                    errorProvider1.Clear();
                    conexion.abrir();
                    string fecha = dtp_fecha.Value.ToString("yyyy/MM/dd");
                    double subtotal = 0;
                    int filas = dgv_agregados.Rows.Count;
                    int c = 0;
                    double p = 0;
                    double s = 0;

                    for (int x = 0; x < filas; x++)
                    {
                        if (dgv_agregados.Rows[x].Cells[1].Value != null && dgv_agregados.Rows[x].Cells[2].Value != null)
                        {
                            c = Convert.ToInt32(dgv_agregados.Rows[x].Cells[2].Value.ToString());
                            p = Convert.ToDouble(dgv_agregados.Rows[x].Cells[1].Value.ToString());
                            s = p * c;
                            subtotal += s;
                        }
                        else
                        {
                            break;
                        }
                    }

                    double impuesto = subtotal * 0.15;
                    double total = subtotal + impuesto;
                    int cod = 1;
                    bool estado = true;

                    cmd = new SqlCommand("Insert into Factura Values('" + (cbxPago.SelectedItem as dynamic).Value + "', '" + fecha + "', '" + txtDNI.Text.ToString() + "', '" + txtRTN.Text.ToString() + "', '" + impuesto + "', '" + subtotal + "', '" + total + "', '" + cod + "', '" + estado + "')", conexion.conectardb);
                    cmd.ExecuteNonQuery();
                    conexion.cerrar();

                    int factura = 0;

                    conexion.abrir();
                    cmd = new SqlCommand("Select * from Factura", conexion.conectardb);
                    string query = ("SELECT MAX(Codigo) AS Codigo FROM Factura");
                    SqlCommand com = new SqlCommand(query, conexion.conectardb);
                    SqlDataReader reg = com.ExecuteReader();
                    while (reg.Read())
                    {
                        factura = Convert.ToInt16((reg["Codigo"]));
                    }

                    conexion.cerrar();

                    // INSERT PARA TABLA DETALLE
                    string producto = "";
                    int cantidadp = 0;
                    int productocodigo = 0;
                    int stockactual =0;
                    int nuevo = 0;

                    for (int i = 0; i < filas; i++)
                    {

                        if(dgv_agregados.Rows[i].Cells[0].Value != null && dgv_agregados.Rows[i].Cells[2].Value != null)
                        {
                            conexion.abrir();
                            producto = dgv_agregados.Rows[i].Cells[0].Value.ToString();
                            cantidadp = Convert.ToInt32(dgv_agregados.Rows[i].Cells[2].Value.ToString());
                            cmd = new SqlCommand("Select * from Productos", conexion.conectardb);
                            string consulta = ("Select * from Productos Where Nombre = '" + producto + "'");
                            SqlCommand comando = new SqlCommand(consulta, conexion.conectardb);
                            SqlDataReader registro = comando.ExecuteReader();
                            while (registro.Read())
                            {
                                productocodigo = Convert.ToInt16((registro["Codigo"]));
                                stockactual = Convert.ToInt16(registro["Stock"]);
                            }
                            conexion.cerrar();

                            conexion.abrir();
                            cmd = new SqlCommand("Insert into Detalle Values('" + factura + "', '" + productocodigo + "', '" + cantidadp + "')", conexion.conectardb);
                            cmd.ExecuteNonQuery();
                            conexion.cerrar();

                            conexion.abrir();
                            nuevo = stockactual - cantidadp;
                            string cons = "UPDATE Productos SET Stock=@Stock Where Codigo=@Codigo";
                            SqlCommand comm = new SqlCommand(cons, conexion.conectardb);
                            comm.Parameters.AddWithValue("@Stock", nuevo);
                            comm.Parameters.AddWithValue("@Codigo", productocodigo);
                            comm.ExecuteNonQuery(); ;
                            conexion.cerrar();
                        }
                        else
                        {
                            break;
                        }
                    }

              

                    MessageBox.Show("Datos ingresados con éxito", "COMPLETADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int fact = Convert.ToInt32(factura);
                    facturaimpresa(fact, subtotal, total, impuesto);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al insertar datos\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Creacion de Factura

        //Envio de todos los datos necesarios para crear la factura e imprimirla
        private void facturaimpresa(int f, double s, double t, double i)
        {
            Clsfunciones.CreaTicket Ticket1 = new Clsfunciones.CreaTicket();

            Ticket1.TextoIzquierda("****************************************");
            Ticket1.TextoCentro("Empresa: Raxel Baterias & Mas");
            Ticket1.TextoIzquierda("****************************************");
            Ticket1.TextoIzquierda("");
            Ticket1.TextoIzquierda("Dirección: Col. Alameda, calle 3318");
            Ticket1.TextoIzquierda("Teléfono: 31823750/22348304");
            Ticket1.TextoIzquierda("RTN: 12345678901234");
            Ticket1.TextoIzquierda("");
            Ticket1.TextoCentro("Factura de Venta");
            Ticket1.TextoIzquierda("No Fac: " + f.ToString());
            Ticket1.TextoIzquierda("Fecha: " + DateTime.Now.ToShortDateString() + "  Hora:" + DateTime.Now.ToShortTimeString());
            Ticket1.TextoIzquierda("");
            Ticket1.TextoIzquierda("****Cliente****");
            Ticket1.TextoIzquierda("DNI: " + txtDNI.Text.ToString());

            if(txtRTN.Text.Length > 0)
            {
                Ticket1.TextoIzquierda("RTN: " + txtRTN.Text.ToString());
            }

            Clsfunciones.CreaTicket.LineasGuion();
            Clsfunciones.CreaTicket.EncabezadoVenta();
            Clsfunciones.CreaTicket.LineasGuion();

            string producto;
            double preciop;
            int cantidadp;
            double subt;
            
            foreach (DataGridViewRow p in dgv_agregados.Rows)
            {
                producto = p.Cells[0].Value.ToString();
                preciop = Convert.ToDouble(p.Cells[1].Value.ToString());
                cantidadp = Convert.ToInt16(p.Cells[2].Value.ToString());
                subt = cantidadp * preciop;
                Ticket1.AgregaArticulo(producto, preciop, cantidadp, subt);
            }

            Clsfunciones.CreaTicket.LineasGuion();
            Ticket1.TextoIzquierda(" ");
            Ticket1.AgregaTotales("Sub-total a pagar:", double.Parse(s.ToString()));
            Ticket1.AgregaTotales("ISV calculado a pagar:", double.Parse(i.ToString()));
            Ticket1.AgregaTotales("Total neto a pagar:", double.Parse(t.ToString()));
            Ticket1.TextoIzquierda(" ");
            Ticket1.TextoIzquierda("========================================");
            Ticket1.TextoIzquierda("*       Gracias por preferirnos      *");
            Ticket1.TextoIzquierda("========================================");
            Ticket1.TextoIzquierda(" ");
            string impresora = "Microsoft XPS Document Writer";
            Ticket1.ImprimirTiket(impresora);
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgv_agregados.SelectedRows.Count > 0)
            {
                dgv_agregados.Rows.RemoveAt(dgv_agregados.SelectedRows[0].Index);
                j--;
            }
            else
            {
                MessageBox.Show("Seleccione un Producto Para Eliminar de la Lista", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtnumerof_Click(object sender, EventArgs e)
        {

        }

        private void DetalleFactura_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /*Validacion del textbox para el DNI, para que solo permita numeros*/
        private void txtDNI_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitud = txtDNI.Text.Length;

            if (longitud >= 13)
            {
                if (e.KeyChar != 8)
                {
                    e.Handled = true;
                }
            }
            else
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
        }

        /*Validacion del textbox para el RTN, para que solo permita numeros*/
        private void txtRTN_KeyPress(object sender, KeyPressEventArgs e)
        {
            int longitud = txtRTN.Text.Length;

            if (longitud >= 14)
            {
                if (e.KeyChar != 8)
                {
                    e.Handled = true;
                }
            }
            else
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
        }

        //Validacion para que en Cantidad solo se puedan ingresar numeros
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
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


        //Validacion que comprueba que el DNI coincida con la expresion regular
        private void txtDNI_TextChanged(object sender, EventArgs e)
        {
            if (txtDNI.Text != String.Empty)
            {
                errorProvider3.Clear();
            }
            else
            {
                errorProvider3.SetError(txtDNI, "Campo Obligatorio");
            }

            Regexp(@"^[0-1]{1}[0-9]{1}[0-1]{1}[0-9]{1}(19|20)\d{2}[0-9]{5}$",txtDNI, pictureBox1, lblDNI, "DNI");
        }

        //Generador del error si no coincide con Regex
        public void Regexp(string re, System.Windows.Forms.TextBox tb, PictureBox pc, Label lbl, string s)
        {
            Regex regex = new Regex(re);

            if (regex.IsMatch(tb.Text))
            {
                lbl.ForeColor = Color.Green;
                lbl.Text = "Valido";
                lbl.Hide();
                pictureBox1.Hide();
            }
            else
            {
                pc.Image = Properties.Resources.invalido;
                lblDNI.Show();
                lbl.ForeColor = Color.Red;
                lbl.Text = s + " Inválido";
            }
        }

        //Validacion que comprueba que el RTN coincida con la expresion regular
        private void txtRTN_TextChanged(object sender, EventArgs e)
        {
            if(txtRTN.Text.Length > 0)
            {
                pictureBox2.Show();
                Regexp2(@"^[0-1]{1}[0-9]{1}[0-1]{1}[0-9]{1}(19|20)\d{2}[0-9]{6}$", txtRTN, pictureBox2, lblRTN, "RTN");
            }
            else
            {
                lblRTN.Hide();
                pictureBox2.Hide();
                rtn = 0;
            }  
        }

        //Generador del error si no coincide con Regex
        public void Regexp2(string re, System.Windows.Forms.TextBox tb, PictureBox pc, Label lbl, string s)
        {
            Regex regex = new Regex(re);

            if (regex.IsMatch(tb.Text))
            {
                lbl.ForeColor = Color.Green;
                lbl.Text = "Valido";
                lbl.Hide();
                pictureBox2.Hide();
                rtn = 0;
            }
            else
            {
                pc.Image = Properties.Resources.invalido;
                lblRTN.Show();
                lbl.ForeColor = Color.Red;
                lbl.Text = s + " Inválido";
                rtn = 1;
            }
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal; btnNormal.Visible = true; BtnMaximizar.Visible = false;
        }

        private void BtnMaximizar_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal; btnNormal.Visible = true; BtnMaximizar.Visible = false;
        }

        private void lblStock_Click(object sender, EventArgs e)
        {

        }

        private void cbxPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider4.Clear();
        }

        //Extraccion de los datos del producto cuando la seleccion cambia
        private void txtProducto_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtProducto.Text != String.Empty)
                {
                    lblStock.Text = "Stock Disponible: ";
                    conexion.abrir();
                    cmd = new SqlCommand("Select * from Productos", conexion.conectardb);

                    string consulta = "SELECT * from Productos Where Nombre COLLATE Latin1_General_CS_AS = '" + txtProducto.Text + "'";
                    SqlCommand comando = new SqlCommand(consulta, conexion.conectardb);
                    SqlDataReader registro = comando.ExecuteReader();
                    while (registro.Read())
                    {
                        codigo = Convert.ToInt16((registro["Codigo"]));
                        nombre = Convert.ToString((registro["Nombre"]));
                        precio = Convert.ToString((registro["Precio de Venta"]));
                        stock = Convert.ToInt16(registro["Stock"]);
                        lblStock.Text = "Stock Disponible: " + stock.ToString();
                    }
                }
                else if(txtProducto.Text == String.Empty)
                {
                    lblStock.Text = "Stock Disponible: ";
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

}

