using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Sistema_ManejoInventario_
{
    public partial class CrearReporte : Form
    {
        public CrearReporte()
        {
            InitializeComponent();
        }

        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_reportes;
        Conexion cone = new Conexion();

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider2.Clear();
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result;
                result = MessageBox.Show("¿Seguro que desea eliminar el reporte?", "Eliminar Reporte", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    errorProvider3.Clear();
                }
            }
            else
            {
                MessageBox.Show("Seleccionar un reporte para eliminar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (dataGridView1.SelectedRows.Count <= 0)
                {
                    errorProvider3.SetError(dataGridView1, "Debe Seleccionar Un Registro Para Eliminarlo");
                }
            }
        }

        private void validarForm()
        {
            if (errorProvider2 != null)
            {
                if (txtBusqueda.Text == String.Empty)
                {
                    errorProvider2.SetError(txtBusqueda, "Este Campo es obligatorio");
                    txtBusqueda.Focus();
                    return;
                }
                else
                {
                    errorProvider2.Clear();
                }
            }
         
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void CrearReporte_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = llenarReportes();
        }

        private DataTable llenarReportes()
        {
            conexion.abrir();
            String consulta = "SELECT * FROM ReporteInfo";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_reportes = new DataTable();
            data_adapter.Fill(tabla_reportes);
            conexion.cerrar();

            return tabla_reportes;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            errorProvider3.Clear();
            try
            {
                if(txtBusqueda.Text == String.Empty) {
                    MessageBox.Show("Ingrese un codigo para buscar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    validarForm();
                }
                else
                {
                    conexion.abrir();
                    int cod = Convert.ToInt16(txtBusqueda.Text);
                    String consulta = "Select * from Reportes where Numero = " + cod;
                    data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
                    tabla_reportes = new DataTable();
                    data_adapter.Fill(tabla_reportes);
                    dataGridView1.DataSource = tabla_reportes;
                    if (dataGridView1.Rows.Count < 2)
                    {
                        MessageBox.Show("La busqueda no encotro resultados.", "BUSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtBusqueda.Clear();
                        dataGridView1.DataSource = llenarReportes();
                    }
                    validarForm();
                    conexion.cerrar();
                }     
            }
            catch
            {
                MessageBox.Show("Ingrese un codigo valido", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBusqueda.Clear();
            }
          
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBusqueda.Clear();
            dataGridView1.DataSource = llenarReportes();
            txtBusqueda.Enabled = true;
            errorProvider2.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            errorProvider3.Clear();
            errorProvider2.Clear();
            ReportesAgregar ra = new ReportesAgregar();
            ra.Show();
            ra.FormClosing += new FormClosingEventHandler(this.ReporteInventario_FormClosing);
        }

        private void ReporteInventario_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataGridView1.DataSource = llenarReportes();
            txtBusqueda.Clear();
            errorProvider2.Clear();
            errorProvider3.Clear();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = e.RowIndex;

            txtBusqueda.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtBusqueda.Enabled = false;
        }

        private void txtBusqueda_Validated(object sender, EventArgs e)
        {

        }

        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar>=32 && e.KeyChar <= 47) || (e.KeyChar >= 58 && e.KeyChar <= 255))
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
