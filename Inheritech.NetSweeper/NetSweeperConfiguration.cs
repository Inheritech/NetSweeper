using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Inheritech.NetSweeper
{
    /// <summary>
    /// Protocolos de red para el barrido de red
    /// </summary>
    public enum Protocol
    {
        /// <summary>
        /// Protocolo HTTP estandar
        /// </summary>
        HTTP,
        /// <summary>
        /// Protocolo HTTP seguro
        /// </summary>
        HTTPS
    }

    public class NetSweeperConfiguration
    {
        /// <summary>
        /// Puerto al cual realizar las solicitudes
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Identificador de recurso al cual hacer las solicitudes
        /// </summary>
        public string ResourceUri { get; }

        /// <summary>
        /// Protocolo de red a utilizar
        /// </summary>
        public Protocol Protocol { get; }

        /// <summary>
        /// Expresión regular para utilizar como validación de respuestas
        /// </summary>
        public Func<string, IPAddress, bool> AcceptanceFunction { get; }

        /// <summary>
        /// Crear un nuevo objeto de configuración de barredor de red
        /// </summary>
        /// <param name="port">Puerto de solicitud</param>
        /// <param name="resource">Recurso de solicitud</param>
        /// <param name="protocol">Protocolo de red</param>
        /// <param name="acceptanceExpression">Expresión regular para validar</param>
        public NetSweeperConfiguration(
            int port,
            string resource,
            Protocol protocol,
            Func<string, IPAddress, bool> acceptanceFunction
        )
        {
            Port = port;
            ResourceUri = resource;
            Protocol = protocol;
            AcceptanceFunction = acceptanceFunction;
        }
    }
}
