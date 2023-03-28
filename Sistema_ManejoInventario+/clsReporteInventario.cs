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
    internal class clsReporteInventario
    {
        Conexion cone = new Conexion();

        public void MostrarInventario(DataGridView data)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SP_reporteInventario", cone.conectardb);

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                data.DataSource = dt;
                data.Columns[0].Width = 60;
                data.Columns[0].HeaderCell.Value = "Codigo";
                data.Columns[1].Width = 250;
                data.Columns[1].HeaderCell.Value = "Nombre";
                data.Columns[2].Width = 50;
                data.Columns[2].HeaderCell.Value = "Stock";
                data.Columns[3].Width = 170;
                data.Columns[3].HeaderCell.Value = "Precio De Compra";
                data.Columns[4].Width = 170;
                data.Columns[4].HeaderCell.Value = "Precio De Venta";
                data.Columns[5].Width = 100;
                data.Columns[5].HeaderCell.Value = "Distribuidor";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cone.cerrar();
            }
        }

        public void Buscar(DataGridView data, string buscarnombre)
        {
            try
            {
                cone.abrir();
                SqlCommand sql = new SqlCommand("filtro_busqueda", cone.conectardb);
                sql.CommandType = CommandType.StoredProcedure;
                sql.Parameters.Add("@filtro", SqlDbType.VarChar, 200).Value = buscarnombre;

                sql.ExecuteNonQuery();
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql);
                da.Fill(dt);
                data.DataSource = dt;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cone.cerrar();
            }
        }
    }
}
