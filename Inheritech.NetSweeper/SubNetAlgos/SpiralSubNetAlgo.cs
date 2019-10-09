using System;
using System.Collections.Generic;
using System.Net;

namespace Inheritech.NetSweeper.SubNetAlgos
{
    /// <summary>
    /// <para>Algoritmo en espiral de escaneo</para>
    /// <para>Este algoritmo retorna primero las direcciones adyacentes
    /// a la dirección local</para>
    /// </summary>
    public class SpiralSubNetAlgo : ISubNetAlgo
    {
        /// <summary>
        /// Obtener sub red
        /// </summary>
        /// <param name="localAddress">Dirección local</param>
        public IEnumerable<IPAddress> GetSubNet(IPAddress localAddress) {
            byte[] ipBytes = localAddress.GetAddressBytes();
            int lowerAddress = Convert.ToInt32(ipBytes[3]);
            int upperAddress = Convert.ToInt32(ipBytes[3]);
            while (lowerAddress > 1 || upperAddress < 254) {
                if (lowerAddress > 1) {
                    lowerAddress--;
                    ipBytes[3] = Convert.ToByte(lowerAddress);
                    yield return new IPAddress(ipBytes);
                }
                if (upperAddress < 254) {
                    upperAddress++;
                    ipBytes[3] = Convert.ToByte(upperAddress);
                    yield return new IPAddress(ipBytes);
                }
            }
        }
    }
}
