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

        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_ventas;
        SqlCommand cmd;
        public Ventas()
        {
            InitializeComponent();
        }

        private void Ventas_Load(object sender, EventArgs e)
        {
            dgv_Ventas.DataSource = llenarVentas();
            lblfact.Hide();
            lblultima.Hide();  
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private DataTable llenarVentas()
        {
            conexion.abrir();
            String consulta = "Select * from FacturaCompleta";
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
            if (txtBusquedaV.Text == String.Empty)
            {
                MessageBox.Show("Ingrese un codigo para realizar la busqueda correctamente", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBusquedaV.Focus();
                errorProvider1.SetError(txtBusquedaV,"Campo Obligatorio");
            }
            else
            {
                button2.Enabled = false;
                button3.Enabled = false;
                conexion.abrir();
                String codigov = txtBusquedaV.Text;
                String consulta = "Select * from Factura_Detalle Where Factura = " + codigov;
                data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
                tabla_ventas = new DataTable();
                data_adapter.Fill(tabla_ventas);
                conexion.cerrar();

                dgv_Ventas.DataSource = tabla_ventas;
                errorProvider1.Clear();
            }

            if (dgv_Ventas.Rows.Count == 0)
            {
                MessageBox.Show("La busqueda no encotro resultados.", "BUSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conexion.abrir();
                dgv_Ventas.DataSource = llenarVentas();
                conexion.cerrar();
                txtBusquedaV.Clear();
            }
            txtBusquedaV.Focus();
            errorProvider1.Clear();
            errorProvider2.Clear();
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtBusquedaV.Clear();
            dgv_Ventas.DataSource = llenarVentas();
            txtBusquedaV.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DetalleFactura df = new DetalleFactura();
            df.Show();
            df.FormClosing += new FormClosingEventHandler(this.DetalleFactura_FormClosing);  
        }

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
            lblfact.Show();
            lblultima.Show();
            lblultima.Text = factura.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            conexion.abrir();
            try
            {
                if (txtBusquedaV.Text != String.Empty)
                {

                    DialogResult d = MessageBox.Show("¿Esta seguro que desea borrar este registro?", "ATENCION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if(d == DialogResult.Yes)
                    {
                        // PARA BORRAR DE TABLA DETALLE
                        string borrar = "Delete From Detalle Where Factura_Codigo = @c;";
                        
                        using (SqlCommand command = new SqlCommand(borrar, conexion.conectardb))
                        {
                            command.Parameters.AddWithValue("@c", txtBusquedaV.Text);
                            command.ExecuteNonQuery();
                        }

                        // PARA BORRAR DE TABLA FACTURA
                        string borrar2 = "Delete From Factura Where Codigo = @c;";
                        using (SqlCommand command = new SqlCommand(borrar2, conexion.conectardb))
                        {
                            command.Parameters.AddWithValue("@c", txtBusquedaV.Text);
                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("Registro eliminado con exito", "COMPLETADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dgv_Ventas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = e.RowIndex;

            txtBusquedaV.Text = dgv_Ventas.CurrentRow.Cells[0].Value.ToString();
            txtBusquedaV.Enabled = false;
        }

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
    }
}
