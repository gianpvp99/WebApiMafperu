using NLog;
using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using WebApiMafperu.Models;
using System.Collections.Generic;

namespace WebApiMafperu.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ReclamoController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: listarTipoDocumwnto
        [HttpGet]
        [Route("api/reclamo/listartipodocumento")]
        public List<DatosTipoDocumento> listarTipoDocumento() 
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosTipoDocumento> lista = new List<DatosTipoDocumento>();

            try
            {
                lista = ws.listarTipoDocumento();
            }
            catch (Exception ex) 
            {
                lista = null;
                logger.Info("Error listarTipoDocumento: " + ex.Message.ToString());
            }

            return lista;
        }

        // GET: listarDepartamento
        [HttpGet]
        [Route("api/reclamo/listardepartamento")]
        public List<DatosDepartamento> listarDepartamento()
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosDepartamento> lista = new List<DatosDepartamento>();

            try
            {
                lista = ws.listarDepartamento();
            }
            catch (Exception ex)
            {
                lista = null;
                logger.Info("Error listarDepartamento: " + ex.Message.ToString());
            }

            return lista; 
        }

        // GET: listarProvincia
        [HttpGet]
        [Route("api/reclamo/listarprovincia")]
        public List<DatosProvincia> listarProvincia(string departamento)
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosProvincia> lista = new List<DatosProvincia>();

            try
            {
                lista = ws.listarProvincia(departamento);
            }
            catch (Exception ex)
            {
                lista = null;
                logger.Info("Error listarProvincia: " + ex.Message.ToString());
            }

            return lista;
        }

        // GET: listarDistrito
        [HttpGet]
        [Route("api/reclamo/listardistrito")]
        public List<DatosDistrito> listarDistrito(string provincia)
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosDistrito> lista = new List<DatosDistrito>();

            try
            {
                lista = ws.listarDistrito(provincia);
            }
            catch (Exception ex)
            {
                lista = null;
                logger.Info("Error listarDistrito: " + ex.Message.ToString());
            }

            return lista;
        }

        // GET: listarMotivo
        [HttpGet]
        [Route("api/reclamo/listarmotivo")]
        public List<DatosMotivo> listarMotivo()
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosMotivo> lista = new List<DatosMotivo>();

            try
            {
                lista = ws.listarMotivo("R");
            }
            catch (Exception ex)
            {
                logger.Info("Error listarMotivo: " + ex.Message.ToString());
            }

            return lista;
        }

        //GET: listarSubMotivo
        [HttpGet]
        [Route("api/reclamo/listarsubmotivo")]
        public List<DatosSubMotivo> listarSubMotivo(string id)
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosSubMotivo> lista = new List<DatosSubMotivo>();

            try
            {
                lista = ws.listarSubMotivo("R", id);
            }
            catch (Exception ex)
            {
                logger.Info("Error listarSubMotivo: " + ex.Message.ToString());
            }

            return lista;
        }

        //POST: EnviarReclamo
        [HttpPost]
        public RespuestaWS EnviarReclamo(string mensaje, [FromBody]DatosRegistroCrm registro) 
        {            
            WebApi ws = new WebApi();
            LogicaNegocio negocio = new LogicaNegocio();
            WebApiCrm wsReclamo = new WebApiCrm();

            RespuestaWS respuestaWS = null;
            RespuestaCrm respuestaCrm = null;
            mensaje = mensaje.Replace("|999", "&");      

            logger.Info("Inicio EnviarReclamo");
            logger.Info("Datos de entrada: " + JsonConvert.SerializeObject(registro));
            try
            {
                if (registro != null)
                {
                    string id = negocio.obtenerId(registro.TipoCaso);
                    logger.Info("ID generado: " + id);

                    var asunto = string.Format("{0} {1} / {2} / {3}", registro.Nombres, registro.ApellidoPaterno, registro.NroDoc, id);
                    var titulo = string.Format("{0} {1}","Detalle de Reclamo",id);

                    //Enviar correo de reclamo o queja
                    mensaje = mensaje.Replace("[TITULO]", titulo);
                    var resultado = ws.enviarReclamo(asunto, mensaje);
                    logger.Info("enviarReclamo => " + resultado);

                    //Registrar información en CRM                    
                    respuestaCrm = new RespuestaCrm();
                    respuestaCrm = wsReclamo.registrarCrm(registro);
                    logger.Info("Datos de Salida registrarCrm => " + JsonConvert.SerializeObject(respuestaCrm));

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
                logger.Info("Fin EnviarReclamo");
            }
            logger.Info("Fin EnviarReclamo");

            return respuestaWS;
        }

        //POST: EnviarArchivo
        [HttpPost]
        public string EnviarArchivo(int indicadorExito, string guid, string idreclamo, string asunto) 
        {
            WebApi ws = new WebApi();
            WebApiCrm wsReclamo = new WebApiCrm();

            string respuesta = string.Empty;
            string rutacliente = string.Empty;            
            string directorio = System.Configuration.ConfigurationManager.AppSettings["Uploads"].ToString();

            logger.Info("Inicio EnviarArchivo");
            try
            {
                if (indicadorExito != 0 && guid != null && idreclamo != "" && asunto != "")
                {                    
                    //Recuperando archivos
                    var request = HttpContext.Current.Request;
                    if (request != null && request.Files.Count > 0)
                    {
                        //Creando carpeta
                        rutacliente = string.Format("{0}\\{1}", directorio, idreclamo);
                        if (!Directory.Exists(rutacliente))
                        {
                            Directory.CreateDirectory(rutacliente);
                        }

                        foreach (string file in request.Files)
                        {
                            var postedFile = request.Files[file];
                            var filePath = HttpContext.Current.Server.MapPath(string.Format("~/Uploads/{0}/{1}", idreclamo, postedFile.FileName));
                            postedFile.SaveAs(filePath);
                        }

                        //Enviar archivos adjuntos a CRM
                        logger.Info("Enviando archivos adjuntos: " + idreclamo);
                        Helper.enviar_adjunto(asunto, guid, idreclamo);

                        //Enviar archivos a SFTP
                        //logger.Info("Enviando archivos SFTP: " + idreclamo);
                        //Helper.enviar_sftp(idreclamo);

                        //Eliminando temporal...
                        var temporal = Path.Combine(directorio, idreclamo);
                        if (Directory.Exists(temporal))
                        {
                            Directory.Delete(temporal, true);
                            logger.Info("Eliminando temporal ...");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = ex.Message.ToString();

                //Eliminando temporal...
                var temporal = Path.Combine(directorio, idreclamo);
                if (Directory.Exists(temporal))
                {
                    Directory.Delete(temporal, true);
                    logger.Info("Eliminando temporal ...");
                }

                logger.Info("Error en Proceso: " + respuesta);                
            }
            logger.Info("Fin EnviarArchivo");

            return respuesta;
        }

        //GET: ObtenerFechaAtencion
        [HttpGet]
        [Route("api/reclamo/fechaatencion")]
        public string ObtenerFechaAtencion() 
        {
            LogicaNegocio negocio = new LogicaNegocio();

            string fechaAtencion = string.Empty;
            string fechaRegistro = DateTime.Now.ToString("yyyy-MM-dd");

            DateTime desde = Convert.ToDateTime(fechaRegistro).AddDays(1);
            DateTime hasta = Convert.ToDateTime(fechaRegistro).AddDays(15);
            
            int valor = 0;
            int numerodia = 0;
            int indferiado = 0;

            try
            {
                while (valor < 15)
                {
                    while (desde <= hasta)
                    {
                        numerodia = Convert.ToInt32(desde.DayOfWeek.ToString("d"));

                        if (numerodia == 1 || numerodia == 2 || numerodia == 3 || numerodia == 4 || numerodia == 5)
                        {
                            indferiado = negocio.validarFeriado(desde.ToString("yyyy-MM-dd"));

                            if (indferiado == 0)
                            {
                                fechaAtencion = desde.ToString("yyyy-MM-dd");

                                valor = valor + 1;
                            }
                            else 
                            {
                                hasta = hasta.AddDays(1);
                            }
                        }
                        else 
                        {
                            hasta = hasta.AddDays(1);
                        }

                        desde = desde.AddDays(1);
                    }
                }

                logger.Info("Fecha de atención generada: " + fechaAtencion);
            }
            catch (Exception ex) 
            {
                logger.Info("Hubo error al obtener fecha de atención: " + ex.Message.ToString());
            }

            return fechaAtencion;
        }

        //POST: EnviarReclamo2
        [HttpPost]
        public RespuestaWS EnviarReclamo2([FromBody] DatosRegistroCrm2 registro)
        {
            WebApi ws = new WebApi();
            LogicaNegocio negocio = new LogicaNegocio();
            WebApiCrm wsReclamo = new WebApiCrm();

            RespuestaWS respuestaWS = null;
            RespuestaCrm respuestaCrm = null;

            string mensaje = Helper.generar_mensaje(registro);

            logger.Info("Inicio EnviarReclamo");
            logger.Info("Datos de entrada: " + JsonConvert.SerializeObject(registro));
            try
            {
                if (registro != null)
                {
                    string id = negocio.obtenerId(registro.TipoCaso);
                    logger.Info("ID generado: " + id);

                    var asunto = string.Format("{0} {1} / {2} / {3}", registro.Nombres, registro.ApellidoPaterno, registro.NroDoc, id);
                    var titulo = string.Format("{0} {1}", "Detalle de Reclamo", id);

                    //Enviar correo de reclamo o queja
                    mensaje = mensaje.Replace("[TITULO]", titulo);
                    var resultado = ws.enviarReclamo(asunto, mensaje);
                    logger.Info("enviarReclamo => " + resultado);

                    //Registrar información en CRM
                    DatosRegistroCrm obj = new DatosRegistroCrm();
                    obj.RepresentaEmpresa = registro.RepresentaEmpresa;
                    obj.TipoDoc = registro.TipoDoc;
                    obj.NroDoc = registro.NroDoc;
                    obj.Nombres = registro.Nombres;
                    obj.ApellidoPaterno = registro.ApellidoPaterno;
                    obj.ApellidoMaterno = registro.ApellidoMaterno;
                    obj.Email = registro.Email;
                    obj.Celular = registro.Celular;
                    obj.uDepartamento = registro.uDepartamento;
                    obj.uProvincia = registro.uProvincia;
                    obj.uDistrito = registro.uDistrito;
                    obj.uDireccion = registro.uDireccion;
                    obj.eRazonSocial = registro.eRazonSocial;
                    obj.eTipoDoc = registro.eTipoDoc;
                    obj.eNroDoc = registro.eNroDoc;
                    obj.TipoCaso = registro.TipoCaso;
                    obj.TipoProducto = registro.TipoProducto;
                    obj.Motivo = registro.Motivo;
                    obj.SubMotivo = registro.SubMotivo;
                    obj.DetalleCaso = string.Concat("Detalle: ", registro.DetalleCaso, " Pedido: ", registro.DesPedido);
                    obj.TipoProductoLib = registro.TipoProductoLib;
                    obj.EnvioNotificacion = registro.EnvioNotificacion;

                    respuestaCrm = new RespuestaCrm();
                    respuestaCrm = wsReclamo.registrarCrm(obj);
                    logger.Info("Datos de Salida registrarCrm => " + JsonConvert.SerializeObject(respuestaCrm));

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
                logger.Info("Fin EnviarReclamo");
            }
            logger.Info("Fin EnviarReclamo");

            return respuestaWS;
        }
    }
}