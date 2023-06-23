using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_ManejoInventario_
{
    internal class clsReporteVentas
    {
        //Instancia de conexion a la BD
        Conexion conexion = new Conexion();

        public void MostrarInventarioVentas(DataGridView data)
        {
            try
            {
                //Llamado a un Proceso Almacenado en la BD
                SqlDataAdapter da = new SqlDataAdapter("SP_ReporteVentas1", conexion.conectardb);

                //Creacion de las columnas de la tabla, con los valores del Proceso Almacenado
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                data.DataSource = dt;
                data.Columns[0].Width = 250;
                data.Columns[0].HeaderCell.Value = "Fecha";
                data.Columns[1].Width = 100;
                data.Columns[1].HeaderCell.Value = "Total Ventas";
                data.Columns[2].Width = 150;
                data.Columns[2].HeaderCell.Value = "Impuestos";
                data.Columns[3].Width = 200;
                data.Columns[3].HeaderCell.Value = "Ganancias Generadas";

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conexion.cerrar();
            }
        }

        public void FiltroReporteVentas(DataGridView data, string fecha)
        {
            try
            {
                //Llamado a un Proceso Almacenado en la BD
                SqlDataAdapter da = new SqlDataAdapter("SP_FiltroReporteVentas", conexion.conectardb);

                //Enviar parametros al Proceso Almacenado
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha;
                DataTable dt = new DataTable();
                da.Fill(dt);
                data.DataSource = dt;
                data.Columns[0].Width = 200;
                data.Columns[0].HeaderCell.Value = "Fecha";
                data.Columns[1].Width = 100;
                data.Columns[1].HeaderCell.Value = "Total Ventas";
                data.Columns[2].Width = 150;
                data.Columns[2].HeaderCell.Value = "Impuestos";
                data.Columns[3].Width = 200;
                data.Columns[3].HeaderCell.Value = "Ganancias Generadas";

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conexion.cerrar();
            }
        }
    }
}
