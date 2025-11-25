using System;
using Dominio;
using ConexionBD; // donde está AccesoDatos

namespace Negocio
{
    public class VoucherNegocio
    {

        public Voucher ObtenerPorCodigo(string codigo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT CodigoVoucher, IdCliente, FechaCanje, IdArticulo
                    FROM Vouchers
                    WHERE CodigoVoucher = @codigo");

                datos.setearParametro("@codigo", codigo);
                datos.ejecutarLectura();

                if (!datos.Lector.Read())
                    return null;

                var voucher = new Voucher
                {
                    CodigoVoucher = (string)datos.Lector["CodigoVoucher"],
                    IdCliente = datos.Lector["IdCliente"] is DBNull
                        ? (int?)null
                        : Convert.ToInt32(datos.Lector["IdCliente"]),
                    FechaCanje = datos.Lector["FechaCanje"] is DBNull
                        ? (DateTime?)null
                        : Convert.ToDateTime(datos.Lector["FechaCanje"]),
                    IdArticulo = datos.Lector["IdArticulo"] is DBNull
                        ? (int?)null
                        : Convert.ToInt32(datos.Lector["IdArticulo"])
                };

                return voucher;
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

        public bool EstaDisponible(string codigo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT COUNT(*)
                    FROM Vouchers
                    WHERE CodigoVoucher = @codigo
                      AND IdCliente IS NULL");

                datos.setearParametro("@codigo", codigo);

                object resultado = datos.ejecutarScalar();
                int cantidad = Convert.ToInt32(resultado);

                return cantidad > 0;
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

        public void CanjearVoucher(string codigo, int idCliente, int idArticulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    UPDATE Vouchers
                    SET IdCliente = @idCliente,
                        IdArticulo = @idArticulo,
                        FechaCanje = GETDATE()
                    WHERE CodigoVoucher = @codigo");

                datos.setearParametro("@codigo", codigo);
                datos.setearParametro("@idCliente", idCliente);
                datos.setearParametro("@idArticulo", idArticulo);

                datos.ejecutarAccion();
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
