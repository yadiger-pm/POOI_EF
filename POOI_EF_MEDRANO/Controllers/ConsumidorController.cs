using POOI_EF_MEDRANO.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POOI_EF_MEDRANO.Controllers
{
    public class ConsumidorController : Controller
    {
        string cadena = ConfigurationManager.ConnectionStrings["cadenaConexion"].ConnectionString;

        IEnumerable<Consumidor> listaConsumidores()
        {
            List<Consumidor> listaConsumidor = new List<Consumidor>();
            
            SqlConnection sqlConnection = new SqlConnection(cadena);//indica la conexión
            sqlConnection.Open();//Aperturando la conexion a SQL
            SqlCommand sqlcommand = new SqlCommand("usp_listar_consumidor", sqlConnection);//Crear el query o comando de ejecución ;
            sqlcommand.CommandType = CommandType.StoredProcedure;
            SqlDataReader sqlDataReader =sqlcommand.ExecuteReader();//Ejecutando el query

            while (sqlDataReader.Read())
            {
                // Se mantiene el IsDBNull para evitar errores con correos vacíos
                listaConsumidor.Add(new Consumidor(
                   sqlDataReader.GetString(0),
                   sqlDataReader.GetString(1),
                   sqlDataReader.GetString(2),
                   sqlDataReader.IsDBNull(3) ? "" : sqlDataReader.GetString(3),//precaución con los correos vacíos
                   sqlDataReader.GetString(4)
                   )
                 );
            }
            sqlDataReader.Close();
            sqlConnection.Close();

            return listaConsumidor;
        }
        public ActionResult Index()
        {
            return View(listaConsumidores());
        }
        public ActionResult Create()
        {
            return View(new Consumidor());
        }

        [HttpPost]
        public ActionResult Create(Consumidor reg)
        {
            string guardarConsumidor(Consumidor consumidor)
            {
                SqlConnection sqlConnection = new SqlConnection(cadena);
                sqlConnection.Open();
                SqlCommand sqlcommand = new SqlCommand("usp_agrega_consumidor", sqlConnection);
                sqlcommand.CommandType = CommandType.StoredProcedure;

                sqlcommand.Parameters.AddWithValue("@dnicont", reg.DNI);
                sqlcommand.Parameters.AddWithValue("@nomcont", reg.Nombres);
                sqlcommand.Parameters.AddWithValue("@apecont", reg.Apellidos);
                sqlcommand.Parameters.AddWithValue("@emailcont", reg.Email);
                sqlcommand.Parameters.AddWithValue("@idpais", reg.Pais);

                sqlcommand.Parameters.Add("@mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                sqlcommand.ExecuteNonQuery(); // Se ejecuta el procedimiento
                string resultado = sqlcommand.Parameters["@mensaje"].Value.ToString();
                sqlConnection.Close();

                return resultado;
            }

            string mensaje = guardarConsumidor(reg);
            ViewBag.Mensaje = mensaje;
            return View(reg);
        }
        Consumidor recuperarConsumidorPorId(string id) 
        {
            Consumidor consumidor = new Consumidor();
            SqlConnection sqlConnection = new SqlConnection(cadena); 
            sqlConnection.Open(); 
            SqlCommand sqlcommand = new SqlCommand("SELECT * FROM tb_Consumidor WHERE dnicont = @id", sqlConnection);

            sqlcommand.Parameters.AddWithValue("@id", id);
            SqlDataReader sqlDataReader = sqlcommand.ExecuteReader();

            while (sqlDataReader.Read()) 
            {
                consumidor = new Consumidor(
                    sqlDataReader.GetString(0), // DNI
                    sqlDataReader.GetString(1), // Nombres
                    sqlDataReader.GetString(2), // Apellidos
                    sqlDataReader.IsDBNull(3) ? "" : sqlDataReader.GetString(3), // Email validado
                    sqlDataReader.GetString(4)  // Pais
                );
            }

            sqlDataReader.Close();
            sqlConnection.Close();

            return consumidor;
        }
        public ActionResult Details(string id)
        {
            Consumidor consumidorExistente = recuperarConsumidorPorId(id);
            return View(consumidorExistente);
        }

        string eliminarConsumidorPorId(string Id)
        {
            SqlConnection sqlConnection = new SqlConnection(cadena);
            sqlConnection.Open();
            SqlCommand sqlcommand = new SqlCommand("usp_eliminar_consumidor", sqlConnection);
            sqlcommand.CommandType = CommandType.StoredProcedure;

            sqlcommand.Parameters.AddWithValue("@dnicont", Id);
            int cantidad = sqlcommand.ExecuteNonQuery();
            string mensaje = cantidad > 0 ? "Registro eliminado correctamente" : "No se encontró el registro para eliminar";

            sqlConnection.Close();
            return mensaje; 
        }

        public ActionResult Delete(string Id)
        {
            Consumidor consumidorExistente = recuperarConsumidorPorId(Id);
            return View(consumidorExistente);
        }

        [HttpPost]
        public ActionResult Delete(Consumidor reg)
        {
            string dniReal = RouteData.Values["id"].ToString();
            string mensaje = eliminarConsumidorPorId(dniReal);
            ViewBag.Mensaje = mensaje;
            return View("Index", listaConsumidores());
        }

        string actualizarConsumidorBd(Consumidor reg)
        {
            SqlConnection sqlConnection = new SqlConnection(cadena);
            sqlConnection.Open();
            string query = "UPDATE tb_Consumidor SET nomcont = @nomcont, apecont = @apecont, emailcont = @emailcont, idpais = @idpais WHERE dnicont = @dnicont";
            SqlCommand sqlcommand = new SqlCommand(query, sqlConnection);
            sqlcommand.CommandType = CommandType.Text;

            sqlcommand.Parameters.AddWithValue("@dnicont", reg.DNI);
            sqlcommand.Parameters.AddWithValue("@nomcont", reg.Nombres);
            sqlcommand.Parameters.AddWithValue("@apecont", reg.Apellidos);
            sqlcommand.Parameters.AddWithValue("@emailcont", reg.Email);
            sqlcommand.Parameters.AddWithValue("@idpais", reg.Pais);

            int cantidad = sqlcommand.ExecuteNonQuery();
            string mensaje = cantidad > 0 ? "Registro actualizado correctamente" : "No se pudo actualizar el registro";

            sqlConnection.Close();
            return mensaje;
        }
        public ActionResult Update(string id)
        {
            Consumidor consumidorExistente = recuperarConsumidorPorId(id);
            return View(consumidorExistente);
        }
        [HttpPost]
        public ActionResult Update(Consumidor reg)
        {
            string mensaje = actualizarConsumidorBd(reg);
            ViewBag.Mensaje = mensaje;
            return View(reg);
        }
    }
}