using System;
using Dominio;
using ConexionBD; 

namespace Negocio
{
    public class ClienteNegocio
    {
        public Cliente ObtenerPorDocumento(string documento)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT Id, Documento, Nombre, Apellido, Email, Direccion, Ciudad, CP
                    FROM Clientes
                    WHERE Documento = @doc");

                datos.setearParametro("@doc", documento);
                datos.ejecutarLectura();

                if (!datos.Lector.Read())
                    return null;

                Cliente cliente = new Cliente
                {
                    Id = (int)datos.Lector["Id"],
                    Documento = (string)datos.Lector["Documento"],
                    Nombre = (string)datos.Lector["Nombre"],
                    Apellido = (string)datos.Lector["Apellido"],
                    Email = (string)datos.Lector["Email"],
                    Direccion = (string)datos.Lector["Direccion"],
                    Ciudad = (string)datos.Lector["Ciudad"],
                    CP = (int)datos.Lector["CP"]
                };

                return cliente;
            }
            catch (Exception ex)
            {        
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public int Insertar(Cliente c)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    INSERT INTO Clientes
                        (Documento, Nombre, Apellido, Email, Direccion, Ciudad, CP)
                    VALUES
                        (@Documento, @Nombre, @Apellido, @Email, @Direccion, @Ciudad, @CP);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);");

                datos.setearParametro("@Documento", c.Documento);
                datos.setearParametro("@Nombre", c.Nombre);
                datos.setearParametro("@Apellido", c.Apellido);
                datos.setearParametro("@Email", c.Email);
                datos.setearParametro("@Direccion", c.Direccion);
                datos.setearParametro("@Ciudad", c.Ciudad);
                datos.setearParametro("@CP", c.CP);

                object resultado = datos.ejecutarScalar();
                return Convert.ToInt32(resultado);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
