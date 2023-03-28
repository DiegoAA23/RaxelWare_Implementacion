using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace Sistema_ManejoInventario_
{
    public partial class InventarioEmpleado : Form
    {
        public InventarioEmpleado()
        {
            InitializeComponent();
        }

        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_inventario;

        private void InventarioEmpleado_Load(object sender, EventArgs e)
        {
            dgv_inventarios.DataSource = Llenar_Inventario();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string filtro;

            if (cbo_filtro.Text == String.Empty)
            {                
                MessageBox.Show("Escoja el filtro para realizar la busqueda correctamente", "Error de busqueda",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbo_filtro.Focus();
                cbo_filtro.DroppedDown = true;
                errorProvider2.SetError(cbo_filtro, "Escoja un Filtro de Busqueda");
            }
            else if(txt_busqueda.Text == String.Empty)
            {
                MessageBox.Show("Escriba algo para realizar la busqueda correctamente", "Error de busqueda",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt_busqueda.Focus(); 
                errorProvider1.SetError(txt_busqueda, "Este Campo es Obligatorio");
            }
            else
            {
                if (cbo_filtro.Text == "Codigo")
                {
                    filtro = cbo_filtro.Text + " = " + txt_busqueda.Text; //filtro para el codigo
                }
                else
                {
                    filtro = cbo_filtro.Text + " LIKE '%" + txt_busqueda.Text + "%'"; //filtro para categoria y nombre
                }
                dgv_inventarios.DataSource = Busqueda_Inventario(filtro);
                txt_busqueda.Focus();
                errorProvider1.Clear();
                errorProvider2.Clear();
            }
        }

        private DataTable Llenar_Inventario()
        {
            conexion.abrir(); //apertura de la conexion
            String consulta = "select *from Empleado_Productos";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_inventario = new DataTable();

            //llenado de la tabla con el data adpter
            data_adapter.Fill(tabla_inventario);
            conexion.cerrar();
            
            return tabla_inventario;
        }

        private DataTable Busqueda_Inventario(string filtro)
        {
            Console.WriteLine(filtro);
            conexion.abrir(); //apertura de la conexion
            String consulta = "select *from Empleado_Productos where " + filtro.ToString(); //consulta con el filtro
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_inventario = new DataTable();

            //llenado de la tabla con el data adpter
            data_adapter.Fill(tabla_inventario);
            conexion.cerrar();

            return tabla_inventario;
        }

        private void dgv_inventarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cbo_filtro_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider2.Clear();
            txt_busqueda.Clear();
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            dgv_inventarios.DataSource = Llenar_Inventario();
            txt_busqueda.Clear();
            cbo_filtro.SelectedIndex = -1;
        }

        private void txt_busqueda_KeyPress(object sender, KeyPressEventArgs e)
        {
       
            if(cbo_filtro.Text == "Codigo")
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
}
