using NLog;
using System;
using System.Web;
using System.IO;
using System.Web.Http;
using WebApiMafperu.Models;

namespace WebApiMafperu.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ChatController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // POST: Enviar archivos adjuntos
        [HttpPost]
        public string EnviarArchivos(string conversationId, string numdocumento)
        {
            string rutacliente = string.Empty;
            string respuesta = string.Empty;
            string directorio = System.Configuration.ConfigurationManager.AppSettings["Uploads"].ToString();            

            try
            {
                if (conversationId != null && conversationId != "" && numdocumento != null && numdocumento != "")
                {
                    var nomarchivo = string.Concat(conversationId, "-", numdocumento);
                    logger.Info("File: " + nomarchivo);

                    //Recuperando archivos
                    var request = HttpContext.Current.Request;
                    if (request != null && request.Files.Count > 0)
                    {
                        //Creando carpeta
                        rutacliente = string.Format("{0}\\{1}", directorio, nomarchivo);
                        if (!Directory.Exists(rutacliente))
                        {
                            Directory.CreateDirectory(rutacliente);
                        }

                        foreach (string file in request.Files)
                        {
                            var postedFile = request.Files[file];
                            var filePath = HttpContext.Current.Server.MapPath(string.Format("~/Uploads/{0}/{1}", nomarchivo, postedFile.FileName));

                            postedFile.SaveAs(filePath);
                        }

                        //Enviando archivos
                        logger.Info("Enviando archivos chat SFTP: " + nomarchivo);
                        Helper.enviar_sftp(nomarchivo);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = ex.Message.ToString();
                logger.Info("Error en Proceso: " + respuesta);
            }

            return respuesta;
        }
    }
}