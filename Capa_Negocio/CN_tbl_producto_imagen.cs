using System;
using System.Collections.Generic;
using System.Linq;
using Capa_Datos;

namespace Capa_Negocio
{
    public class CN_tbl_producto_imagen
    {
        public static List<tbl_producto_imagen> ListarImagenesProducto(int proId)
        {
            using (var db = new DataClasses1DataContext())
            {
                var lista = from i in db.tbl_producto_imagen
                            where i.pro_id == proId
                            && i.pimg_estado == 'A'
                            orderby i.pimg_fecha_creacion descending
                            select i;

                return lista.ToList();
            }
        }

        public static tbl_producto_imagen ObtenerImagen(int id)
        {
            using (var db = new DataClasses1DataContext())
            {
                var imagen = (from i in db.tbl_producto_imagen
                              where i.pimg_id == id
                              && i.pimg_estado == 'A'
                              select i).FirstOrDefault();

                return imagen;
            }
        }

        public static tbl_producto_imagen ObtenerImagenPrincipal(int proId)
        {
            using (var db = new DataClasses1DataContext())
            {
                var imagen = (from i in db.tbl_producto_imagen
                              where i.pro_id == proId
                              && i.pimg_estado == 'A'
                              orderby i.pimg_fecha_creacion descending
                              select i).FirstOrDefault();

                return imagen;
            }
        }

        public static bool InsertarImagenProducto(int proId, string nombre, string ruta)
        {
            using (var db = new DataClasses1DataContext())
            {
                tbl_producto_imagen imagen = new tbl_producto_imagen();

                imagen.pro_id = proId;
                imagen.pimg_nombre = nombre;
                imagen.pimg_ruta = ruta;
                imagen.pimg_fecha_creacion = DateTime.Now;
                imagen.pimg_estado = 'A';

                db.tbl_producto_imagen.InsertOnSubmit(imagen);
                db.SubmitChanges();

                return true;
            }
        }

        public static bool ActualizarImagenProducto(int id, string nombre, string ruta)
        {
            using (var db = new DataClasses1DataContext())
            {
                var imagen = (from i in db.tbl_producto_imagen
                              where i.pimg_id == id
                              select i).FirstOrDefault();

                if (imagen == null)
                    return false;

                imagen.pimg_nombre = nombre;
                imagen.pimg_ruta = ruta;

                db.SubmitChanges();
                return true;
            }
        }

        public static bool EliminarImagenProducto(int id)
        {
            using (var db = new DataClasses1DataContext())
            {
                var imagen = (from i in db.tbl_producto_imagen
                              where i.pimg_id == id
                              select i).FirstOrDefault();

                if (imagen == null)
                    return false;

                imagen.pimg_estado = 'I';

                db.SubmitChanges();
                return true;
            }
        }

        public static bool InactivarImagenesProducto(int proId)
        {
            using (var db = new DataClasses1DataContext())
            {
                var imagenes = from i in db.tbl_producto_imagen
                               where i.pro_id == proId
                               && i.pimg_estado == 'A'
                               select i;

                foreach (var img in imagenes)
                {
                    img.pimg_estado = 'I';
                }

                db.SubmitChanges();
                return true;
            }
        }


    }
}