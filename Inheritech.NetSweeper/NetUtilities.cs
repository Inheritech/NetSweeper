using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Inheritech.NetSweeper
{
    /// <summary>
    /// Utilidades de red
    /// </summary>
    class NetUtilities
    {
        /// <summary>
        /// Generar un rango de direcciones IP de subred en base a la IP local
        /// </summary>
        /// <param name="upperBound">Valor máximo de la IP</param>
        /// <param name="fastStart">Determina si se debe calcular la IP de inicio en base a la IP local</param>
        /// <param name="fastStartBase">Intervalo de IPs para el inico rápido</param>
        /// <returns>Lista de direcciones IP en base a la subred</returns>
        public static List<IPAddress> GetSubNet(int upperBound = 255, bool fastStart = false, int fastStartBase = 50)
        {
            List<IPAddress> resultList = new List<IPAddress>();
            IPAddress localIP = GetLocalIPAddress();
            byte[] ipBytes = localIP.GetAddressBytes();
            int startAddress = 1;
            if (fastStart) {
                float cent = Convert.ToInt32(ipBytes[3]);
                cent /= fastStartBase;
                cent = (int)Math.Floor(cent);
                startAddress = (int)cent * fastStartBase;
            }
            for (int i = startAddress; i < upperBound; i++) {
                ipBytes[3] = Convert.ToByte(i);
                resultList.Add(new IPAddress(ipBytes));
            }

            return resultList;
        }

        /// <summary>
        /// Obtener la dirección IP local o retornar null
        /// </summary>
        /// <returns>Objeto de dirección IP</returns>
        public static IPAddress GetLocalIPAddress()
        {
            try {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList) {
                    if (ip.AddressFamily == AddressFamily.InterNetwork) {
                        return ip.MapToIPv4();
                    }
                }
            }
            catch (Exception) {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface network in networkInterfaces) {
                    IPInterfaceProperties properties = network.GetIPProperties();

                    foreach (IPAddressInformation address in properties.UnicastAddresses) {

                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        if (IPAddress.IsLoopback(address.Address))
                            continue;

                        return address.Address.MapToIPv4();
                    }
                }
            }

            return null;
        }
    }
}
