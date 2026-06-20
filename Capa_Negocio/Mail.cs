using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Capa_Negocio
{
    public class Mail
    {
        private readonly string from = "anshelo.j.b@gmail.com";
        private readonly string pass = "dzfgmbskuhvokkjb";

        public bool EnviarQR(string to, byte[] qrBytes)
        {
            try
            {
                MailMessage correo = new MailMessage();

                correo.From = new MailAddress(from);
                correo.To.Add(new MailAddress(to));
                correo.Subject = "Código de verificación REQUIEM";
                correo.Body = "Escanea el código QR adjunto para continuar con el inicio de sesión.";
                correo.IsBodyHtml = false;

                MemoryStream ms = new MemoryStream(qrBytes);
                Attachment adjunto = new Attachment(ms, "codigo_qr.png", "image/png");

                correo.Attachments.Add(adjunto);

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from, pass);
                smtp.EnableSsl = true;

                smtp.Send(correo);

                adjunto.Dispose();
                ms.Dispose();
                correo.Dispose();
                smtp.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}