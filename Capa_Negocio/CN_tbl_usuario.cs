using Capa_Datos;
using System;
using System.Linq;
using System.Text;


namespace Capa_Negocio
{
    public class CN_tbl_usuario
    {
        private static DataClasses1DataContext db = new DataClasses1DataContext();

        public static int RegistrarUsuario(tbl_usuario nuevo)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    if (db.tbl_usuario.Any(u => u.usu_cedula == nuevo.usu_cedula))
                        return -1;

                    if (db.tbl_usuario.Any(u => u.usu_correo == nuevo.usu_correo))
                        return -2;

                    if (db.tbl_usuario.Any(u => u.usu_nick == nuevo.usu_nick))
                        return -3;

                    db.tbl_usuario.InsertOnSubmit(nuevo);
                    db.SubmitChanges();

                    return nuevo.usu_id;
                }
            }
            catch
            {
                return -99;
            }
        }

        public static bool autetixced(string cedula)
        {
            return db.tbl_usuario.Any(u =>
                u.usu_cedula == cedula &&
                u.usu_estado == 'A'
            );
        }

        public static bool autetixpass(string cedula, string password)
        {
            return db.tbl_usuario.Any(u =>
                u.usu_cedula == cedula &&
                db.desencriptacon(u.usu_contrasenia) == password &&
                u.usu_estado == 'A'
            );
        }

        public static tbl_usuario traerusuario(string cedula, string password)
        {
            return db.tbl_usuario.FirstOrDefault(u =>
                u.usu_cedula == cedula &&
                db.desencriptacon(u.usu_contrasenia) == password &&
                u.usu_estado == 'A'
            );
        }

        public static tbl_usuario traerxced(string cedula)
        {
            return db.tbl_usuario.FirstOrDefault(u =>
                u.usu_cedula == cedula &&
                u.usu_estado == 'A'
            );
        }

        public static tbl_usuario traerxcedTodos(string cedula)
        {
            return db.tbl_usuario.FirstOrDefault(u =>
                u.usu_cedula == cedula
            );
        }

        public static void modificausu(tbl_usuario usuario)
        {
            db.SubmitChanges();
        }

        public static bool autentixcc(string cedula, string password)
        {
            return db.tbl_usuario.Any(u =>
                u.usu_cedula == cedula &&
                db.desencriptacon(u.usu_contrasenia) == password &&
                u.usu_estado == 'A'
            );
        }

        public static IQueryable<tbl_usuario> ListarUsuariosBloqueados()
        {
            return db.tbl_usuario.Where(u => u.usu_estado == 'B');
        }

        public static bool DesbloquearUsuario(int usu_id)
        {
            try
            {
                tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);

                if (usuario == null)
                    return false;

                usuario.usu_estado = 'A';
                usuario.usu_intentos = 0;
                usuario.usu_fecha_ultimo_intento = null;

                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //metodos contraseña recuperacion
        public static tbl_usuario RecuperarPorCedulaCorreo(string cedula, string correo)
        {
            using (var db = new DataClasses1DataContext())
            {
                return db.tbl_usuario.FirstOrDefault(u =>
                    u.usu_cedula == cedula &&
                    u.usu_correo == correo
                );
            }
        }

        public static bool RecuperarPassword(int usu_id, string nuevaPassword)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);

                    if (usuario == null)
                        return false;

                    byte[] bytes = Encoding.UTF8.GetBytes(nuevaPassword);
                    usuario.usu_contrasenia = new System.Data.Linq.Binary(bytes);

                    usuario.usu_codigo_OPT = null;
                    usuario.usu_estado = 'A';
                    usuario.usu_intentos = 0;
                    usuario.usu_fecha_ultimo_intento = null;

                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public static tbl_usuario ObtenerUsuarioPorCedulaTodos(string cedula)
        {
            using (var db = new DataClasses1DataContext())
            {
                return db.tbl_usuario.FirstOrDefault(u => u.usu_cedula == cedula);
            }
        }

        public static bool ValidarPassword(string cedula, string password)
        {
            using (var db = new DataClasses1DataContext())
            {
                tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u =>
                    u.usu_cedula == cedula &&
                    u.usu_estado == 'A'
                );

                if (usuario == null || usuario.usu_contrasenia == null)
                    return false;

                string passwordBD = Encoding.UTF8.GetString(usuario.usu_contrasenia.ToArray());

                return passwordBD == password;
            }
        }

        public static bool RegistrarLoginCorrecto(int usu_id, string otp)
        {
            try
            {
                using (var db = new DataClasses1DataContext())
                {
                    tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);

                    if (usuario == null)
                        return false;

                    usuario.usu_codigo_OPT = otp;
                    usuario.usu_intentos = 0;
                    usuario.usu_estado = 'A';
                    usuario.usu_fecha_ultimo_intento = null;

                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool RegistrarIntentoFallido(int usu_id)
        {
            using (var db = new DataClasses1DataContext())
            {
                tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);

                if (usuario == null)
                    return false;

                usuario.usu_fecha_ultimo_intento = DateTime.Now;
                usuario.usu_intentos = (usuario.usu_intentos ?? 0) + 1;

                bool bloqueado = false;

                if (usuario.usu_intentos >= 3)
                {
                    usuario.usu_estado = 'B';
                    bloqueado = true;
                }

                db.SubmitChanges();
                return bloqueado;
            }
        }
        public static bool RecuperarPasswordTemporalWhatsapp(string cedula, string claveTemporal)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    tbl_usuario usuario = db.tbl_usuario
                        .FirstOrDefault(u => u.usu_cedula == cedula);

                    if (usuario == null)
                        return false;

                    // ⚠️ IMPORTANTE: mismo formato que usas en login
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(claveTemporal);
                    usuario.usu_contrasenia = new System.Data.Linq.Binary(bytes);

                    // 🔥 LIMPIAR OTP viejo
                    usuario.usu_codigo_OPT = null;

                    // 🔥 DESBLOQUEAR
                    usuario.usu_estado = 'A';
                    usuario.usu_intentos = 0;
                    usuario.usu_fecha_ultimo_intento = null;

                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool LimpiarOTP(string cedula)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_cedula == cedula);

                    if (usuario == null)
                        return false;

                    usuario.usu_codigo_OPT = null;
                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // =====================================================
        // CRUD ADMIN USUARIOS
        // =====================================================

        public static IQueryable<tbl_usuario> ListarUsuariosAdmin()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            return db.tbl_usuario
                .Where(u => u.usu_estado != 'E')
                .OrderByDescending(u => u.usu_id);
        }

        public static tbl_usuario ObtenerUsuarioPorId(int usu_id)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext())
            {
                return db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);
            }
        }

        public static int RegistrarUsuarioAdmin(tbl_usuario nuevo)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    if (db.tbl_usuario.Any(u => u.usu_cedula == nuevo.usu_cedula && u.usu_estado != 'E'))
                        return -1;

                    if (db.tbl_usuario.Any(u => u.usu_correo == nuevo.usu_correo && u.usu_estado != 'E'))
                        return -2;

                    if (db.tbl_usuario.Any(u => u.usu_nick == nuevo.usu_nick && u.usu_estado != 'E'))
                        return -3;

                    db.tbl_usuario.InsertOnSubmit(nuevo);
                    db.SubmitChanges();

                    return nuevo.usu_id;
                }
            }
            catch
            {
                return -99;
            }
        }

        public static bool ActualizarUsuarioAdmin(tbl_usuario datos)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == datos.usu_id);

                    if (usuario == null)
                        return false;

                    if (db.tbl_usuario.Any(u =>
                        u.usu_id != datos.usu_id &&
                        u.usu_cedula == datos.usu_cedula &&
                        u.usu_estado != 'E'))
                        return false;

                    if (db.tbl_usuario.Any(u =>
                        u.usu_id != datos.usu_id &&
                        u.usu_correo == datos.usu_correo &&
                        u.usu_estado != 'E'))
                        return false;

                    if (db.tbl_usuario.Any(u =>
                        u.usu_id != datos.usu_id &&
                        u.usu_nick == datos.usu_nick &&
                        u.usu_estado != 'E'))
                        return false;

                    usuario.usu_cedula = datos.usu_cedula;
                    usuario.usu_nombres = datos.usu_nombres;
                    usuario.usu_apellidos = datos.usu_apellidos;
                    usuario.usu_direccion = datos.usu_direccion;
                    usuario.usu_celular = datos.usu_celular;
                    usuario.usu_correo = datos.usu_correo;
                    usuario.usu_fecha_cumple = datos.usu_fecha_cumple;
                    usuario.usu_nick = datos.usu_nick;
                    usuario.tusu_id = datos.tusu_id;
                    usuario.usu_estado = datos.usu_estado;

                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CambiarPasswordAdmin(int usu_id, string nuevaPassword)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);

                    if (usuario == null)
                        return false;

                    byte[] bytes = Encoding.UTF8.GetBytes(nuevaPassword);
                    usuario.usu_contrasenia = new System.Data.Linq.Binary(bytes);

                    usuario.usu_codigo_OPT = null;
                    usuario.usu_intentos = 0;
                    usuario.usu_fecha_ultimo_intento = null;

                    db.SubmitChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool EliminarUsuarioLogico(int usu_id)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);

                    if (usuario == null)
                        return false;

                    usuario.usu_estado = 'E';

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

        public static bool UsuarioTieneVinculos(int usu_id)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    // aquí puedes agregar más tablas en el futuro
                    bool tieneImagenes = db.tbl_usuario_imagen.Any(i => i.usu_id == usu_id);

                    return tieneImagenes;
                }
            }
            catch
            {
                return true;
            }
        }

        public static bool EliminarUsuarioFisico(int usu_id)
        {
            try
            {
                using (DataClasses1DataContext db = new DataClasses1DataContext())
                {
                    // 🔴 BLOQUEA SI TIENE RELACIONES
                    if (UsuarioTieneVinculos(usu_id))
                        return false;

                    tbl_usuario usuario = db.tbl_usuario.FirstOrDefault(u => u.usu_id == usu_id);

                    if (usuario == null)
                        return false;

                    db.tbl_usuario.DeleteOnSubmit(usuario);
                    db.SubmitChanges();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool TieneImagenes(int usu_id)
        {
            using (DataClasses1DataContext db = new DataClasses1DataContext())
            {
                return db.tbl_usuario_imagen.Any(i => i.usu_id == usu_id);
            }
        }


    }
}