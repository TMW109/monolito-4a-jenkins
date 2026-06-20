using Capa_Datos;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace Capa_Negocio
{
    public class CN_tbl_usuario_imagen
    {
        public static string GuardarImagen(int usu_id, byte[] imagen, string nombre, string tipo)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    tbl_usuario_imagen img = new tbl_usuario_imagen();

                    img.usu_id = usu_id;
                    img.uimg_nombre = nombre;
                    img.uimg_tipo = tipo;
                    img.uimg_data = new Binary(imagen);
                    img.uimg_estado = 'A';

                    db.tbl_usuario_imagen.InsertOnSubmit(img);
                    db.SubmitChanges();

                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }


        public static bool GuardarImagenesUsuario(int usu_id, List<byte[]> imagenes, List<string> nombres, List<string> tipos)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    for (int i = 0; i < imagenes.Count; i++)
                    {
                        tbl_usuario_imagen img = new tbl_usuario_imagen();

                        img.usu_id = usu_id;
                        img.uimg_nombre = nombres[i];
                        img.uimg_tipo = tipos[i];
                        img.uimg_data = new Binary(imagenes[i]);
                        img.uimg_fecha_creacion = DateTime.Now;
                        img.uimg_estado = 'A';

                        db.tbl_usuario_imagen.InsertOnSubmit(img);
                    }

                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static IQueryable<tbl_usuario_imagen> ListarImagenesUsuario(int usu_id)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            return db.tbl_usuario_imagen
                .Where(i => i.usu_id == usu_id && i.uimg_estado == 'A')
                .OrderByDescending(i => i.uimg_id);
        }

        public static bool EliminarImagenLogica(int uimg_id)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    tbl_usuario_imagen img = db.tbl_usuario_imagen.FirstOrDefault(i => i.uimg_id == uimg_id);

                    if (img == null)
                        return false;

                    img.uimg_estado = 'E';
                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool EliminarImagenFisica(int uimg_id)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    tbl_usuario_imagen img = db.tbl_usuario_imagen.FirstOrDefault(i => i.uimg_id == uimg_id);

                    if (img == null)
                        return false;

                    db.tbl_usuario_imagen.DeleteOnSubmit(img);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool EliminarImagenesFisicasPorUsuario(int usu_id)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    var imagenes = db.tbl_usuario_imagen.Where(i => i.usu_id == usu_id);

                    db.tbl_usuario_imagen.DeleteAllOnSubmit(imagenes);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool EliminarImagenesLogicasPorUsuario(int usu_id)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    var imagenes = db.tbl_usuario_imagen.Where(i => i.usu_id == usu_id);

                    foreach (tbl_usuario_imagen img in imagenes)
                    {
                        img.uimg_estado = 'E';
                    }

                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


    }
}