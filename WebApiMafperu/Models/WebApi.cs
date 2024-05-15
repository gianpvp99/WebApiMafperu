using NLog;
using System;
using RestSharp;
using System.Net;
using System.Text;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace WebApiMafperu.Models
{
    public class WebApi
    {
        private static string token = "";
        private static string clientID = "";
        private static string clientSecret = "";        
        private string urlEPA = "";

        private static string emailVehiculo = System.Configuration.ConfigurationManager.AppSettings["EmailVehicular"].ToString();
        private static string emailRemitente = System.Configuration.ConfigurationManager.AppSettings["EmailRemitente"].ToString();
        private static string emailContacto = System.Configuration.ConfigurationManager.AppSettings["EmailContacto"].ToString();
        private static string emailReclamaciones = System.Configuration.ConfigurationManager.AppSettings["EmailReclamaciones"].ToString();
        private static string emailNotificacionReclamo = System.Configuration.ConfigurationManager.AppSettings["EmailNotificacionReclamo"].ToString();
        private static string emailNotificacionContacto = System.Configuration.ConfigurationManager.AppSettings["EmailNotificacionContacto"].ToString();

        private static string servidor = System.Configuration.ConfigurationManager.AppSettings["Servidor"].ToString();
        private static int puerto = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Puerto"]);
        private static string clave = System.Configuration.ConfigurationManager.AppSettings["Clave"].ToString();

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public WebApi()
        {
            getToken();
        }
        public void getToken()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var url = "";

            var client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.POST);
            var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(string.Format("{0}:{1}", clientID, clientSecret)));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", "Basic " + basicAuth);
            var response = client.Execute(request);
            var responseJson = JObject.Parse(response.Content.ToString());
            token = responseJson["access_token"].ToString();
        }
        public ConversationDetail darConversacionesUltimaSemana(int pageNumber)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Paging paging = new Paging();
            paging.pageSize = "100";
            paging.pageNumber = pageNumber;

            string fecha1 = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            string fecha2 = DateTime.Now.ToString("yyyy-MM-dd");

            string h1 = "", h2 = "";
            if (DateTime.UtcNow.Hour < 10)
                h1 = "0" + DateTime.UtcNow.Hour;
            else
                h1 = "" + DateTime.UtcNow.Hour;

            if (DateTime.UtcNow.AddHours(1).Hour < 10)
                h2 = "0" + DateTime.UtcNow.AddHours(1).Hour;
            else
                h2 = "" + DateTime.UtcNow.AddHours(1).Hour;

            h2 = h1;
            List<Predicate> predicates = new List<Predicate>();

            Predicate predicate = new Predicate();
            predicate.type = "dimension";
            predicate.dimension = "mediaType";
            predicate.@operator = "matches";
            predicate.value = "email";
            predicates.Add(predicate);

            predicate = new Predicate();
            predicate.type = "dimension";
            predicate.dimension = "wrapUpCode";
            predicate.@operator = "exists";
            predicate.value = null;
            predicates.Add(predicate);

            SegmentFilter segmentFilter = new SegmentFilter();
            segmentFilter.type = "and";
            segmentFilter.predicates = predicates;

            List<SegmentFilter> segmentFilters = new List<SegmentFilter>();
            segmentFilters.Add(segmentFilter);

            BodyConversationDetail body = new BodyConversationDetail();
            body.interval = fecha1 + "T" + h1 + ":00:00.000Z/" + fecha2 + "T" + h2 + ":00:00.000Z";
            body.order = "asc";
            body.orderBy = "conversationStart";
            body.paging = paging;
            body.segmentFilters = segmentFilters;

            var json = JsonConvert.SerializeObject(body);
            var url = "";
            var client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "bearer " + token);
            request.AddJsonBody(json);
            var response = client.Execute(request);
            while (response.StatusCode.ToString().Equals("429"))
            {
                int value = 1000;
                for (int z = 0; z < response.Headers.Count; z++)
                {

                    if (response.Headers[z].Name.Equals("Retry-After"))
                    {
                        int after = Int32.Parse(response.Headers[z].Value.ToString());

                        value = value * after;
                    }
                }
                Thread.Sleep(value);
                response = client.Execute(request);
            }
            var responseJson = JsonConvert.DeserializeObject<ConversationDetail>(response.Content.ToString());
            return responseJson;
        }
        public ConversationDetail darConversacionesUltimaHora(int pageNumber)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Paging paging = new Paging();
            paging.pageSize = "100";
            paging.pageNumber = pageNumber;

            string fecha1 = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            string fecha2 = DateTime.Now.ToString("yyyy-MM-dd");

            string h1 = "", h2 = "";
            if (DateTime.UtcNow.Hour < 10)
                h1 = "0" + DateTime.UtcNow.Hour;
            else
                h1 = "" + DateTime.UtcNow.Hour;

            if (DateTime.UtcNow.AddHours(1).Hour < 10)
                h2 = "0" + DateTime.UtcNow.AddHours(1).Hour;
            else
                h2 = "" + DateTime.UtcNow.AddHours(1).Hour;

            h2 = h1;
            List<Predicate> predicates = new List<Predicate>();

            Predicate predicate = new Predicate();
            predicate.type = "dimension";
            predicate.dimension = "mediaType";
            predicate.@operator = "matches";
            predicate.value = "email";
            predicates.Add(predicate);

            predicate = new Predicate();
            predicate.type = "dimension";
            predicate.dimension = "wrapUpCode";
            predicate.@operator = "exists";
            predicate.value = null;
            predicates.Add(predicate);

            SegmentFilter segmentFilter = new SegmentFilter();
            segmentFilter.type = "and";
            segmentFilter.predicates = predicates;

            List<SegmentFilter> segmentFilters = new List<SegmentFilter>();
            segmentFilters.Add(segmentFilter);

            BodyConversationDetail body = new BodyConversationDetail();
            body.interval = fecha1 + "T" + h1 + ":00:00.000Z/" + fecha2 + "T" + h2 + ":00:00.000Z";
            body.order = "asc";
            body.orderBy = "conversationStart";
            body.paging = paging;
            body.segmentFilters = segmentFilters;

            var json = JsonConvert.SerializeObject(body);
            var url = "";
            var client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "bearer " + token);
            request.AddJsonBody(json);
            var response = client.Execute(request);
            while (response.StatusCode.ToString().Equals("429"))
            {
                int value = 1000;
                for (int z = 0; z < response.Headers.Count; z++)
                {

                    if (response.Headers[z].Name.Equals("Retry-After"))
                    {
                        int after = Int32.Parse(response.Headers[z].Value.ToString());

                        value = value * after;
                    }
                }
                Thread.Sleep(value);
                response = client.Execute(request);
            }
            var responseJson = JsonConvert.DeserializeObject<ConversationDetail>(response.Content.ToString());
            return responseJson;
        }
        public bool validarDominioCorreo(string correo)
        {
            bool respuesta = true;
            //Lista de dominios restringidos
            List<string> listaDominios = new List<string>();
            listaDominios.Add("");

            //Obtenemos el dominio del correo enviado
            try
            {
                string[] arrCorreo = correo.Split('@');
                string resBusqueda = listaDominios.Find(x => x == arrCorreo[1]);
                if (resBusqueda != null && resBusqueda != "")
                {
                    respuesta = false;
                }
                else
                {
                    respuesta = true;
                }
            }
            catch (Exception e)
            {

            }
            return respuesta;
        }
        public string sendMessageRespuesta(Interaction i)
        {
            string result = "";
            try
            {
                if (i.addressFrom != null && i.addressFrom != "")
                {
                    string uri = string.Format("{0}?cliente=MAF&canal=Correo&connId={1}", urlEPA, i.conversationId.ToString());

                    MailMessage correo = new MailMessage();
                    correo.From = new MailAddress("", "MAF PERÚ", System.Text.Encoding.UTF8); //Correo de salida
                    correo.Subject = "Cuéntanos tu experiencia con MAF Perú";//toAsunto
                    correo.To.Add(i.addressFrom); //Correo destino

                    correo.Body = "<!DOCTYPE html>";
                    correo.Body = correo.Body + "<html>";
                    correo.Body = correo.Body + "<head>";
                    correo.Body = correo.Body + "<meta http - equiv = 'Content-Type' content = 'text/html; charset=utf-8'/>";
                    correo.Body = correo.Body + "<title></title>";
                    correo.Body = correo.Body + "</head>";
                    correo.Body = correo.Body + "<body>";
                    correo.Body = correo.Body + "<div>";
                    correo.Body = correo.Body + "<a href = '"+ uri + "' title = 'Ir a Encuesta' target = '_blank'>";
                    correo.Body = correo.Body + "<img alt = 'Encuesta' src = ''/>";
                    correo.Body = correo.Body + "</a>";
                    correo.Body = correo.Body + "</div>";
                    correo.Body = correo.Body + "</body>";
                    correo.Body = correo.Body + "</html>";

                    correo.IsBodyHtml = true;
                    correo.Priority = MailPriority.Normal;

                    SmtpClient smtp = new SmtpClient();
                    smtp.UseDefaultCredentials = true;
                    smtp.Host = ""; //Host del servidor de correo 
                    smtp.Port = 0; //Puerto de salida
                    smtp.Credentials = new System.Net.NetworkCredential("", ""); //Cuenta de correo            

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    smtp.EnableSsl = true; //True si el servidor de correo permite ssl

                    try
                    {
                        using (var smtpClient = new SmtpClient())
                        {
                            //Envio del correo
                            smtp.Send(correo);
                        }
                    }
                    catch (Exception ex)
                    {
                        result = (ex.Message.ToString() + " " + ex.InnerException.Message.ToString());
                    }

                    correo.Dispose();

                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                result = (e.Message.ToString() + " " + e.InnerException.Message.ToString());
            }

            return result;
        }
        public string enviarContacto(string asunto, string mensaje)
        {
            string respuesta = string.Empty;
            if (mensaje != null)
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress(emailRemitente, "MAF PERÚ", System.Text.Encoding.UTF8); //Correo de salida
                correo.Subject = asunto; //Asunto
                correo.To.Add(emailContacto); //Correo destino 

                correo.Body = mensaje;
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = true;
                smtp.Host = servidor; //Host del servidor de correo 
                smtp.Port = puerto; //Puerto de salida
                smtp.Credentials = new System.Net.NetworkCredential(emailRemitente, clave); //Cuenta de correo            

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = true; //True si el servidor de correo permite ssl

                try
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        //Envio del correo
                        smtp.Send(correo);
                        respuesta = "OK";
                    }
                }
                catch (Exception ex)
                {
                    respuesta = (ex.Message.ToString());

                    logger.Info("Error en enviarContacto: " + respuesta);
                }

                correo.Dispose();

                Thread.Sleep(1000);
            }

            return respuesta;
        }
        public string enviarReclamo(string asunto, string mensaje)
        {
            string respuesta = string.Empty;
            if (mensaje != null)
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress(emailRemitente, "MAF PERÚ", System.Text.Encoding.UTF8); //Correo origen
                correo.Subject = asunto; //Asunto
                correo.To.Add(emailContacto); //Correo destino 
                correo.To.Add(emailReclamaciones); //Correo destino 

                correo.Body = mensaje;
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false; //true
                smtp.Host = servidor; //Host del servidor de correo 
                smtp.Port = puerto; //Puerto de salida
                smtp.Credentials = new System.Net.NetworkCredential(emailRemitente, clave); //Cuenta de correo            

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = true; //True si el servidor de correo permite ssl

                try
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        //Envio del correo
                        smtp.Send(correo);
                        respuesta = "OK";
                    }
                }
                catch (Exception ex)
                {
                    respuesta = (ex.Message.ToString());

                    logger.Info("Error en enviarReclamo: " + respuesta);
                }

                correo.Dispose();

                Thread.Sleep(1000);
            }

            return respuesta;
        }
        public string enviarVehiculo(string asunto, string mensaje)
        {
            string respuesta = string.Empty;
            if (mensaje != null)
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress(emailRemitente, "MAF PERÚ", System.Text.Encoding.UTF8); //Correo origen
                correo.Subject = asunto; //Asunto
                correo.To.Add(emailVehiculo);
                correo.Body = mensaje;
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false; //true
                smtp.Host = servidor; //Host del servidor de correo 
                smtp.Port = puerto; //Puerto de salida
                smtp.Credentials = new System.Net.NetworkCredential(emailRemitente, clave); //Cuenta de correo            

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = true; //True si el servidor de correo permite ssl

                try
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        //Envio del correo
                        smtp.Send(correo);
                        respuesta = "OK";
                    }
                }
                catch (Exception ex)
                {
                    respuesta = (ex.Message.ToString());

                    logger.Info("Error en enviarVehiculo: " + respuesta);
                }

                correo.Dispose();

                Thread.Sleep(1000);
            }

            return respuesta;
        }
        public string notificar(string tipo, string mensaje, int indicador)
        {
            string respuesta = string.Empty;
            string asunto = string.Concat("NOTIFICACIÓN DE ERROR DE REGISTRO - ", tipo);

            string[] correos = new string[2];
            switch (indicador)
            {
                case 1: //Reclamo
                    correos = emailNotificacionReclamo.Split(';');
                    break;
                case 2: //Contacto
                    correos = emailNotificacionContacto.Split(';');
                    break;
            }

            if (mensaje != null)
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress(emailRemitente, "MAF PERÚ", System.Text.Encoding.UTF8); //Correo origen
                correo.Subject = asunto; //Asunto
                correo.To.Add(correos[0].ToString()); //Correo destino 
                correo.To.Add(correos[1].ToString()); //Correo destino 

                correo.Body = mensaje;
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.High;

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false; //true
                smtp.Host = servidor; //Host del servidor de correo 
                smtp.Port = puerto; //Puerto de salida
                smtp.Credentials = new System.Net.NetworkCredential(emailRemitente, clave); //Cuenta de correo            

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = true; //True si el servidor de correo permite ssl

                try
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        //Envio del correo
                        smtp.Send(correo);
                        respuesta = "OK";
                    }
                }
                catch (Exception ex)
                {
                    respuesta = (ex.Message.ToString());
                }

                correo.Dispose();

                Thread.Sleep(1000);
            }

            return respuesta;
        }
    }
}