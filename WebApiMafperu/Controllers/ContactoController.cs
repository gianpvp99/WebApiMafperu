using NLog;
using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using WebApiMafperu.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Antlr.Runtime;
using System.Linq;
using WebGrease.Activities;
using System.Web.Http.Cors;

namespace WebApiMafperu.Controllers
{
    public class ContactoController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        // GET: listarTipoDocumwnto
        //[EnableCors(origins: "*", headers: "*", methods: "DELETE")]
        [HttpGet]
        [Route("api/contacto/listartipodocumento")]
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

        [HttpGet]
        [Route("api/contacto/clienteDocumento")]
        public DatosCliente listarCliente(string nroDocumento)
        {
            try
            {
                WebApiCrm ws = new WebApiCrm();
                DatosCliente lista = new DatosCliente();
                lista = ws.listarCliente(nroDocumento);
                return lista;

            }catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("api/contacto/clienteDocumento2")]
        public DatosCliente listarCliente2(string nroDocumento)
        {
            try
            {
                WebApiCrm ws = new WebApiCrm();
                DatosCliente lista = new DatosCliente();
                lista = ws.listarCliente2(nroDocumento);
                if (lista.error)
                {
                    // Lanzar una excepción con el código de estado HTTP 500
                    return new DatosCliente { error = lista.error };
                }
                return lista;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: listarMotivo
        [HttpGet]
        [Route("api/contacto/listarmotivo")]
        public List<DatosMotivo> listarMotivo(string tipo)
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosMotivo> lista = new List<DatosMotivo>();

            try
            {
                var data = ws.listarMotivo(tipo);

                if (tipo == "S")
                {
                    lista = data.FindAll(x => x.Codigo != "8c332a94-90d6-ea11-a813-000d3a378e0f");
                }
                else 
                {
                    lista = data;
                }                
            }
            catch (Exception ex)
            {
                lista = null;
                logger.Info("Error listarMotivo: " + ex.Message.ToString());
            }

            return lista;
        }

        //GET: listarSubMotivo
        [HttpGet]
        [Route("api/contacto/listarsubmotivo")]
        public List<DatosSubMotivo> listarSubMotivo(string tipo, string motivo)
        {
            WebApiCrm ws = new WebApiCrm();
            List<DatosSubMotivo> lista = new List<DatosSubMotivo>();

            try
            {
                lista = ws.listarSubMotivo(tipo, motivo);
            }
            catch (Exception ex)
            {
                lista = null;
                logger.Info("Error listarSubMotivo: " + ex.Message.ToString());
            }

            return lista;
        }

        [HttpPost]
        [Route("api/contacto/adjunto")]
        public async Task<IHttpActionResult> adjunto()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Ok(new RespuestaCrm() { indicadorExito = 1, descripcionError = "Sin archivos"});
                    //return BadRequest("No se adjuntaron archivos");

                }
                // Configurar el directorio donde se guardarán los archivos adjuntos
                var uploadPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads"), "adjuntos");
                Directory.CreateDirectory(uploadPath);


                // Configurar el proveedor de stream para leer los archivos adjuntos
                var provider = new MultipartFormDataStreamProvider(uploadPath);
                // Leer los archivos adjuntos de la solicitud y guardarlos en el directorio configurado
                await Request.Content.ReadAsMultipartAsync(provider);
                var files = new List<string>();
                WebApiCrm crm = new WebApiCrm();

                RespuestaCrm response = new RespuestaCrm();

                foreach (var file in provider.FileData)
                {
                    var fileInfo = new FileInfo(file.LocalFileName);
                    var fileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var fileExtension = Path.GetExtension(fileName);
                    var fileBytes = File.ReadAllBytes(file.LocalFileName);
                    var filePath = fileInfo.FullName;
                    var fileBase64 = Convert.ToBase64String(fileBytes);
                    File.Move(file.LocalFileName, Path.Combine(uploadPath, fileName + "_" + fileInfo.Name + fileExtension  ));

                    // Crear objeto para enviar al servicio externo
                    var adjuntoData = new DatosAdjuntoCrm()
                    {
                        gEntidad = provider.FormData.GetValues("caseId").FirstOrDefault(), //"9aa235a2-6511-ef11-9f8a-000d3a597a3c",
                        Asunto = provider.FormData.GetValues("subject").FirstOrDefault(),
                        NombreArchivo = fileName,
                        Data = fileBase64
                    };

                    // Aquí puedes realizar operaciones con el archivo, como guardarlo en el servidor o procesarlo de alguna manera

                    files.Add(fileName);
                    response = crm.adjuntarCrm(adjuntoData);
                }

                //return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Archivos subidos correctamente", Files = files });

                return Ok(new RespuestaCrm() { indicadorExito = response.indicadorExito, descripcionError= response.descripcionError }) ;

            }catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/contacto/registroForm")]
        public async Task<IHttpActionResult> registroForm(DatosRegistroCrm datosRegistro)
        {
            try
            {
                WebApiCrm crm = new WebApiCrm();
                var response = crm.registrarCrm(datosRegistro);
                return Ok(new RespuestaCrm { indicadorExito = response.indicadorExito, descripcionError = response.descripcionError });

            }
            catch (Exception ex)

            {
                throw ex;
            }
        }

        //[EnableCors(origins: "*", headers: "*", methods: "POST")]
        [HttpPost]
        [Route("api/contacto/prueba")]
        public IHttpActionResult prueba([FromBody] Prueba prueba)
        {
            try
            {

                return Ok("Correcto");

            }
            catch (Exception ex)

            {
                throw ex;
            }
        }




        //POST: EnviarConsulta
        [HttpPost]
        public RespuestaWS EnviarConsulta(string mensaje, [FromBody] DatosRegistroForm registro)
        {
            WebApi ws = new WebApi();
            WebApiCrm wsReclamo = new WebApiCrm();
            LogicaNegocio negocio = new LogicaNegocio();

            RespuestaWS respuestaWS = null;
            RespuestaCrm respuestaCrm = null;
            mensaje = mensaje.Replace("|999", "&");

            logger.Info("Inicio EnviarContacto");
            logger.Info("Datos de entrada: " + JsonConvert.SerializeObject(registro));
            try
            {
                if (registro != null)
                {
                    string id = negocio.obtenerId(registro.TipoCaso);
                    logger.Info("ID generado: " + id);

                    var asunto = "Contáctanos MAF PERÚ";

                    //Enviar correo a GC
                    mensaje = mensaje.Replace("|999", "&");
                    var resultado = ws.enviarContacto(asunto, mensaje);
                    logger.Info("enviarContacto => " + resultado);

                    //Registrar información en CRM                    
                    respuestaCrm = new RespuestaCrm();
                    respuestaCrm = wsReclamo.registrarForm(registro);
                    logger.Info("Datos de Salida registrarForm => " + JsonConvert.SerializeObject(respuestaCrm));

                    if (respuestaCrm != null)
                    {
                        respuestaWS = new RespuestaWS();
                        respuestaWS.indicadorExito = respuestaCrm.indicadorExito;
                        respuestaWS.guid = respuestaCrm.descripcionError;
                        respuestaWS.asunto = asunto;
                        respuestaWS.idreclamo = "no aplica";
                    }

                    logger.Info("Datos de salida: " + JsonConvert.SerializeObject(respuestaWS));
                }
            }
            catch (Exception ex)
            {
                logger.Info("Error en Proceso: " + ex.Message.ToString());
                logger.Info("Fin EnviarContacto");
            }
            logger.Info("Fin EnviarContacto");

            return respuestaWS;
        }

        //POST: EnviarArchivo
        [HttpPost]
        public string EnviarArchivo(int indicadorExito, string guid, string asunto)
        {
            WebApi ws = new WebApi();
            WebApiCrm wsReclamo = new WebApiCrm();

            string respuesta = string.Empty;
            string rutacliente = string.Empty;
            string directorio = System.Configuration.ConfigurationManager.AppSettings["Uploads"].ToString();

            logger.Info("Inicio EnviarArchivo");
            try
            {
                if (indicadorExito != 0 && guid != null && asunto != "")
                {
                    //Recuperando archivos
                    var request = HttpContext.Current.Request;
                    if (request != null && request.Files.Count > 0)
                    {
                        //Creando carpeta
                        rutacliente = string.Format("{0}\\{1}", directorio, guid);
                        if (!Directory.Exists(rutacliente))
                        {
                            Directory.CreateDirectory(rutacliente);
                        }

                        foreach (string file in request.Files)
                        {
                            var postedFile = request.Files[file];
                            var filePath = HttpContext.Current.Server.MapPath(string.Format("~/Uploads/{0}/{1}", guid, postedFile.FileName));
                            postedFile.SaveAs(filePath);
                        }

                        //Enviar archivos adjuntos a CRM
                        logger.Info("Enviando archivos adjuntos: " + guid);
                        Helper.enviar_adjunto(asunto, guid, guid);

                        //Eliminando temporal...
                        var temporal = Path.Combine(directorio, guid);
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
                var temporal = Path.Combine(directorio, guid);
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

        //POST: EnviarConsulta2
        [HttpPost]
        public RespuestaWS EnviarConsulta2([FromBody] DatosRegistroForm2 registro)
        {
            WebApi ws = new WebApi();
            WebApiCrm wsReclamo = new WebApiCrm();
            LogicaNegocio negocio = new LogicaNegocio();

            RespuestaWS respuestaWS = null;
            RespuestaCrm respuestaCrm = null;

            string mensaje = Helper.generar_mensaje2(registro);

            logger.Info("Inicio EnviarContacto");
            logger.Info("Datos de entrada: " + JsonConvert.SerializeObject(registro));
            try
            {
                if (registro != null)
                {
                    string id = negocio.obtenerId(registro.TipoCaso);
                    logger.Info("ID generado: " + id);

                    var asunto = "Contáctanos MAF PERÚ";

                    //Enviar correo a GC
                    var resultado = ws.enviarContacto(asunto, mensaje);
                    logger.Info("enviarContacto => " + resultado);

                    //Registrar información en CRM
                    DatosRegistroForm obj = new DatosRegistroForm();
                    obj.RazonSocial = registro.RazonSocial;
                    obj.TipoDoc = registro.TipoDoc;
                    obj.NroDoc = registro.NroDoc;
                    obj.Nombres = registro.Nombres;
                    obj.ApellidoPaterno = registro.ApellidoPaterno;
                    obj.ApellidoMaterno = registro.ApellidoMaterno;
                    obj.Email = registro.Email;
                    obj.Celular = registro.Celular;
                    obj.Motivo = registro.Motivo;
                    obj.SubMotivo = registro.SubMotivo;
                    obj.DetalleCaso = registro.DetalleCaso;
                    obj.TipoCaso = registro.TipoCaso;
                    obj.TipoProductoLib = registro.TipoProductoLib;
                    obj.EnvioNotificacion = registro.EnvioNotificacion;

                    respuestaCrm = new RespuestaCrm();
                    respuestaCrm = wsReclamo.registrarForm(obj);
                    logger.Info("Datos de Salida registrarForm => " + JsonConvert.SerializeObject(respuestaCrm));

                    if (respuestaCrm != null)
                    {
                        respuestaWS = new RespuestaWS();
                        respuestaWS.indicadorExito = respuestaCrm.indicadorExito;
                        respuestaWS.guid = respuestaCrm.descripcionError;
                        respuestaWS.asunto = asunto;
                        respuestaWS.idreclamo = "no aplica";
                    }

                    logger.Info("Datos de salida: " + JsonConvert.SerializeObject(respuestaWS));
                }
            }
            catch (Exception ex)
            {
                logger.Info("Error en Proceso: " + ex.Message.ToString());
                logger.Info("Fin EnviarContacto");
            }
            logger.Info("Fin EnviarContacto");

            return respuestaWS;
        }
    }
}

public class Prueba{
    public string prueba { get; set; }
    public int id { get; set; }
}