using System;
using System.Collections.Generic;
using System.Linq;
using Capa_Datos;

namespace Capa_Negocio
{
    public class CN_tbl_proveedor
    {


        public static tbl_proveedor ObtenerProveedor(int id)
        {
            using (var db = new DataClasses1DataContext())
            {
                return db.tbl_proveedor
                    .FirstOrDefault(p => p.prov_id == id);
            }
        }
        public static List<tbl_proveedor> ListarProveedor()
        {
            using (var db = new DataClasses1DataContext())
            {
                return db.tbl_proveedor
                    .OrderByDescending(p => p.prov_id)
                    .ToList();
            }
        }

        public static bool InsertarProveedor(string nombre)
        {
            using (var db = new DataClasses1DataContext())
            {
                tbl_proveedor proveedor = new tbl_proveedor();

                proveedor.prov_nombre = nombre;
                proveedor.prov_estado = 'A';

                db.tbl_proveedor.InsertOnSubmit(proveedor);
                db.SubmitChanges();

                return true;
            }
        }

        public static bool ActualizarProveedor(int id, string nombre, char estado)
        {
            using (var db = new DataClasses1DataContext())
            {
                var proveedor = (from p in db.tbl_proveedor
                                 where p.prov_id == id
                                 select p).FirstOrDefault();

                if (proveedor == null)
                    return false;

                proveedor.prov_nombre = nombre;
                proveedor.prov_estado = estado;

                db.SubmitChanges();
                return true;
            }
        }

        public static bool EliminarProveedor(int id)
        {
            using (var db = new DataClasses1DataContext())
            {
                db.sp_EliminarProveedor(id);
                return true;
            }
        }

        public static List<tbl_proveedor> BuscarProveedor(string texto)
        {
            using (var db = new DataClasses1DataContext())
            {
                return db.tbl_proveedor
                    .Where(p => p.prov_nombre.Contains(texto))
                    .OrderByDescending(p => p.prov_id)
                    .ToList();
            }
        }

        public static bool ImportarProveedor(int id, string nombre, char estado)
        {
            using (var db = new DataClasses1DataContext())
            {
                db.sp_ImportarProveedor(id, nombre, estado);
                return true;
            }
        }

        public static List<tbl_proveedor> ListarProveedorActivo()
        {
            using (var db = new DataClasses1DataContext())
            {
                return db.tbl_proveedor
                    .Where(p => p.prov_estado == 'A')
                    .OrderBy(p => p.prov_nombre)
                    .ToList();
            }
        }


    }
}