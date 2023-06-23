using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_ManejoInventario_
{
    public partial class Inventario : Form
    {
        public Inventario()
        {
            InitializeComponent();
        }

        //Instancias de conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_inventario;
        SqlCommand cmd;

        private void Inventario_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Llenar_Inventario();
            cbxEstado.SelectedIndex = 0;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }

        //Funcion que llena un data adapter con los datos de inventario
        private DataTable Llenar_Inventario()
        {
            conexion.abrir();
            String consulta = "SELECT Codigo, Nombre, Stock, [Precio de Venta], [Precio de Compra], [Fecha de Compra], [Punto de Reorden], Categoria, Distribuidor FROM ProductoDetalle Where Estado != 0";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_inventario = new DataTable();

            //Llenado de la tabla con el data adpter
            data_adapter.Fill(tabla_inventario);
            conexion.cerrar();

            return tabla_inventario;
        }

        private DataTable Busqueda_Inventario(string filtro)
        {
            Console.WriteLine(filtro);
            conexion.abrir(); //apertura de la conexion
            String consulta = "select * from ProductoDetalle where " + filtro.ToString(); //consulta con el filtro
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_inventario = new DataTable();

            //llenado de la tabla con el data adpter
            data_adapter.Fill(tabla_inventario);
            conexion.cerrar();

            return tabla_inventario;
        }

        private void dataGridView1_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //Limpeza del formulario para resetearlo
        private void btn_limpiar_Click_1(object sender, EventArgs e)
        {
            limpiarForm();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            errorProvider3.Clear();
            errorProvider2.Clear();
            errorProvider1.Clear();

            //Instancia de formulario Agregar
            AgregarInventario ag = new AgregarInventario();
            ag.Show();
            this.Enabled = false;
            ag.FormClosing += new FormClosingEventHandler(this.AgregarInventario_FormClosing);
        }

        /*Funcion que espera al cierre del formulario Agregar, para poder
         actualizar la informacion una vez que esto ocurra*/
        private void AgregarInventario_FormClosing(object sender, FormClosingEventArgs e)
        {
            limpiarForm();
            this.Enabled = true;
        }


        private void btnActualizar_Click_1(object sender, EventArgs e)
        {
            errorProvider2.Clear();
            errorProvider1.Clear();
            errorProvider3.Clear();

            //Instancia de formulario Actualizar
            ActualizarInventario av = new ActualizarInventario(); 
            av.Show();
            this.Enabled = false;
            av.FormClosing += new FormClosingEventHandler(this.ActualizarInventario_FormClosing);
            limpiarForm();
        }

        /*Funcion que espera al cierre del formulario Actualizar, para poder
        actualizar la informacion una vez que esto ocurra*/
        private void ActualizarInventario_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = true;
            limpiarForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filtro;

            //Validaciones para evitar campos vacios
            if (cbxFiltro.Text == String.Empty)
            {
                MessageBox.Show("Escoja el filtro para realizar la búsqueda correctamente", "Error de busqueda",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxFiltro.Focus();
                cbxFiltro.DroppedDown = true;
                errorProvider2.SetError(cbxFiltro, "Escoja un Filtro de Busqueda");
            }
            else if (txtBusqueda.Text == String.Empty)
            {
                MessageBox.Show("Escriba algo para realizar la búsqueda correctamente", "Error de busqueda",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBusqueda.Focus();
                errorProvider1.SetError(txtBusqueda, "Este Campo es Obligatorio");
            }
            else
            {
                btnActualizar.Enabled = false;
                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;

                //Definicion del tipo de filtro de busqueda
                if (cbxFiltro.Text == "Codigo")
                {
                    filtro = cbxFiltro.Text + " = " + txtBusqueda.Text; //filtro para el codigo
                }
                else
                {
                    filtro = cbxFiltro.Text + " LIKE '%" + txtBusqueda.Text + "%'"; //filtro para categoria y nombre
                }
                dataGridView1.DataSource = Busqueda_Inventario(filtro);
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("La búsqueda no encontro resultados.", "BÚSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conexion.abrir();
                    dataGridView1.DataSource = Llenar_Inventario();
                    conexion.cerrar();
                    limpiarForm();
                }
                txtBusqueda.Focus();
                errorProvider1.Clear();
                errorProvider2.Clear();
                errorProvider3.Clear();
            }
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            errorProvider2.Clear();
            errorProvider1.Clear();
            errorProvider3.Clear();
            conexion.abrir();
            try
            {
                //Validaciones para evitar campos invalidos
                if (txtBusqueda.Text != String.Empty)
                {

                    DialogResult d = MessageBox.Show("¿Está seguro que desea borrar este registro?", "ATENCION", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    errorProvider3.Clear();
                    if (d == DialogResult.Yes)
                    {

                        string borrar2 = "UPDATE Productos SET Estado = 0 WHERE Codigo = @Codigo;";
                        using (SqlCommand command = new SqlCommand(borrar2, conexion.conectardb))
                        {
                            command.Parameters.AddWithValue("@Codigo", txtBusqueda.Text);
                            command.ExecuteNonQuery();
                        }

                        MessageBox.Show("Registro eliminado con éxito", "COMPLETADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridView1.DataSource = Llenar_Inventario();
                    }
                }
                else
                {
                    MessageBox.Show("No ha seleccionado un registro para eliminar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    if (dataGridView1.SelectedRows.Count <= 0)
                    {
                        errorProvider3.SetError(dataGridView1, "Debe Seleccionar Un Registro Para Eliminarlo");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar el registro\n" + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            conexion.cerrar();
            txtBusqueda.Clear();
            txtBusqueda.Enabled = true;
        }

        /*Manejo de la seleccion de celdas del datagridview para almacenarla
         y poder realizar la accion de eliminar */
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            int i;
            i = e.RowIndex;
            txtBusqueda.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtBusqueda.Enabled = false;
        }

        private void dataGridView1_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        /*Limpieza del formulario para resetear todos los controles y
         volverlo a su estado por defecto*/
        private void limpiarForm()
        {
            foreach (Control ctr in this.Controls)
            {
                if (ctr is TextBox)
                {
                    ctr.Text = "";
                }
                cbxFiltro.SelectedIndex = -1;
            }
            txtBusqueda.Enabled = true;
            dataGridView1.DataSource = Llenar_Inventario();
            btnActualizar.Enabled = true;
            btnAgregar.Enabled = true;
            btnEliminar.Enabled = true;
            cbxEstado.Enabled = true;
            cbxEstado.SelectedIndex = 0;
            dataGridView1.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider2.Clear();
            txtBusqueda.Clear();
            txtBusqueda.Enabled = true;
            cbxEstado.Enabled = false;
            cbxEstado.SelectedIndex = -1;
        }

        /*Validacion que prohibe al usuario escribir ciertos valores
        dependiendo del filtro de busqueda que ha escogio. Codigo solo permite
        numeros, Categoria solo letras, y Nombre ambos tipos*/
        private void txtBusqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbxFiltro.Text == "Codigo")
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
            else if (cbxFiltro.Text == "Categoria")
            {
                if (!(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }


        }

        /*Funcion para resaltar los articulos del inventario que hayan
         llegado a su punto de reorden, esten agotados, etc.*/
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
                int totalfilas = dataGridView1.Rows.Count;
                int stock;
                int reorden;

                for (int i = 0; i < totalfilas; i++)
                {
                    stock = Convert.ToInt16(dataGridView1.Rows[i].Cells[2].Value);
                    reorden = Convert.ToInt16(dataGridView1.Rows[i].Cells[6].Value);
                    if (stock <= reorden)
                    {
                        DataGridViewRow row = dataGridView1.Rows[i];

                        row.DefaultCellStyle.BackColor = Color.IndianRed;
                    }
                }
        }

        private void cbxEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxEstado.SelectedIndex == 1)
            {
                dataGridView1.DataSource = inventarioBorrado();
            }
            else
            {
                dataGridView1.DataSource = Llenar_Inventario();
            }
        }

        private DataTable inventarioBorrado()
        {
            conexion.abrir();
            String consulta = "SELECT Codigo, Nombre, Stock, [Precio de Venta], [Precio de Compra], [Fecha de Compra], [Punto de Reorden], Categoria, Distribuidor FROM ProductoDetalle Where Estado = 0";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_inventario = new DataTable();
            data_adapter.Fill(tabla_inventario);
            conexion.cerrar();

            return tabla_inventario;
        }
    }
}
