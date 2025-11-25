using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Dominio;
using Negocio;        
using TPApi.Models;   

namespace TPApi.Controllers
{
    [RoutePrefix("api/articulos")]
    public class ArticuloController : ApiController
    {
        private readonly ArticuloNegocio _articuloNegocio;

        public ArticuloController()
        {
            _articuloNegocio = new ArticuloNegocio();
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetArticulos()
        {
            try
            {
                List<Articulo> articulos = _articuloNegocio.ListarArticulos();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("{id:int}", Name = "GetArticuloPorId")]
        public IHttpActionResult GetArticulo(int id)
        {
            try
            {
                Articulo articulo = _articuloNegocio.ObtenerArticuloPorId(id);
                if (articulo == null)
                    return NotFound();

                return Ok(articulo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("buscar")]
        public IHttpActionResult Buscar(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("El parámetro 'q' es obligatorio para buscar.");

            try
            {
                List<Articulo> resultados = _articuloNegocio.BuscarArticulos(q);
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Post([FromBody] ArticuloDTO dto)
        {
            if (dto == null)
                return BadRequest("Debe enviar los datos del producto.");

            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("Los campos 'Codigo' y 'Nombre' son obligatorios.");

            if (dto.Precio < 0)
                return BadRequest("El precio no puede ser negativo.");

            try
            {
                if (!_articuloNegocio.ExisteMarca(dto.IdMarca))
                    return BadRequest("La marca indicada no existe.");

                if (!_articuloNegocio.ExisteCategoria(dto.IdCategoria))
                    return BadRequest("La categoría indicada no existe.");

                if (_articuloNegocio.ExisteCodigoArticulo(dto.Codigo, null)) // null = alta
                    return BadRequest("Ya existe un artículo con ese código.");

                int nuevoId = _articuloNegocio.CrearArticulo(
                    dto.Codigo,
                    dto.Nombre,
                    dto.Descripcion,
                    dto.IdMarca,
                    dto.IdCategoria,
                    dto.Precio
                );

                var creado = _articuloNegocio.ObtenerArticuloPorId(nuevoId);
                return CreatedAtRoute("GetArticuloPorId", new { id = nuevoId }, creado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("{id:int}/imagenes")]
        public IHttpActionResult AgregarImagenes(int id, [FromBody] List<string> urls)
        {
            if (urls == null || urls.Count == 0)
                return BadRequest("Debe enviar una lista de URLs de imágenes.");

            if (urls.Exists(string.IsNullOrWhiteSpace))
                return BadRequest("Las URLs de las imágenes no pueden ser vacías.");

            try
            {
                var articulo = _articuloNegocio.ObtenerArticuloPorId(id);
                if (articulo == null)
                    return NotFound();

                _articuloNegocio.AgregarImagenes(id, urls);

                return StatusCode(HttpStatusCode.NoContent); // 204
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Put(int id, [FromBody] ArticuloDTO dto)
        {
            if (dto == null)
                return BadRequest("Debe enviar los datos del producto.");

            if (string.IsNullOrWhiteSpace(dto.Codigo) || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("Los campos 'Codigo' y 'Nombre' son obligatorios.");

            if (dto.Precio < 0)
                return BadRequest("El precio no puede ser negativo.");

            try
            {
                var actual = _articuloNegocio.ObtenerArticuloPorId(id);
                if (actual == null)
                    return NotFound();

                if (!_articuloNegocio.ExisteMarca(dto.IdMarca))
                    return BadRequest("La marca indicada no existe.");

                if (!_articuloNegocio.ExisteCategoria(dto.IdCategoria))
                    return BadRequest("La categoría indicada no existe.");

                if (_articuloNegocio.ExisteCodigoArticulo(dto.Codigo, id))
                    return BadRequest("Ya existe otro artículo con ese código.");

                _articuloNegocio.ModificarArticulo(
                    id,
                    dto.Codigo,
                    dto.Nombre,
                    dto.Descripcion,
                    dto.IdMarca,
                    dto.IdCategoria,
                    dto.Precio
                );

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var actual = _articuloNegocio.ObtenerArticuloPorId(id);
                if (actual == null)
                    return NotFound();

                _articuloNegocio.EliminarArticulo(id);

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
