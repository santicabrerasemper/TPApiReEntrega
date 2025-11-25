using System;
using System.Collections.Generic;
using System.Linq;
using Dominio;
using ConexionBD;

namespace Negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> ListarArticulos()
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT  a.Id,
                            a.Codigo,
                            a.Nombre,
                            a.Descripcion,
                            a.Precio,
                            m.Descripcion AS Marca,
                            c.Descripcion AS Categoria,
                            i.ImagenUrl
                    FROM ARTICULOS a
                    INNER JOIN MARCAS m      ON a.IdMarca = m.Id
                    INNER JOIN CATEGORIAS c  ON a.IdCategoria = c.Id
                    LEFT JOIN IMAGENES i     ON a.Id = i.IdArticulo
                    ORDER BY a.Id, i.Id;");

                datos.ejecutarLectura();

                var diccionario = new Dictionary<int, Articulo>();

                while (datos.Lector.Read())
                {
                    int id = (int)datos.Lector["Id"];

                    if (!diccionario.ContainsKey(id))
                    {
                        diccionario[id] = new Articulo
                        {
                            Id = id,
                            Codigo = datos.Lector["Codigo"].ToString(),
                            Nombre = datos.Lector["Nombre"].ToString(),
                            Descripcion = datos.Lector["Descripcion"].ToString(),
                            Precio = (decimal)datos.Lector["Precio"],
                            Marca = datos.Lector["Marca"].ToString(),
                            Categoria = datos.Lector["Categoria"].ToString(),
                            Imagenes = new List<string>()
                        };
                    }

                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        diccionario[id].Imagenes.Add(datos.Lector["ImagenUrl"].ToString());
                }

                return diccionario.Values.ToList();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Articulo ObtenerArticuloPorId(int idArticulo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT  a.Id,
                            a.Codigo,
                            a.Nombre,
                            a.Descripcion,
                            a.Precio,
                            m.Descripcion AS Marca,
                            c.Descripcion AS Categoria,
                            i.ImagenUrl
                    FROM ARTICULOS a
                    INNER JOIN MARCAS m     ON a.IdMarca = m.Id
                    INNER JOIN CATEGORIAS c ON a.IdCategoria = c.Id
                    LEFT JOIN IMAGENES i    ON a.Id = i.IdArticulo
                    WHERE a.Id = @id
                    ORDER BY a.Id, i.Id;");

                datos.setearParametro("@id", idArticulo);
                datos.ejecutarLectura();

                Articulo articulo = null;

                while (datos.Lector.Read())
                {
                    if (articulo == null)
                    {
                        articulo = new Articulo
                        {
                            Id = (int)datos.Lector["Id"],
                            Codigo = datos.Lector["Codigo"].ToString(),
                            Nombre = datos.Lector["Nombre"].ToString(),
                            Descripcion = datos.Lector["Descripcion"].ToString(),
                            Precio = (decimal)datos.Lector["Precio"],
                            Marca = datos.Lector["Marca"].ToString(),
                            Categoria = datos.Lector["Categoria"].ToString(),
                            Imagenes = new List<string>()
                        };
                    }

                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        articulo.Imagenes.Add(datos.Lector["ImagenUrl"].ToString());
                }

                return articulo;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Articulo> BuscarArticulos(string q)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    SELECT  a.Id,
                            a.Codigo,
                            a.Nombre,
                            a.Descripcion,
                            a.Precio,
                            m.Descripcion AS Marca,
                            c.Descripcion AS Categoria,
                            i.ImagenUrl
                    FROM ARTICULOS a
                    INNER JOIN MARCAS m      ON a.IdMarca = m.Id
                    INNER JOIN CATEGORIAS c  ON a.IdCategoria = c.Id
                    LEFT JOIN IMAGENES i     ON a.Id = i.IdArticulo
                    WHERE a.Codigo      LIKE @q
                       OR a.Nombre      LIKE @q
                       OR a.Descripcion LIKE @q
                    ORDER BY a.Id, i.Id;");

                datos.setearParametro("@q", "%" + q + "%");
                datos.ejecutarLectura();

                var diccionario = new Dictionary<int, Articulo>();

                while (datos.Lector.Read())
                {
                    int id = (int)datos.Lector["Id"];

                    if (!diccionario.ContainsKey(id))
                    {
                        diccionario[id] = new Articulo
                        {
                            Id = id,
                            Codigo = datos.Lector["Codigo"].ToString(),
                            Nombre = datos.Lector["Nombre"].ToString(),
                            Descripcion = datos.Lector["Descripcion"].ToString(),
                            Precio = (decimal)datos.Lector["Precio"],
                            Marca = datos.Lector["Marca"].ToString(),
                            Categoria = datos.Lector["Categoria"].ToString(),
                            Imagenes = new List<string>()
                        };
                    }

                    if (!(datos.Lector["ImagenUrl"] is DBNull))
                        diccionario[id].Imagenes.Add(datos.Lector["ImagenUrl"].ToString());
                }

                return diccionario.Values.ToList();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public bool ExisteMarca(int idMarca)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT COUNT(*) FROM MARCAS WHERE Id = @id");
                datos.setearParametro("@id", idMarca);
                var resultado = datos.ejecutarScalar();
                int cantidad = Convert.ToInt32(resultado);
                return cantidad > 0;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public bool ExisteCategoria(int idCategoria)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT COUNT(*) FROM CATEGORIAS WHERE Id = @id");
                datos.setearParametro("@id", idCategoria);
                var resultado = datos.ejecutarScalar();
                int cantidad = Convert.ToInt32(resultado);
                return cantidad > 0;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public bool ExisteCodigoArticulo(string codigo, int? idExcluir)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                if (idExcluir.HasValue)
                {
                    datos.setearConsulta(@"
                        SELECT COUNT(*)
                        FROM ARTICULOS
                        WHERE Codigo = @codigo AND Id <> @id");
                    datos.setearParametro("@codigo", codigo);
                    datos.setearParametro("@id", idExcluir.Value);
                }
                else
                {
                    datos.setearConsulta(@"
                        SELECT COUNT(*)
                        FROM ARTICULOS
                        WHERE Codigo = @codigo");
                    datos.setearParametro("@codigo", codigo);
                }

                var resultado = datos.ejecutarScalar();
                int cantidad = Convert.ToInt32(resultado);
                return cantidad > 0;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public int CrearArticulo(string codigo, string nombre, string descripcion,
                                 int idMarca, int idCategoria, decimal precio)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio)
                    VALUES (@Codigo, @Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);");

                datos.setearParametro("@Codigo", codigo);
                datos.setearParametro("@Nombre", nombre);
                datos.setearParametro("@Descripcion", descripcion);
                datos.setearParametro("@IdMarca", idMarca);
                datos.setearParametro("@IdCategoria", idCategoria);
                datos.setearParametro("@Precio", precio);

                object resultado = datos.ejecutarScalar();
                return Convert.ToInt32(resultado);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void AgregarImagenes(int idArticulo, List<string> urls)
        {
            if (urls == null || urls.Count == 0)
                return;

            AccesoDatos datos = new AccesoDatos();
            try
            {
                foreach (var url in urls)
                {
                    datos.setearConsulta(@"
                        INSERT INTO IMAGENES (IdArticulo, ImagenUrl)
                        VALUES (@IdArticulo, @ImagenUrl);");

                    datos.setearParametro("@IdArticulo", idArticulo);
                    datos.setearParametro("@ImagenUrl", url);

                    datos.ejecutarAccion();
                }
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void ModificarArticulo(int id, string codigo, string nombre, string descripcion,
                                      int idMarca, int idCategoria, decimal precio)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
                    UPDATE ARTICULOS
                    SET Codigo      = @Codigo,
                        Nombre      = @Nombre,
                        Descripcion = @Descripcion,
                        IdMarca     = @IdMarca,
                        IdCategoria = @IdCategoria,
                        Precio      = @Precio
                    WHERE Id = @Id");

                datos.setearParametro("@Codigo", codigo);
                datos.setearParametro("@Nombre", nombre);
                datos.setearParametro("@Descripcion", descripcion);
                datos.setearParametro("@IdMarca", idMarca);
                datos.setearParametro("@IdCategoria", idCategoria);
                datos.setearParametro("@Precio", precio);
                datos.setearParametro("@Id", id);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void EliminarArticulo(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("DELETE FROM IMAGENES WHERE IdArticulo = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();

                datos.setearConsulta("UPDATE Vouchers SET IdArticulo = NULL WHERE IdArticulo = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();

                datos.setearConsulta("DELETE FROM ARTICULOS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
