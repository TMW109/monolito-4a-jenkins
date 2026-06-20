using System;
using System.Collections.Generic;
using System.Linq;
using Capa_Datos;

namespace Capa_Negocio
{
    public class CN_tbl_producto
    {
        public static List<tbl_producto> ListarProducto()
        {
            using (var db = new DataClasses1DataContext())
            {
                var lista = from p in db.tbl_producto
                            where p.pro_estado == 'A'
                            orderby p.pro_id descending
                            select p;

                return lista.ToList();
            }
        }

        public static tbl_producto ObtenerProducto(int id)
        {
            using (var db = new DataClasses1DataContext())
            {
                var producto = (from p in db.tbl_producto
                                where p.pro_id == id
                                && p.pro_estado == 'A'
                                select p).FirstOrDefault();

                return producto;
            }
        }

        public static int InsertarProducto(string nombre, int cantidad, decimal precio, int provId)
        {
            using (var db = new DataClasses1DataContext())
            {
                tbl_producto producto = new tbl_producto();

                producto.pro_nombre = nombre;
                producto.pro_cantidad = cantidad;
                producto.pro_precio = precio;
                producto.pro_estado = 'A';
                producto.prov_id = provId;

                db.tbl_producto.InsertOnSubmit(producto);
                db.SubmitChanges();

                return producto.pro_id;
            }
        }

        public static bool ActualizarProducto(int id, string nombre, int cantidad, decimal precio, int provId)
        {
            using (var db = new DataClasses1DataContext())
            {
                var producto = (from p in db.tbl_producto
                                where p.pro_id == id
                                select p).FirstOrDefault();

                if (producto == null)
                    return false;

                producto.pro_nombre = nombre;
                producto.pro_cantidad = cantidad;
                producto.pro_precio = precio;
                producto.prov_id = provId;

                db.SubmitChanges();
                return true;
            }
        }

        public static bool EliminarProducto(int id)
        {
            using (var db = new DataClasses1DataContext())
            {
                var producto = (from p in db.tbl_producto
                                where p.pro_id == id
                                select p).FirstOrDefault();

                if (producto == null)
                    return false;

                producto.pro_estado = 'I';

                db.SubmitChanges();
                return true;
            }
        }

        public static List<tbl_producto> BuscarProducto(string texto)
        {
            using (var db = new DataClasses1DataContext())
            {
                var lista = from p in db.tbl_producto
                            where p.pro_estado == 'A'
                            && p.pro_nombre.Contains(texto)
                            orderby p.pro_nombre
                            select p;

                return lista.ToList();
            }
        }



        //METODO PRA VERIFICAR PRODUCOT UNICO POR NOMBRE
        public static bool verificarproductounico(string nombre)
        {
            using (var db = new DataClasses1DataContext())
            {
                return db.tbl_producto.Any(p => p.pro_nombre.Contains(nombre) && p.pro_estado == 'A');

            }
        }


        //excel 
        public static bool ImportarProducto(
            int id,
            string nombre,
            int cantidad,
            decimal precio,
            char estado,
            int? provId,
            string provNombre)
        {
            using (var db = new DataClasses1DataContext())
            {
                db.sp_ImportarProducto(
                    id,
                    nombre,
                    cantidad,
                    precio,
                    estado,
                    provId,
                    provNombre
                );

                return true;
            }
        }
    }
}