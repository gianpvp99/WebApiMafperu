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
using Azure.Identity;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Graph.Models.Security;
using System.Web.UI;
using Microsoft.Ajax.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.IdentityModel;
using System.Net.Http.Headers;

namespace WebApiMafperu.Controllers
{
    public class ContactoController : ApiController
    {
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();


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
                    return Ok(new RespuestaCrm() { indicadorExito = 1, descripcionError = "Sin archivos" });
                    //return BadRequest("No se adjuntaron archivos");

                }
                // Configurar el directorio donde se guardarán los archivos adjuntos
                var uploadPath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads"), "Adjuntos_Contacto");
                Directory.CreateDirectory(uploadPath);


                // Configurar el proveedor de stream para leer los archivos adjuntos
                var provider = new MultipartFormDataStreamProvider(uploadPath);
                // Leer los archivos adjuntos de la solicitud y guardarlos en el directorio configurado
                await Request.Content.ReadAsMultipartAsync(provider);
                List<RespuestaCrm> response = new List<RespuestaCrm>();

                var tasks = provider.FileData.Select(async file => // Generar una lista de tareas asíncronas para procesar cada file
                {
                    var fileInfo = new FileInfo(file.LocalFileName);
                    var fileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var fileExtension = Path.GetExtension(fileName);
                    var fileNameSinExtension = Path.GetFileNameWithoutExtension(fileName);
                    var fileBytes = File.ReadAllBytes(file.LocalFileName);
                    var filePath = fileInfo.FullName;
                    var fileBase64 = Convert.ToBase64String(fileBytes);
                    File.Move(file.LocalFileName, Path.Combine(uploadPath, fileNameSinExtension + "_" + fileInfo.Name + fileExtension));

                    // Crear objeto para enviar al servicio externo
                    var adjuntoData = new DatosAdjuntoCrm()
                    {
                        gEntidad = provider.FormData.GetValues("caseId").FirstOrDefault(), //"9aa235a2-6511-ef11-9f8a-000d3a597a3c",
                        Asunto = provider.FormData.GetValues("subject").FirstOrDefault(),
                        NombreArchivo = fileName,
                        Data = fileBase64
                    };

                    // Se envía el adjuntoData al servicio externo
                    WebApiCrm crm = new WebApiCrm();
                    return await crm.adjuntarCrm(adjuntoData);
                });

                var tasksArray = await Task.WhenAll(tasks);

                foreach (var item in tasksArray)
                {
                    response.Add(item);
                };

                //// Obtener el token de acceso para OneDrive
                //var app = ConfidentialClientApplicationBuilder.Create("YOUR_APP_ID")
                //            .WithClientSecret("YOUR_CLIENT_SECRET")
                //            .WithTenantId("YOUR_TENANT_ID")
                //            .Build();

                //string[] scopes = { "https://graph.microsoft.com/.default" };

                //var result = await app.AcquireTokenForClient(scopes)
                //                      .ExecuteAsync();

                //// Inicializar el cliente de Graph
                //var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
                //{
                //    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                //    return Task.FromResult(0);
                //}));

                //// Subir el archivo a OneDrive
                //var stream = File.OpenRead(filePath); // Ruta del archivo que quieres subir
                //var uploadedFile = await graphClient.Me.Drive.Root.ItemWithPath("YOUR_FOLDER_PATH/" + fileName).Content
                //                              .Request()
                //                              .PutAsync<DriveItem>(stream);

                //// Si se subió correctamente, devuelve el enlace de descarga
                //var downloadLink = uploadedFile.WebUrl;


                return Ok(response);
            }

            catch (Exception ex)
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

        //GET: listarSubMotivo
        [HttpGet]
        [Route("api/contacto/probando")]
        public async Task<IHttpActionResult> prueba()
        {

            try
            {
                string tenantId = "9a2bc5f0-580b-4178-aa35-836e9eb5b4e8";
                string clientId = "99129ae9-d834-4268-a763-13782d187fc9";
                string clientSecret = "vl-8Q~Yh17uqALTmrdBX9AxrRPYNHjQ3XEDgob9A";
                string authority = "https://login.microsoftonline.com/" + tenantId;
                string scope = "https://graph.microsoft.com/.default";

                var confidentialClientApplication = ConfidentialClientApplicationBuilder
                   .Create(clientId)
                   .WithClientSecret(clientSecret)
                   .WithAuthority(new Uri(authority))
                   .Build();

                var authenticationResult = await confidentialClientApplication
                    .AcquireTokenForClient(new string[] { scope })
                    .ExecuteAsync();

                var accessToken = authenticationResult.AccessToken;

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await httpClient.GetAsync("https://graph.microsoft.com/v1.0/me");
                var content = await response.Content.ReadAsStringAsync();


                //var graphHandler = new GraphHandler(tenantId, clientId, clientSecret);

                //var user = await graphHandler.GetUser("gianpvp99@gmail.com");
                //Console.WriteLine(user?.DisplayName);

                return Ok("true");
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


public class GraphHandler{

    public GraphServiceClient GraphClient { get; private set; }
    public GraphHandler(string tenantId, string clientId, string clientSecret)
    {
        GraphClient = CreateGraphClient(tenantId, clientId, clientSecret);
    }

    public GraphServiceClient CreateGraphClient(string tenantId, string clientId, string clientSecret)
    {
        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        var clientSecretCredential = new ClientSecretCredential(
            tenantId, clientId, clientSecret, options);
        var scopes = new[] { "https://graph.microsoft.com/.default" };

        return new GraphServiceClient(clientSecretCredential, scopes);

    }

    public async Task<Microsoft.Graph.Models.User> GetUser(string userPrincipalName)
    {
        return await GraphClient.Users[userPrincipalName].GetAsync();
    }

    //public async Task<(IEnumerable<Site>, IEnumerable<Site>)> GetSharepointSites()
    //{
    //    var sites = (await GraphClient.Sites.GetAllSites.GetAsync())?.Value;
    //    if (sites == null)
    //    {
    //        return (null, null);
    //    }

    //    sites.RemoveAll(x => string.IsNullOrEmpty(x.DisplayName));

    //    var spSites = new List<Site>();
    //    var oneDriveSites = new List<Site>();

    //    foreach (var site in sites)
    //    {
    //        if (site == null) continue;

    //        var compare = site.WebUrl?.Split(site.SiteCollection?.Hostname)[1].Split("/");
    //        if (compare.All(x => !string.IsNullOrEmpty(x)) || compare.Length < 1)
    //        {
    //            continue;
    //        }

    //        if (compare[1] == "sites" || string.IsNullOrEmpty(compare[1]))
    //            spSites.Add(site);
    //        else if (compare[1] == "personal")
    //            oneDriveSites.Add(site);
    //    }

    //    return (spSites, oneDriveSites);
    //}

}