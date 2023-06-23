using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_ManejoInventario_
{
    public partial class Ventas : Form
    {
        //Instancias de la conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_ventas;
        SqlCommand cmd;
        public Ventas()
        {
            InitializeComponent();
        }

        //Funcion que llena la tabla al iniciar el formulario asi como ocultar valores que aun no se deben ver.
        private void Ventas_Load(object sender, EventArgs e)
        {
            dgv_Ventas.DataSource = llenarVentas();
            lblfact.Hide();
            lblultima.Hide();
            cbxEstado.SelectedIndex = 0;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //Funcion que extrae la informacion de Facturas la BD 
        private DataTable llenarVentas()
        {
            conexion.abrir();
            String consulta = "SELECT Codigo, [Metodo de Pago], Fecha, DNI, RTN, Impuesto, Subtotal, Total, Responsable FROM FacturaCompleta Where Estado != 0";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_ventas = new DataTable();
            data_adapter.Fill(tabla_ventas);
            conexion.cerrar();

            return tabla_ventas;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridview1_RowHeaderMouseClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button7_Click(object sender, EventArgs e)
        {
            errorProvider2.Clear();

            //Validacion de campos vacios
            if (txtBusquedaV.Text == String.Empty)
            {
                MessageBox.Show("Ingrese un código para realizar la busqueda correctamente", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBusquedaV.Focus();
                errorProvider3.SetError(txtBusquedaV, "Campo Obligatorio");
            }
            else
            {
                //Resultados de la busqueda que coincidan con lo pedido
                button2.Enabled = false;
                button3.Enabled = false;
                cbxEstado.Enabled = false;
                cbxEstado.SelectedIndex = -1;
                conexion.abrir();
                String codigov = txtBusquedaV.Text;
                String consulta = "Select * from Factura_Detalle Where Factura = " + codigov;
                data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
                tabla_ventas = new DataTable();
                data_adapter.Fill(tabla_ventas);
                conexion.cerrar();

                dgv_Ventas.DataSource = tabla_ventas;
                dgv_Ventas.Enabled = false;
                errorProvider1.Clear();
                errorProvider3.Clear();
                errorProvider2.Clear();
            }

            if (dgv_Ventas.Rows.Count == 0)
            {
                MessageBox.Show("La búsqueda no encontró resultados.", "BÚSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conexion.abrir();
                txtBusquedaV.Clear();
                dgv_Ventas.DataSource = llenarVentas();
                txtBusquedaV.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                dgv_Ventas.Enabled = true;
                cbxEstado.Enabled = true;
                cbxEstado.SelectedIndex = 0;
            }
            txtBusquedaV.Focus();
            errorProvider1.Clear();
            errorProvider2.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        //Limpieza del formulario a su estado por defecto
        private void button4_Click(object sender, EventArgs e)
        {
            txtBusquedaV.Clear();
            dgv_Ventas.DataSource = llenarVentas();
            txtBusquedaV.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            dgv_Ventas.Enabled = true;
            cbxEstado.Enabled = true;
            cbxEstado.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DetalleFactura df = new DetalleFactura();
            errorProvider1.Clear();
            errorProvider3.Clear();
            errorProvider2.Clear();
            df.Show();
            this.Enabled = false;
            df.FormClosing += new FormClosingEventHandler(this.DetalleFactura_FormClosing);  
        }

        /*Funcion que espera al cierre del formulario de Agregar Facturas, para poder
        actualizar la informacion incluyendo los registros nuevos, asi como desplegar detalles
        de la ultima operacion realizada*/
        private void DetalleFactura_FormClosing(object sender, FormClosingEventArgs e)
        {
            txtBusquedaV.Clear();
            dgv_Ventas.DataSource = llenarVentas();
            txtBusquedaV.Enabled = true;

            conexion.abrir();
            int factura = 0;
            cmd = new SqlCommand("Select * from Factura", conexion.conectardb);
            string query = ("SELECT MAX(Codigo) AS Codigo FROM Factura");
            SqlCommand com = new SqlCommand(query, conexion.conectardb);
            SqlDataReader reg = com.ExecuteReader();
            while (reg.Read())
            {
                factura = Convert.ToInt16((reg["Codigo"]));
            }
            conexion.cerrar();
            this.Enabled = true;
            lblfact.Show();
            lblultima.Show();
            lblultima.Text = factura.ToString();
            txtBusquedaV.Clear();
            dgv_Ventas.DataSource = llenarVentas();
            txtBusquedaV.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            errorProvider1.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            errorProvider3.Clear();
            conexion.abrir();
            try
            {
                //Validacion de campos vacios o invalidos
                if (txtBusquedaV.Text != String.Empty)
                {

                    DialogResult d = MessageBox.Show("¿Está seguro que desea borrar este registro?", "ATENCION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if(d == DialogResult.Yes)
                    {
                        // PARA BORRAR DE TABLA FACTURA
                        string borrar2 = "Update Factura Set Estado = 0 Where Codigo = @c;";
                        using (SqlCommand command = new SqlCommand(borrar2, conexion.conectardb))
                        {
                            command.Parameters.AddWithValue("@c", txtBusquedaV.Text);
                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("Registro eliminado con éxito", "COMPLETADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgv_Ventas.DataSource = llenarVentas();
                        errorProvider2.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("No ha escogido una factura", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    errorProvider2.SetError(dgv_Ventas, "Debe Seleccionar un Registro Para Eliminar");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar el registro\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            conexion.cerrar();
            txtBusquedaV.Clear();
            txtBusquedaV.Enabled = true;
        }

        /*Manejo de la seleccion de celdas del datagridview para almacenarla
        y poder realizar la accion de eliminar */
        private void dgv_Ventas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = e.RowIndex;

            txtBusquedaV.Text = dgv_Ventas.CurrentRow.Cells[0].Value.ToString();
            txtBusquedaV.Enabled = false;
        }

        //Validacion del campo de Busqueda, para que solo permita numeros
        private void txtBusquedaV_KeyPress(object sender, KeyPressEventArgs e)
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

        //Validacion del campo de Busqueda, para que no este vacio
        private void txtBusquedaV_TextChanged(object sender, EventArgs e)
        {
            if(txtBusquedaV.Text.Length > 0)
            {
                errorProvider3.Clear();
            }
        }

        private void cbxEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxEstado.SelectedIndex == 1)
            {
                dgv_Ventas.DataSource = ventasBorradas();
            }
            else
            {
                dgv_Ventas.DataSource = llenarVentas();
            }
        }

        private DataTable ventasBorradas()
        {
            conexion.abrir();
            String consulta = "SELECT Codigo, [Metodo de Pago], Fecha, DNI, RTN, Impuesto, Subtotal, Total, Responsable FROM FacturaCompleta Where Estado = 0";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_ventas = new DataTable();
            data_adapter.Fill(tabla_ventas);
            conexion.cerrar();

            return tabla_ventas;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
