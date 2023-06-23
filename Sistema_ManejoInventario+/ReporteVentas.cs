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

namespace Sistema_ManejoInventario_
{
    public partial class ReporteVentas : Form
    {
        //Instancia de formulario ReporteInventario
        clsReporteVentas rv = new clsReporteVentas();

        //Instancias de conexion a la BD y objetos para manejar la informacion
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_reportes;
        SqlCommand cmd;
        public ReporteVentas()
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

        //Rellenar el datagridview a la hora de cargar el formulario, y limitar los valores del datetimepicker
        private void ReporteVentas_Load(object sender, EventArgs e)
        {
            conexion.abrir();
            clsReporteVentas rv = new clsReporteVentas();
            rv.MostrarInventarioVentas(dataGridView1);
            conexion.cerrar();
            dtpFiltro.MaxDate = DateTime.Now;

            conexion.abrir();
            System.DateTime minimo;
            cmd = new SqlCommand("Select * From Reportes", conexion.conectardb);
            string query = ("SELECT MIN(Fecha) AS Fecha FROM Factura");
            SqlCommand com = new SqlCommand(query, conexion.conectardb);
            SqlDataReader reg = com.ExecuteReader();
            while (reg.Read())
            {
                minimo = (((DateTime)reg["Fecha"]));
                dtpFiltro.MinDate = minimo;
            }
            conexion.cerrar(); 
        }

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNormal_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnNormal.Visible = false;
            BtnMaximizar.Visible = true;
        }

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //Permite el movimiento libre del formulario
        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Resultados que coincidan con la busqueda
            conexion.abrir();
            string fecha = dtpFiltro.Value.ToString("yyyy/MM/dd");
            clsReporteVentas rv = new clsReporteVentas();
            rv.FiltroReporteVentas(dataGridView1, fecha);
            if (dataGridView1.Rows.Count < 2)
            {
                MessageBox.Show("La búsqueda no encontró resultados.", "BUSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                conexion.abrir();
                rv.MostrarInventarioVentas(dataGridView1);
                conexion.cerrar();
                dtpFiltro.MaxDate = DateTime.Now;
                dtpFiltro.Value = dtpFiltro.MaxDate;
            }
            conexion.cerrar();
        }

        //Generacion del reporte impreso en un documento Excel
        private void button2_Click(object sender, EventArgs e)
        {
            //Seleccion de todos los elementos en la tabla
            dataGridView1.SelectAll();

            //Creacion de los objetos para la creacion del documento Excel
            DataObject copydata = dataGridView1.GetClipboardContent();
            if (copydata != null) Clipboard.SetDataObject(copydata);
            Microsoft.Office.Interop.Excel.Application xlapp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWbook;
            Microsoft.Office.Interop.Excel.Worksheet xlsheet;
            object miseddata = System.Reflection.Missing.Value;
            xlWbook = xlapp.Workbooks.Add(miseddata);


            //Llenado de las celdas en el documento
            xlsheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWbook.Worksheets.get_Item(1);
            Microsoft.Office.Interop.Excel.Range rango = (Microsoft.Office.Interop.Excel.Range)xlsheet.Cells[4, 1];
            rango.Select();

            xlsheet.PasteSpecial(rango, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);


            //Creacion de encabezado del Reporte
            string fechaactual = DateTime.Now.ToString("yyyy/MM/dd");
            xlapp.Cells[1, 3] = "Generado el " + fechaactual;
            xlapp.Cells[1, 4] = "Raxel Baterias & Mas";
            xlapp.Cells[1, 5] = "Reporte de Inventario";
            xlapp.Cells[1, 3].Font.Bold = true;
            xlapp.Cells[1, 4].Font.Bold = true;
            xlapp.Cells[1, 5].Font.Bold = true;

            conexion.abrir();
            int numero = 0;
            cmd = new SqlCommand("Select * From Reportes", conexion.conectardb);
            string query = ("SELECT MAX(Numero) AS Numero FROM Reportes");
            SqlCommand com = new SqlCommand(query, conexion.conectardb);
            SqlDataReader reg = com.ExecuteReader();
            while (reg.Read())
            {
                numero = Convert.ToInt16((reg["Numero"]));
            }
            conexion.cerrar();

            numero = numero + 1;
            xlapp.Cells[1, 2] = "Reporte #" + numero.ToString();
            xlapp.Cells[1, 2].Font.Bold = true;

            for (int x = 0; x < 4; x++)
            {
                xlapp.Cells[3, x + 2].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            }

            int cantidad = dataGridView1.Rows.Count;

            for (int fila = 4; fila <= cantidad + 3; fila++)
            {
                for (int columna = 0; columna < dataGridView1.Columns.Count; columna++)
                {
                    xlapp.Cells[fila, columna + 2].Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                }
            }

            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                xlapp.Cells[3, i+1] = dataGridView1.Columns[i - 1].HeaderText;
                xlapp.Cells[3, i+1].Font.Bold = true;
                xlapp.Cells[3, i+1].HorizontalAlignment = HorizontalAlignment.Center;
            }

            //Apertura del archivo generado con el reporte
            MessageBox.Show("Su reporte se ha generado exitosamente", "REPORTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            xlapp.Columns.AutoFit();
            xlapp.Visible = true;

            bool estado = true;
            //Guardado del reporte en la BD
            conexion.abrir();
            cmd = new SqlCommand("Insert Into Reportes Values('Ventas', 1, '" + fechaactual + "', '"+ estado + "')", conexion.conectardb);
            cmd.ExecuteNonQuery();
            conexion.cerrar();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Limpieza del formulario para resetearlo a su estado por defecto
        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            conexion.abrir();
            clsReporteVentas rv = new clsReporteVentas();
            rv.MostrarInventarioVentas(dataGridView1);
            conexion.cerrar();
            dtpFiltro.MaxDate = DateTime.Now;
            dtpFiltro.Value = dtpFiltro.MaxDate;
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnNormal.Visible = true;
            BtnMaximizar.Visible = false;
        }
    }
}
