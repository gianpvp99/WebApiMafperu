using NLog;
using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using WebApiMafperu.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiMafperu.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VehiculoController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //POST: obtenerTipoDocumento
        [HttpPost]
        public string obtenerTipoDocumento(string id)
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosTipoDocumento> lista = new List<DatosTipoDocumento>();

            string tipoDocumento = string.Empty;

            try
            {
                lista = ws.listarTipoDocumento();

                var resultado = lista.Find(x => x.Codigo == id);
                tipoDocumento = resultado.Nombre.ToString();
            }
            catch (Exception ex)
            {
                lista = null;
                logger.Info("Error obtenerTipoDocumento: " + ex.Message.ToString());
            }

            return tipoDocumento;
        }

        //POST: EnviarVehiculo
        [HttpPost]
        [Route("api/vehiculo/enviarvehiculo")]
        public RespuestaWS EnviarVehiculo(string mensaje, [FromBody] DatosRegistroForm registro)
        {
            WebApi ws = new WebApi();
            LogicaNegocio negocio = new LogicaNegocio();
            WebApiCrm wcrm = new WebApiCrm();

            RespuestaWS respuestaWS = null;
            RespuestaCrm respuestaCrm = null;
            mensaje = mensaje.Replace("|999", "&");

            logger.Info("Inicio EnviarVehiculo");
            logger.Info("Datos de entrada: " + JsonConvert.SerializeObject(registro));
            try
            {
                if (registro != null)
                {
                    string id = negocio.obtenerId(registro.TipoCaso);
                    logger.Info("ID generado: " + id);

                    var asunto = "Detalle de Solicitud";
                    var titulo = string.Format("{0} {1}", "Detalle de Solicitud", id);

                    //Enviar correo vehiculo
                    mensaje = mensaje.Replace("[TITULO]", titulo);
                    var resultado = ws.enviarVehiculo(asunto, mensaje);
                    logger.Info("enviarVehiculo =>" + resultado);

                    //Registrar información en CRM
                    respuestaCrm = new RespuestaCrm();
                    respuestaCrm = wcrm.registrarForm(registro);
                    logger.Info("Datos de Salida registrarCrm =>" + JsonConvert.SerializeObject(respuestaCrm));

                    if (respuestaCrm != null)
                    {
                        respuestaWS = new RespuestaWS();
                        respuestaWS.idreclamo = id;
                        respuestaWS.indicadorExito = respuestaCrm.indicadorExito;
                        respuestaWS.guid = respuestaCrm.descripcionError;
                    }

                    logger.Info("Datos de salida: " + JsonConvert.SerializeObject(respuestaWS));
                }
            }
            catch (Exception ex)
            {
                logger.Info("Error en Proceso: " + ex.Message.ToString());
                logger.Info("Fin EnviarVehiculo");
            }
            logger.Info("Fin EnviarVehiculo");

            return respuestaWS;
        }

        //POST: EnviarArchivo
        [HttpPost]
        [Route("api/vehiculo/enviararchivo")]
        public async Task<IHttpActionResult> EnviarArchivoAsync(int indicadorExito, string guid, string idsolicitud, string asunto)
        {
            //WebApi ws = new WebApi();
            List<RespuestaCrm> response = new List<RespuestaCrm>();
            string respuesta = string.Empty;
            string rutacliente = string.Empty;
            string directorio = System.Configuration.ConfigurationManager.AppSettings["Uploads"].ToString();
            var uploadPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath($"~/{directorio}"), $"Adjuntos_SeguroVehicular\\{idsolicitud}");

            logger.Info("Inicio EnviarArchivo");
            try
            {
                if (indicadorExito != 0 && guid != null && idsolicitud != "" && asunto != "")
                {
                    //Recuperando archivos
                    var request = HttpContext.Current.Request;
                    if (request != null && request.Files.Count > 0)
                    {
                        //Creando carpeta
                        //rutacliente = string.Format("\\{0}\\{1}", directorio + "", idsolicitud);
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        foreach (string file in request.Files)
                        {
                            var postedFile = request.Files[file];
                            var filePath = HttpContext.Current.Server.MapPath($"~/{directorio}/Adjuntos_SeguroVehicular/{idsolicitud}/{postedFile.FileName}");
                            postedFile.SaveAs(filePath);
                        }

                        //Enviar archivos adjuntos a CRM
                        logger.Info("Enviando archivos adjuntos: " + idsolicitud);
                        response = await Helper.enviar_adjunto(asunto, guid, idsolicitud);

                        //Eliminando temporal...
                        var temporal = Path.Combine(directorio, idsolicitud);
                        if (Directory.Exists(uploadPath))
                        {
                            Directory.Delete(uploadPath, true);
                            logger.Info("Eliminando temporal ...");
                        }
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                respuesta = ex.Message.ToString();

                //Eliminando temporal...
                //var temporal = Path.Combine(directorio, idsolicitud);
                if (Directory.Exists(uploadPath))
                {
                    Directory.Delete(uploadPath, true);
                    logger.Info("Eliminando temporal ...");
                }

                logger.Info("Error en Proceso: " + respuesta);
                logger.Info("Fin EnviarArchivo");
                return InternalServerError(ex);
            }

        }
    }
}