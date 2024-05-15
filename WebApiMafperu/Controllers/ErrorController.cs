using NLog;
using System;
using System.Web.Http;
using WebApiMafperu.Models;

namespace WebApiMafperu.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ErrorController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //POST: Notificar
        [HttpPost]
        public string Notificar(string tipo, string mensaje, int indicador)
        {
            WebApi ws = new WebApi();
            string resultado = string.Empty;

            logger.Info("Inicio Notificar");
            try
            {
                var respuesta = ws.notificar(tipo, mensaje, indicador);
            }
            catch (Exception ex) 
            {
                resultado = ex.Message.ToString();

                logger.Info("Error en Proceso: " + ex.Message.ToString());
                logger.Info("Fin Notificar");
            }
            logger.Info("Fin Notificar");

            return resultado;
        }
    }
}