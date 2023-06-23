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

        //Instancias de la conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_reportes;

        private void button2_Click(object sender, EventArgs e)
        {
            errorProvider2.Clear();

            //Confirmacion de la elminacion de los registros seleccionados
            if (txtBusqueda.Text != String.Empty)
            {
                DialogResult result;
                result = MessageBox.Show("¿Seguro que desea eliminar el reporte?", "Eliminar Reporte", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    errorProvider3.Clear();
                    conexion.abrir();
                    int codigo;
                    codigo = Convert.ToInt16(txtBusqueda.Text); 
                    SqlCommand cmm = new SqlCommand("Update Reportes Set Estado = 0 Where Numero = " + codigo, conexion.conectardb);
                    cmm.ExecuteNonQuery();
                    conexion.cerrar();
                    txtBusqueda.Clear();
                    dataGridView1.DataSource = llenarReportes();
                    txtBusqueda.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Seleccionar un reporte para eliminar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (txtBusqueda.Text == String.Empty)
                {
                    errorProvider3.SetError(dataGridView1, "Debe Seleccionar Un Registro Para Eliminarlo");
                }
            }
        }

        //Funcion que valida todo el formulario con errorproviders
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

        //Llenar el datagridview a la hora que cargue el formulario
        private void CrearReporte_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = llenarReportes();
            cbxEstado.SelectedIndex = 0;
        }

        //Funcion que extrae la informacion sobre los reportes en la BD
        private DataTable llenarReportes()
        {
            conexion.abrir();
            String consulta = "SELECT [Numero de Reporte], [Tipo de Reporte], Usuario, [Fecha de Creacion] FROM ReporteInfo Where Estado != 0";
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
                //Validacion de campos vacios
                if(txtBusqueda.Text == String.Empty) {
                    MessageBox.Show("Ingrese un número para buscar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    validarForm();
                }
                else
                {
                    button3.Enabled = false;
                    btnEliminar.Enabled = false;
                    cbxEstado.SelectedIndex = -1;
                    cbxEstado.Enabled = false;
                    conexion.abrir();
                    int cod = Convert.ToInt16(txtBusqueda.Text);
                    String consulta = "Select * from Reportes where Numero = " + cod;
                    data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
                    tabla_reportes = new DataTable();
                    data_adapter.Fill(tabla_reportes);
                    dataGridView1.DataSource = tabla_reportes;
                    dataGridView1.Enabled = false;
                    if (dataGridView1.Rows.Count < 1)
                    {
                        MessageBox.Show("La busqueda no encontró resultados.", "BUSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtBusqueda.Clear();
                        txtBusqueda.Clear();
                        dataGridView1.DataSource = llenarReportes();
                        txtBusqueda.Enabled = true;
                        errorProvider2.Clear();
                        button3.Enabled = true;
                        btnEliminar.Enabled = true;
                        dataGridView1.Enabled = true;
                        cbxEstado.Enabled = true;
                        cbxEstado.SelectedIndex = 0;
                    }
                    conexion.cerrar();

                }     
            }
            catch
            {
                MessageBox.Show("Ingrese un número valido", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBusqueda.Clear();
            }
          
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBusqueda.Clear();
            dataGridView1.DataSource = llenarReportes();
            txtBusqueda.Enabled = true;
            errorProvider2.Clear();
            button3.Enabled = true;
            btnEliminar.Enabled = true;
            dataGridView1.Enabled = true;
            cbxEstado.Enabled = true;
            cbxEstado.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            errorProvider3.Clear();
            errorProvider2.Clear();

            //Instancia de formulario Agregar
            ReportesAgregar ra = new ReportesAgregar();
            ra.Show();
            this.Enabled = false;
            ra.FormClosing += new FormClosingEventHandler(this.ReporteInventario_FormClosing);
        }


        /*Funcion que espera al cierre del formulario Agregar, para poder
         actualizar la informacion una vez que esto ocurra*/
        private void ReporteInventario_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataGridView1.DataSource = llenarReportes();
            txtBusqueda.Clear();
            this.Enabled = true;
            txtBusqueda.Clear();
            dataGridView1.DataSource = llenarReportes();
            txtBusqueda.Enabled = true;
            errorProvider2.Clear();
            button3.Enabled = true;
            btnEliminar.Enabled = true;
            errorProvider2.Clear();
            errorProvider3.Clear();
        }


        /*Manejo de la seleccion de celdas del datagridview para almacenarla
         y poder realizar la accion de eliminar */
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

        //Validacion para el cuadro de busqueda, donde solo se permiten numeros
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

        private void cbxEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxEstado.SelectedIndex == 1)
            {
                dataGridView1.DataSource = reportesBorrados();
            }
            else
            {
                dataGridView1.DataSource = llenarReportes();
            }
        }

        private DataTable reportesBorrados()
        {
            conexion.abrir();
            String consulta = "SELECT [Numero de Reporte], [Tipo de Reporte], Usuario, [Fecha de Creacion] FROM ReporteInfo Where Estado = 0";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_reportes = new DataTable();
            data_adapter.Fill(tabla_reportes);
            conexion.cerrar();

            return tabla_reportes;
        }
    }
}
