using System.Collections.Generic;
using System.Net;

namespace Inheritech.NetSweeper.SubNetAlgos
{
    /// <summary>
    /// Algoritmo de sub-red a utilizar
    /// </summary>
    public interface ISubNetAlgo
    {
        /// <summary>
        /// Obtener direcciones IP de subred en base a este algoritmo
        /// </summary>
        /// <param name="localAddress">Dirección IP local</param>
        IEnumerable<IPAddress> GetSubNet(IPAddress localAddress);
    }
}
