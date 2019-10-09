using Inheritech.NetSweeper.SubNetAlgos;
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
    public class NetUtilities
    {
        /// <summary>
        /// Generar un rango de direcciones IP de subred en base a la IP local
        /// </summary>
        /// <returns>Lista de direcciones IP en base a la subred</returns>
        public static List<IPAddress> GetSubNet(ISubNetAlgo algo = null)
        {
            List<IPAddress> resultList = new List<IPAddress>();
            IPAddress localIP = GetLocalIPAddress();
            if (localIP == null) {
                return resultList;
            }
            if (algo == null) {
                algo = new DefSubNetAlgo();
            }
            resultList.AddRange(algo.GetSubNet(localIP));
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
