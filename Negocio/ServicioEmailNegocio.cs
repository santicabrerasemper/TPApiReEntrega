using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class ServicioEmailNegocio
    {
        private const string SmtpHost = "smtp.outlook.com";
        private const int SmtpPort = 587;
        private const bool Ssl = true;

        private const string Usuario = "pruebasfacultad@outlook.es";
        private const string Clave = "Hola123#";
        private const string FromName = "TuElectro";
        public void Enviar(string destino, string asunto, string htmlBody)
        {
            using (var msg = new MailMessage())
            using (var smtp = new SmtpClient(SmtpHost, SmtpPort))
            {
                msg.From = new MailAddress(Usuario, FromName, Encoding.UTF8);
                msg.To.Add(destino);
                msg.Subject = asunto;
                msg.SubjectEncoding = Encoding.UTF8;
                msg.Body = htmlBody;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = true;

                smtp.EnableSsl = Ssl;
                smtp.Credentials = new NetworkCredential(Usuario, Clave);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Timeout = 15000;

                smtp.Send(msg);
            }
        }
    }
}
