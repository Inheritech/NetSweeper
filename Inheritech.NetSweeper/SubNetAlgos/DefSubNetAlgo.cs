using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Inheritech.NetSweeper.SubNetAlgos
{
    /// <summary>
    /// Algoritmo base de escaneo
    /// </summary>
    public class DefSubNetAlgo : ISubNetAlgo
    {
        /// <summary>
        /// Valor máximo de la IP
        /// </summary>
        private int _upperBound;

        /// <summary>
        /// Determina si se debe calcular la IP de inicio en base a la IP local
        /// </summary>
        private bool _fastStart;

        /// <summary>
        /// Intervalo de IPs para el inico rápido
        /// </summary>
        private int _fastStartBase;

        /// <param name="upperBound">Valor máximo de la IP</param>
        /// <param name="fastStart">Determina si se debe calcular la IP de inicio en base a la IP local</param>
        /// <param name="fastStartBase">Intervalo de IPs para el inico rápido</param>
        public DefSubNetAlgo(int upperBound = 255, bool fastStart = false, int fastStartBase = 50) {
            _upperBound = upperBound;
            _fastStart = fastStart;
            _fastStartBase = fastStartBase;
        }

        /// <summary>
        /// Obtener sub red
        /// </summary>
        /// <param name="localAddress">Dirección IP</param>
        public IEnumerable<IPAddress> GetSubNet(IPAddress localAddress) {
            byte[] ipBytes = localAddress.GetAddressBytes();
            int startAddress = 1;
            if (_fastStart) {
                float cent = Convert.ToInt32(ipBytes[3]);
                cent /= _fastStartBase;
                cent = (int)Math.Floor(cent);
                startAddress = (int)cent * _fastStartBase;
            }
            for (int i = startAddress; i < _upperBound; i++) {
                ipBytes[3] = Convert.ToByte(i);
                yield return new IPAddress(ipBytes);
            }
        }
    }
}
