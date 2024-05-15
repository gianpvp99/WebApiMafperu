using System.Collections.Generic;
using System.Security.Policy;

namespace WebApiMafperu.Models
{
    public class DatosUsuario 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class TipoDocumentoList 
    {
        public List<DatosTipoDocumento> lista { get; set; }
    }
    public class DatosTipoDocumento 
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class DatosCliente
    {
        public string Apellido_materno { get; set; }
        public string Apellido_paterno { get; set; }
        public string Email { get; set; }
        public string Nro_documento { get; set; }
        public string Primer_nombre {  get; set; }
        public string Segundo_nombre { get; set; }
        public string Telefono { get; set; }
        public string Telefono2 { get; set; }
        public string Tipo { get; set; }

        public bool error { get; set; }

    }

    public class Adjunto
    {
        public bool Estado { get; set; }
        public string Mensaje { get; set; }
    }

    public class DepartamentoList
    {
        public List<DatosDepartamento> lista { get; set; }
    }
    public class DatosDepartamento
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
    public class ProvinciaList
    {
        public List<DatosProvincia> lista { get; set; }
    }
    public class DatosProvincia
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
    public class DistritoList
    {
        public List<DatosDistrito> lista { get; set; }
    }
    public class DatosDistrito
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
    public class MotivoList
    {
        public List<DatosMotivo> lista { get; set; }
    }
    public class DatosMotivo
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
    public class SubMotivoList
    {
        public List<DatosSubMotivo> lista { get; set; }
    }
    public class DatosSubMotivo
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
    public class DatosRegistroCrm 
    {
        public int RepresentaEmpresa { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string NumeroContrato { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string uDepartamento { get; set; }
        public string uProvincia { get; set; }
        public string uDistrito { get; set; }
        public string uDireccion { get; set; }
        public string eRazonSocial { get; set; }
        public string eTipoDoc { get; set; }
        public string eNroDoc { get; set; }
        public string TipoCaso { get; set; }
        public string TipoProducto { get; set; }
        public string Motivo { get; set; }
        public string SubMotivo { get; set; }
        public string DetalleCaso { get; set; }
        public string TipoProductoLib { get; set; }
        public string EnvioNotificacion { get; set; }
        public string Placa { get; set; }
    }
    public class DatosAdjuntoCrm 
    {
        public string Asunto { get; set; }
        public string gEntidad { get; set; }
        public string NombreArchivo { get; set; }
        public string Data { get; set; }
    }
    public class RespuestaCrm
    {
        public int indicadorExito { get; set; }
        public string descripcionError { get; set; }
    }
    public class RespuestaWS 
    {
        public string idreclamo { get; set; }
        public int indicadorExito { get; set; }
        public string guid { get; set; }
        public string asunto { get; set; }
    }
    public class DatosRegistroForm 
    {
        public string RazonSocial { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string NumeroContrato { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string Motivo { get; set; }
        public string SubMotivo { get; set; }
        public string DetalleCaso { get; set; }
        public string TipoCaso { get; set; }
        public string TipoProductoLib { get; set; }
        public string EnvioNotificacion { get; set; }
        public string Placa { get; set; }
    }
    public class DatosRegistroCrm2
    {
        public int RepresentaEmpresa { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string uDepartamento { get; set; }
        public string uProvincia { get; set; }
        public string uDistrito { get; set; }
        public string uDireccion { get; set; }
        public string eRazonSocial { get; set; }
        public string eTipoDoc { get; set; }
        public string eNroDoc { get; set; }
        public string TipoCaso { get; set; }
        public string TipoProducto { get; set; }
        public string Motivo { get; set; }
        public string SubMotivo { get; set; }
        public string DetalleCaso { get; set; }
        public string TipoProductoLib { get; set; }
        public string EnvioNotificacion { get; set; }

        /*INI-Datos Adicionales*/
        public string Edad { get; set; }
        public string DesTipoDoc { get; set; }
        public string Empresa { get; set; }
        public string DesTipoDoc2 { get; set; }
        public string NroDoc2 { get; set; }
        public string Parentesco { get; set; }
        public string Nombres2 { get; set; }
        public string ApellidoPaterno2 { get; set; }
        public string ApellidoMaterno2 { get; set; }
        public string DesDepartamento { get; set; }
        public string DesProvincia { get; set; }
        public string DesDistrito { get; set; }
        public string Referencia { get; set; }
        public string DesCarta { get; set; }
        public string Caso { get; set; }
        public string DesTipoProd { get; set; }
        public string NroOper { get; set; }
        public string Monto { get; set; }
        public string DesMotivo { get; set; }
        public string DesSubMotivo { get; set; }
        public string DesPedido { get; set; }
        public string TieneAdjunto { get; set; }
        /*FIN-Datos Adicionales*/
    }
    public class DatosRegistroForm2
    {
        public string RazonSocial { get; set; }
        public string TipoDoc { get; set; }
        public string NroDoc { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public string Motivo { get; set; }
        public string SubMotivo { get; set; }
        public string DetalleCaso { get; set; }
        public string TipoCaso { get; set; }
        public string TipoProductoLib { get; set; }
        public string EnvioNotificacion { get; set; }
        public string DesTipoDoc { get; set; }
        public string DesTipoProducto { get; set; }
        public string DesMotivo { get; set; }
        public string DesSubMotivo { get; set; }
        public string DesConsulta { get; set; }
        public string DesSolicitud { get; set; }
        public string DesTieneAdjunto { get; set; }
        public string DesCondiciones { get; set; }
    }
}