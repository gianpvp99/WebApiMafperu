using System.Collections.Generic;

namespace WebApiMafperu.Models
{
    public class LogicaNegocio
    {
        public void insertarInteraccion(Interaction i)
        {
        }
        public void insertarLogEnvioCorreo(string conversationId, string email, string message)
        {
        }
        public List<Ubigeo> listarDepartamento() 
        {
            return new List<Ubigeo>();
        }
        public List<Ubigeo> listarProvincia(string departamento)
        {
            return new List<Ubigeo>();
        }
        public List<Ubigeo> listarDistrito(string departamento, string provincia)
        {
            return new List<Ubigeo>();
        }
        public List<Motivo> listarMotivo()
        {
            return new List<Motivo>();
        }
        public List<Motivo> listarSubMotivo(string nommotivo)
        {
            return new List<Motivo>();
        }
        public string obtenerId(string tipo)
        {
            return null;
        }
        public int validarFeriado(string fecha) 
        {
            return 0;
        }
    }
}