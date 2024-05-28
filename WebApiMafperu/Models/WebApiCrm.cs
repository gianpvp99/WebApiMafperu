using NLog;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using System;
using Microsoft.Ajax.Utilities;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace WebApiMafperu.Models
{
    public class WebApiCrm
    {
        private static string token = string.Empty;
        private static string usuario = System.Configuration.ConfigurationManager.AppSettings["usuario"].ToString();
        private static string password = System.Configuration.ConfigurationManager.AppSettings["password"].ToString();
        private static string urlCRM = System.Configuration.ConfigurationManager.AppSettings["urlCRM"].ToString();

        private static Logger logger = LogManager.GetCurrentClassLogger();
        public WebApiCrm() 
        {
            obtenerTokenCrm();
        }
        public void obtenerTokenCrm()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = $"{urlCRM}/login/authenticate";

            DatosUsuario datosUsuario = new DatosUsuario();
            datosUsuario.Username = usuario;
            datosUsuario.Password = password;

            var json = JsonConvert.SerializeObject(datosUsuario);

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(json);

            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"""", "");

                token = resultado;
            }
        }
        public List<DatosMotivo> listarMotivo(string tipo)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = $"{urlCRM}/Motivo";
            //var uri = string.Format("{0}={1}", "", tipo);

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            request.AddQueryParameter("Tipo", tipo);

            var response = client.Execute(request);

            var responseJson = new List<DatosMotivo>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                responseJson = JsonConvert.DeserializeObject<List<DatosMotivo>>(resultado);
            }

            return responseJson;
        }
        public List<DatosSubMotivo> listarSubMotivo(string tipo, string motivo)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //var uri = string.Format("{0}={1}&{2}={3}", "", tipo, "Motivo", id);
            var uri = $"{urlCRM}/SubMotivo";

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            request.AddQueryParameter("Tipo", tipo);
            request.AddQueryParameter("Motivo", motivo);
            var response = client.Execute(request);

            var responseJson = new List<DatosSubMotivo>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                responseJson = JsonConvert.DeserializeObject<List<DatosSubMotivo>>(resultado);
            }

            return responseJson;
        }
        public List<DatosTipoDocumento> listarTipoDocumento()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = $"{urlCRM}/TipoDoc";

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);

            var responseJson = new List<DatosTipoDocumento>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                responseJson = JsonConvert.DeserializeObject<List<DatosTipoDocumento>>(resultado);
            }

            return responseJson;
        }
        public DatosCliente listarCliente(string nroDocumento)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = $"{urlCRM}/cliente/ObtenerPorDocumento";

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            request.AddQueryParameter("documento", nroDocumento);
            
            var response = client.Execute(request);

            var responseJson = new DatosCliente();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                responseJson = JsonConvert.DeserializeObject<DatosCliente>(resultado);
            }

            return responseJson;
        }
        public List<ClienteContrato> listarCliente2(string nroDocumento)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = $"{urlCRM}/contrato/ObtenerPorDocumento";

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            request.AddQueryParameter("documento", nroDocumento);

            var response = client.Execute(request);

            var responseJson = new List<ClienteContrato>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                var responseJsonArray = JsonConvert.DeserializeObject<List<ClienteContrato>>(resultado);

                if (responseJsonArray != null && responseJsonArray.Count > 0)
                {
                    return responseJsonArray;
                }
                else
                    return new List<ClienteContrato>() { };
                }
            else
            {
                return new List<ClienteContrato>() {};
            }

        }

        public List<DatosDepartamento> listarDepartamento()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = "";

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);

            var responseJson = new List<DatosDepartamento>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                responseJson = JsonConvert.DeserializeObject<List<DatosDepartamento>>(resultado);
            }

            return responseJson;
        }
        public List<DatosProvincia> listarProvincia(string CodDepartamento)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = string.Format("{0}?{1}={2}", "", "CodDepartamento", CodDepartamento);

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);

            var responseJson = new List<DatosProvincia>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                responseJson = JsonConvert.DeserializeObject<List<DatosProvincia>>(resultado);
            }

            return responseJson;
        }
        public List<DatosDistrito> listarDistrito(string CodProvincia)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = string.Format("{0}?{1}={2}", "", "CodProvincia", CodProvincia);

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            var response = client.Execute(request);

            var responseJson = new List<DatosDistrito>();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();
                resultado = resultado.Replace(@"\", "");
                resultado = resultado.TrimStart('"');
                resultado = resultado.TrimEnd('"');

                responseJson = JsonConvert.DeserializeObject<List<DatosDistrito>>(resultado);
            }

            return responseJson;
        }
        public RespuestaCrm registrarCrm(DatosRegistroCrm datosRegistro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = $"{urlCRM}/RegistroForm";

            var r = new DatosRegistroCrm();
            r.ApellidoMaterno = datosRegistro.ApellidoMaterno;
            r.ApellidoPaterno = datosRegistro.ApellidoPaterno;
            r.Celular = datosRegistro.Celular;
            r.DetalleCaso = datosRegistro.DetalleCaso;
            r.Email = datosRegistro.Email;
            r.EnvioNotificacion = datosRegistro.EnvioNotificacion;
            r.Motivo = datosRegistro.Motivo;
            r.Nombres = datosRegistro.Nombres;
            r.NroDoc = datosRegistro.NroDoc;
            r.NumeroContrato = datosRegistro.NumeroContrato;
            r.Placa = datosRegistro.Placa;
            r.SubMotivo = datosRegistro.SubMotivo;
            r.TipoCaso = datosRegistro.TipoCaso;
            r.TipoDoc = datosRegistro.TipoDoc;
            r.TipoProductoLib = datosRegistro.TipoProductoLib;

            //r.RepresentaEmpresa = datosRegistro.RepresentaEmpresa
            //r.uDepartamento = datosRegistro.uDepartamento;
            //r.uProvincia = datosRegistro.uProvincia;
            //r.uDistrito = datosRegistro.uDistrito;
            //r.uDireccion = datosRegistro.uDireccion;
            //r.eRazonSocial = datosRegistro.eRazonSocial;
            //r.eTipoDoc = datosRegistro.eTipoDoc;
            //r.eNroDoc = datosRegistro.eNroDoc;
            //r.TipoProducto = datosRegistro.TipoProducto;

            var json = JsonConvert.SerializeObject(r);

            logger.Info("Datos de Entrada registrarCrm =>" + json);

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddJsonBody(json);

            var response = client.Execute(request);

            var responseJson = new RespuestaCrm();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();

                responseJson = JsonConvert.DeserializeObject<RespuestaCrm>(resultado);
            }

            return responseJson;
        }
        public async Task<RespuestaCrm> adjuntarCrm(DatosAdjuntoCrm datosAdjunto)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = $"{urlCRM}/adjunto";

            var r = new DatosAdjuntoCrm();
            r.Asunto = datosAdjunto.Asunto;
            r.gEntidad = datosAdjunto.gEntidad;
            r.NombreArchivo = datosAdjunto.NombreArchivo;
            r.Data = datosAdjunto.Data;         

            var json = JsonConvert.SerializeObject(r);

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddJsonBody(json);

            var response = await client.ExecuteAsync(request);

            var responseJson = new RespuestaCrm();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();

                responseJson = JsonConvert.DeserializeObject<RespuestaCrm>(resultado);
            }

            return responseJson;
        }
        public RespuestaCrm registrarForm(DatosRegistroForm datosRegistro)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var uri = "";

            var r = new DatosRegistroForm();
            r.RazonSocial = datosRegistro.RazonSocial;
            r.TipoDoc = datosRegistro.TipoDoc;
            r.NroDoc = datosRegistro.NroDoc;
            r.Nombres = datosRegistro.Nombres;
            r.ApellidoPaterno = datosRegistro.ApellidoPaterno;
            r.ApellidoMaterno = datosRegistro.ApellidoMaterno;
            r.Email = datosRegistro.Email;
            r.Celular = datosRegistro.Celular;
            r.Motivo = datosRegistro.Motivo;
            r.SubMotivo = datosRegistro.SubMotivo;
            r.DetalleCaso = datosRegistro.DetalleCaso;
            r.TipoCaso = datosRegistro.TipoCaso;
            r.TipoProductoLib = datosRegistro.TipoProductoLib;
            r.EnvioNotificacion = datosRegistro.EnvioNotificacion;

            var json = JsonConvert.SerializeObject(r);

            logger.Info("Datos de Entrada registrarForm => " + json);

            var client = new RestClient(uri);
            RestRequest request = new RestRequest("", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddJsonBody(json);

            var response = client.Execute(request);

            var responseJson = new RespuestaCrm();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultado = response.Content.ToString();

                responseJson = JsonConvert.DeserializeObject<RespuestaCrm>(resultado);
            }

            return responseJson;
        }

    }
}