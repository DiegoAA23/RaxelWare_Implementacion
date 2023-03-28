using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_ManejoInventario_
{
    public partial class ReporteInventario : Form
    {
        clsReporteInventario ri = new clsReporteInventario();
        Conexion conexion = new Conexion();
        SqlDataAdapter data_adapter;
        DataTable tabla_reportes;
        SqlCommand cmd;
        public ReporteInventario()
        {
            InitializeComponent();
        }


        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

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

        private void ReporteInventario_Load(object sender, EventArgs e)
        {
            conexion.abrir();
            ri.MostrarInventario(dataGridView1);
            conexion.cerrar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ri.Buscar(dataGridView1, this.txtBusq.Text.Trim());
        }

        private void validarForm()
        {
            if (errorProvider1 != null)
            {
                if (txtBusq.Text == String.Empty)
                {
                    errorProvider1.SetError(txtBusq, "Este Campo es obligatorio");
                    txtBusq.Focus();
                    return;
                }
                else
                {
                    errorProvider1.Clear();
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBusq.Text == String.Empty)
                {
                    validarForm();
                    MessageBox.Show("Ingrese un codigo para buscar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    conexion.abrir();
                    int cod = Convert.ToInt16(txtBusq.Text);
                    String consulta = "Select * from Productos where Codigo = " + cod;
                    data_adapter = new SqlDataAdapter(consulta, conexion.conectardb);
                    tabla_reportes = new DataTable();
                    data_adapter.Fill(tabla_reportes);
                    dataGridView1.DataSource = tabla_reportes;
                    if (dataGridView1.Rows.Count == 0)
                    {
                        MessageBox.Show("La busqueda no encotro resultados.", "BUSQUEDA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        conexion.abrir();
                        ri.MostrarInventario(dataGridView1);
                        conexion.cerrar();
                        txtBusq.Text = "";
                    }
                    validarForm();
                    conexion.cerrar();
                }
            }
            catch
            {
                MessageBox.Show("Ingrese un codigo valido", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtBusq.Clear();
            }
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

        private void BarraTop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BarraTop_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectAll();
            DataObject copydata = dataGridView1.GetClipboardContent();
            if (copydata != null) Clipboard.SetDataObject(copydata);
            Microsoft.Office.Interop.Excel.Application xlapp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWbook;
            Microsoft.Office.Interop.Excel.Worksheet xlsheet;
            object miseddata = System.Reflection.Missing.Value;
            xlWbook = xlapp.Workbooks.Add(miseddata);

            xlsheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWbook.Worksheets.get_Item(1);
            Microsoft.Office.Interop.Excel.Range rango = (Microsoft.Office.Interop.Excel.Range)xlsheet.Cells[2, 1];
            rango.Select();

            xlsheet.PasteSpecial(rango, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);

            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                xlapp.Cells[1, i + 1] = dataGridView1.Columns[i - 1].HeaderText;
                xlapp.Cells[1, i + 1].Font.Bold = true;
                xlapp.Cells[1, i + 1].HorizontalAlignment = HorizontalAlignment.Center;
            }

            MessageBox.Show("Su reporte se ha generado exitosamente", "REPORTE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            xlapp.Columns.AutoFit();
            xlapp.Visible = true;

            string fechaactual = DateTime.Now.ToString("yyyy/MM/dd");

            conexion.abrir();
            cmd = new SqlCommand("Insert Into Reportes Values('Inventario', 1, '"+ fechaactual + "')", conexion.conectardb);
            cmd.ExecuteNonQuery();
            conexion.cerrar();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            conexion.abrir();
            ri.MostrarInventario(dataGridView1);
            conexion.cerrar();

            txtBusq.Text = "";
        }

        private void BtnMaximizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            btnNormal.Visible = true;
            BtnMaximizar.Visible = false;
        }

        private void txtBusq_KeyPress(object sender, KeyPressEventArgs e)
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
