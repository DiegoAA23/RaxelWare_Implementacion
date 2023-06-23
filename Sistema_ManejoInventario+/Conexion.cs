using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Sistema_ManejoInventario_
{
    public class Conexion
    {
        //Cadena de conexion a la BD, especificando nombre de host y de la propia BD
        string cadena = "Data source=localhost;Initial Catalog=RaxelDB;Integrated Security=True";
        public SqlConnection conectardb = new SqlConnection();
        
        private static int codigo; //Para comprobar nivel de usuario
        public int Codigo { get => codigo; set => codigo = value; }

        public Conexion()
        {
            conectardb.ConnectionString = cadena; //Especificacion del origen de datos.
        }


        //Funcion que abre la conexion
        public void abrir()
        {
            try
            {
                conectardb.Open();
                Console.WriteLine("Conexión abierta!");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error al abrir la Base de Datos"+ex.Message);
            }
        }

        //Funcion que cierra la conexion
        public void cerrar()
        {
            conectardb.Close();
        }


    }
}
