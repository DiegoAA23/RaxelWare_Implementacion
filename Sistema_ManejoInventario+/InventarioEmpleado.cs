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

        //Instancias de conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_inventario;

        //Llenado del formulario con los datos de la BD al iniciar el formulario
        private void InventarioEmpleado_Load(object sender, EventArgs e)
        {
            dgv_inventarios.DataSource = Llenar_Inventario();
        }

        //Realiza la busqueda de inventario especificada por el usuario
        private void button1_Click(object sender, EventArgs e)
        {
            string filtro;

            //Validaciones para evitar campos vacios
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
                //Definicion del tipo de filtro de busqueda
                if (cbo_filtro.Text == "Codigo")
                {
                    filtro = cbo_filtro.Text + " = " + txt_busqueda.Text; //filtro para el codigo
                }
                else
                {
                    filtro = cbo_filtro.Text + " LIKE '%" + txt_busqueda.Text + "%'"; //filtro para categoria y nombre
                }

                dgv_inventarios.DataSource = Busqueda_Inventario(filtro);

                if (dgv_inventarios.Rows.Count == 0)
                {
                    MessageBox.Show("La búsqueda no encontró resultados.", "BÚSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    conexion.abrir();
                    dgv_inventarios.DataSource = Llenar_Inventario();
                    conexion.cerrar();
                    txt_busqueda.Clear();
                    cbo_filtro.SelectedIndex = -1;
                }

                txt_busqueda.Focus();
                errorProvider1.Clear();
                errorProvider2.Clear();
            }
        }

        /*Funcion que llena el datagridview del formulario con los datos
         del inventario*/
        private DataTable Llenar_Inventario()
        {
            conexion.abrir();
            String consulta = "select *from Empleado_Productos";
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_inventario = new DataTable();

            data_adapter.Fill(tabla_inventario);
            conexion.cerrar();
            
            return tabla_inventario;
        }

        private DataTable Busqueda_Inventario(string filtro)
        {
            Console.WriteLine(filtro);
            conexion.abrir();
            String consulta = "select * from Empleado_Productos where " + filtro.ToString(); //consulta con el filtro
            data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
            tabla_inventario = new DataTable();

            //Llenado de la tabla con el data adpter
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
            txt_busqueda.Enabled = true;
        }

        /*Funcion que limpia todo el formulario y lo regresa a su 
        estado por defecto*/
        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            dgv_inventarios.DataSource = Llenar_Inventario();
            txt_busqueda.Clear();
            cbo_filtro.SelectedIndex = -1;
        }

        /*Validacion que prohibe al usuario escribir ciertos valores
         dependiendo del filtro de busqueda que ha escogio. Codigo solo permite
        numeros, Categoria solo letras, y Nombre ambos tipos*/
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
            else if(cbo_filtro.Text == "Categoria")
            {
                if(!(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back))
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
