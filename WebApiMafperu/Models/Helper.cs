using NLog;
using System;
using System.IO;
using Renci.SshNet;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Web.Http.Results;

namespace WebApiMafperu.Models
{
    public static class Helper
    {
        public static void enviar_sftp(string folder)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            string host = System.Configuration.ConfigurationManager.AppSettings["ServidorArchivo"].ToString();
            string repositorio = System.Configuration.ConfigurationManager.AppSettings["RepositorioArchivo"].ToString();
            int puerto = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["PuertoArchivo"]);
            string usuario = System.Configuration.ConfigurationManager.AppSettings["UsuarioArchivo"].ToString();
            string clave = System.Configuration.ConfigurationManager.AppSettings["ClaveArchivo"].ToString();
            string directorio = System.Configuration.ConfigurationManager.AppSettings["Uploads"].ToString();

            logger.Info("Inicio Envío a SFTP");
            try
            {
                //Validamos..
                string _directorio = Path.Combine(directorio, folder);
                string[] _files = Directory.GetFiles(_directorio);

                logger.Info("Cant. Archivos: " + _files.Length.ToString());

                if (_files != null && _files.Length > 0)
                {
                    using (SftpClient client = new SftpClient(host, puerto, usuario, clave))
                    {
                        {
                            client.Connect();

                            logger.Info("Conectado a: " + host.ToString());

                            client.ChangeDirectory(repositorio);

                            //Inicializando..
                            var rutaCliente = string.Format("{0}/{1}", repositorio, folder);

                            if (!client.Exists(rutaCliente))
                            {
                                client.CreateDirectory(rutaCliente);
                            }

                            client.ChangeDirectory(rutaCliente);

                            foreach (string e in _files)
                            {
                                using (var fileStream = new FileStream(e.ToString(), FileMode.Open))
                                {
                                    var _fileName = Path.GetFileName(e.ToString());

                                    logger.Info("Transferencia de " + _fileName.ToString() + " : " + fileStream.Length.ToString() + " bytes");

                                    client.UploadFile(fileStream, _fileName, null);
                                }
                            }
                        }
                    }
                }
                else
                {
                    logger.Info("No se encontraron archivos para transferir. Por favor, verifique.");
                }
            }
            catch (Exception error)
            {
                logger.Info("Error Envío a SFTP: " + error.ToString());
            }
            logger.Info("Fin Envío a SFTP");
        }
        public static async Task<List<RespuestaCrm>> enviar_adjunto(string asunto, string guid, string folder)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            string directorio = System.Configuration.ConfigurationManager.AppSettings["Uploads"].ToString();

            logger.Info("Inicio Envío Adjunto CRM");
            try
            {
                //Validamos..
                string _directorio = System.Web.Hosting.HostingEnvironment.MapPath($"~/{directorio}/Adjuntos_SeguroVehicular/{folder}");
                string[] _files = Directory.GetFiles(_directorio);

                List<RespuestaCrm> resultado = new List<RespuestaCrm>();
                WebApiCrm wsReclamo = new WebApiCrm();

                logger.Info("Cant. Archivos: " + _files.Length.ToString());

                if (_files != null && _files.Length > 0)
                {
                    foreach (string e in _files)
                    {
                        using (var fileStream = new FileStream(e.ToString(), FileMode.Open))
                        {
                            var ms = new MemoryStream();
                            fileStream.CopyTo(ms);

                            byte[] data = ms.ToArray();

                            string _fileBase64 = Convert.ToBase64String(data);
                            string _fileName = Path.GetFileName(e.ToString());

                            DatosAdjuntoCrm datosAdjuntoCrm = new DatosAdjuntoCrm();
                            datosAdjuntoCrm.Asunto = asunto;
                            datosAdjuntoCrm.gEntidad = guid;
                            datosAdjuntoCrm.NombreArchivo = _fileName;
                            datosAdjuntoCrm.Data = _fileBase64;

                            //resultado = await wsReclamo.adjuntarCrm(datosAdjuntoCrm);
                            resultado.Add(await wsReclamo.adjuntarCrm(datosAdjuntoCrm));
                            logger.Info("enviar_adjunto =>" + JsonConvert.SerializeObject(resultado));
                        }
                    }
                }
                else
                {
                    logger.Info("No se encontraron archivos para adjuntar. Por favor, verifique.");
                }
                return resultado;

            }
            catch (Exception error)
            {
                logger.Info("Error Adjunto CRM: " + error.ToString());
                throw error;
            }

        }
        public static string generar_mensaje(DatosRegistroCrm2 input) 
        {
            string _mensaje = string.Empty;

            try
            {
                _mensaje = "<!DOCTYPE html>";
                _mensaje = _mensaje + "<html>";
                _mensaje = _mensaje + "<head>";
                _mensaje = _mensaje + "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>";
                _mensaje = _mensaje + "<title></title>";
                _mensaje = _mensaje + "</head>";
                _mensaje = _mensaje + "<body>";
                _mensaje = _mensaje + "<p><b>[TITULO]</b></p>";
                _mensaje = _mensaje + "<p>¿Es menor de Edad?:" + " " + input.Edad + "</p>";

                if (input.Edad == "No")
                {
                    _mensaje = _mensaje + "<p>Tipo de Documento:" + " " + input.DesTipoDoc + "</p>";
                    _mensaje = _mensaje + "<p>Número de Documento:" + " " + input.NroDoc + "</p>";
                    _mensaje = _mensaje + "<p>Nombres:" + " " + input.Nombres + "</p>";
                    _mensaje = _mensaje + "<p>Primer apellido:" + " " + input.ApellidoPaterno + "</p>";
                    _mensaje = _mensaje + "<p>Segundo apellido:" + " " + input.ApellidoMaterno + "</p>";
                    _mensaje = _mensaje + "<p>¿Representa a una empresa?:" + " " + input.Empresa + "</p>";

                    if (input.Empresa == "Si")
                    {
                        _mensaje = _mensaje + "<p>Razón Social:" + " " + input.eRazonSocial + "</p>";
                        _mensaje = _mensaje + "<p>Tipo de Documento:" + " " + input.eTipoDoc + "</p>";
                        _mensaje = _mensaje + "<p>Número de Documento:" + " " + input.eNroDoc + "</p>";
                    }
                }
                else if (input.Edad == "Si")
                {
                    _mensaje = _mensaje + "<p>Del solicitante</p>";
                    _mensaje = _mensaje + "<p>Tipo de Documento:" + " " + input.DesTipoDoc + "</p>";
                    _mensaje = _mensaje + "<p>Número de Documento:" + " " + input.NroDoc + "</p>";
                    _mensaje = _mensaje + "<p>Nombres:" + " " + input.Nombres + "</p>";
                    _mensaje = _mensaje + "<p>Primer apellido:" + " " + input.ApellidoPaterno + "</p>";
                    _mensaje = _mensaje + "<p>Segundo apellido:" + " " + input.ApellidoMaterno + "</p>";
                    _mensaje = _mensaje + "<p>Del apoderado</p>";
                    _mensaje = _mensaje + "<p>Tipo de Documento:" + " " + input.DesTipoDoc2 + "</p>";
                    _mensaje = _mensaje + "<p>Número de Documento:" + " " + input.NroDoc2 + "</p>";
                    _mensaje = _mensaje + "<p>Parentesco:" + " " + input.Parentesco + "</p>";
                    _mensaje = _mensaje + "<p>Nombres:" + " " + input.Nombres2 + "</p>";
                    _mensaje = _mensaje + "<p>Primer apellido:" + " " + input.ApellidoPaterno2 + "</p>";
                    _mensaje = _mensaje + "<p>Segundo apellido:" + " " + input.ApellidoMaterno2 + "</p>";
                }

                _mensaje = _mensaje + "<p>Email:" + " " + input.Email + "</p>";
                _mensaje = _mensaje + "<p>Teléfono Celular:" + " " + input.Celular + "</p>";
                _mensaje = _mensaje + "<p>Departamento:" + " " + input.DesDepartamento + "</p>";
                _mensaje = _mensaje + "<p>Provincia:" + " " + input.DesProvincia + "</p>";
                _mensaje = _mensaje + "<p>Distrito:" + " " + input.DesDistrito + "</p>";
                _mensaje = _mensaje + "<p>Dirección actual:" + " " + input.uDireccion + "</p>";
                _mensaje = _mensaje + "<p>Referencia:" + " " + input.Referencia + "</p>";
                _mensaje = _mensaje + "<p>Medio elegido para envío de respuesta:" + " " + input.DesCarta + "</p>";
                _mensaje = _mensaje + "<p>Tipo de Caso:" + " " + input.Caso + "</p>";
                _mensaje = _mensaje + "<p>Tipo de Producto:" + " " + input.DesTipoProd + "</p>";
                _mensaje = _mensaje + "<p>Número de Operación o Placa:" + " " + input.NroOper + "</p>";
                _mensaje = _mensaje + "<p>Monto Reclamado:" + " " + input.Monto + "</p>";
                _mensaje = _mensaje + "<p>Motivo:" + " " + input.DesMotivo + "</p>";
                _mensaje = _mensaje + "<p>Sub Motivo:" + " " + input.DesSubMotivo + "</p>";
                _mensaje = _mensaje + "<p>Brindanos mas detalle:" + " " + input.DetalleCaso + "</p>";
                _mensaje = _mensaje + "<p>Pedido:" + " " + input.DesPedido + "</p>";
                _mensaje = _mensaje + "<p>¿Adjunto documentos?:" + " " + input.TieneAdjunto + "</p>";
                _mensaje = _mensaje + "</body>";
                _mensaje = _mensaje + "</html>";
            }
            catch (Exception ex)
            {
                _mensaje = "";
            }

            return _mensaje;
        }
        public static string generar_mensaje2(DatosRegistroForm2 input)
        {
            string _mensaje = string.Empty;

            try
            {
                _mensaje = "<!DOCTYPE html>";
                _mensaje = _mensaje + "<html>";
                _mensaje = _mensaje + "<head>";
                _mensaje = _mensaje + "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>";
                _mensaje = _mensaje + "<title></title>";
                _mensaje = _mensaje + "</head>";
                _mensaje = _mensaje + "<body>";
                _mensaje = _mensaje + "<p><b>Datos del Formulario</b></p>";
                _mensaje = _mensaje + "<p>- Tipo de documento:" + " " + input.DesTipoDoc + "</p>";
                _mensaje = _mensaje + "<p>- Número de documento:" + " " + input.NroDoc + "</p>";
                _mensaje = _mensaje + "<p>- Tus nombres o razón social:" + " " + input.Nombres + "</p>";
                _mensaje = _mensaje + "<p>- Apellido Paterno:" + " " + input.ApellidoPaterno + "</p>";
                _mensaje = _mensaje + "<p>- Apellido Materno:" + " " + input.ApellidoMaterno + "</p>";
                _mensaje = _mensaje + "<p>- Teléfono:" + " " + input.Celular + "</p>";
                _mensaje = _mensaje + "<p>- Email:" + " " + input.Email + "</p>";
                _mensaje = _mensaje + "<p>- Tipo de Producto:" + " " + input.DesTipoProducto + "</p>";
                _mensaje = _mensaje + "<p>- Motivo:" + " " + input.DesMotivo + "</p>";
                _mensaje = _mensaje + "<p>- Sub Motivo:" + " " + input.DesSubMotivo + "</p>";
                _mensaje = _mensaje + "<p>- Descripción:" + " " + input.DetalleCaso + "</p>";
                _mensaje = _mensaje + "<p>- Indicador de Consulta:" + " " + input.DesConsulta + "</p>";
                _mensaje = _mensaje + "<p>- Indicador de Solicitud:" + " " + input.DesSolicitud + "</p>";
                _mensaje = _mensaje + "<p>- Adjunto documentos:" + " " + input.DesTieneAdjunto + "</p>";
                _mensaje = _mensaje + "<p>- Acepta condiciones:" + " " + input.DesCondiciones + "</p>";
                _mensaje = _mensaje + "</body>";
                _mensaje = _mensaje + "</html>";
            }
            catch (Exception ex)
            {
                _mensaje = "";
            }

            return _mensaje;
        }
    }
}