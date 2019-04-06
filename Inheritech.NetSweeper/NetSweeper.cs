using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inheritech.NetSweeper
{
    /// <summary>
    /// Resultado de operación de revisión de dirección IP
    /// </summary>
    public enum CheckAddressResult
    {
        /// <summary>
        /// IP encontrada
        /// </summary>
        Found,
        /// <summary>
        /// IP no encontrada
        /// </summary>
        NotFound,
        /// <summary>
        /// Operacion cancelada
        /// </summary>
        Canceled
    }

    /// <summary>
    /// Clase de apoyo para realizar barridos de red
    /// </summary>
    public sealed class NetSweeper
    {
        /// <summary>
        /// Al emitir un estado de barrido
        /// </summary>
        public event EventHandler<SweepStatus> OnStatus;

        /// <summary>
        /// Determina si el barrido está en progreso
        /// </summary>
        public bool Running { get; private set; }

        /// <summary>
        /// Cliente HTTP utilizado
        /// </summary>
        private HttpClient m_client;

        /// <summary>
        /// Configuración del barredor
        /// </summary>
        private NetSweeperConfiguration m_config;

        /// <summary>
        /// IPs de la sub-red/LAN
        /// </summary>
        private readonly List<IPAddress> m_subNet;

        /// <summary>
        /// Grado de paralelismo a utilizar
        /// </summary>
        private int m_maxParallelismDegree;

        /// <summary>
        /// Construir Net Sweeper
        /// </summary>
        /// <param name="config">Configuración</param>
        /// <param name="client">Cliente HTTP a utilizar o nulo para crear uno nuevo</param>
        /// <param name="upperBound">Numero más alto en las direcciones IP de la subred</param>
        /// <param name="fastStart">Determina si se debe calcular una IP adelantada en caso que la red empiece más alla del 0</param>
        /// <param name="fastStartBase">Intervalo de IPs para el inicio rápido</param>
        public NetSweeper(
            NetSweeperConfiguration config,
            HttpClient client = null,
            int maxParallelismDegree = 1,
            int upperBound = 255,
            bool fastStart = false,
            int fastStartBase = 50
        )
        {
            m_subNet = NetUtilities.GetSubNet(upperBound, fastStart, fastStartBase);
            if (client == null) {
                client = new HttpClient();
            }
            m_client = client;
            m_config = config;
            m_maxParallelismDegree = maxParallelismDegree;
        }

        /// <summary>
        /// Realizar el barrido de red
        /// </summary>
        /// <returns></returns>
        public void Sweep()
        {
            if (Running)
                return;

            Running = true;

            CheckAddresses(m_subNet);
        }

        /// <summary>
        /// Revisar direcciones IP
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private void CheckAddresses(List<IPAddress> addresses)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            try {
                Parallel.ForEach(addresses, new ParallelOptions
                {
                    CancellationToken = tokenSource.Token,
                    MaxDegreeOfParallelism = m_maxParallelismDegree
                }, async (address) =>
                {
                    var result = await CheckAddress(address, tokenSource.Token);
                    switch (result) {
                        case CheckAddressResult.Found:
                            Debug.WriteLine("Found Address: " + address.ToString());
                            RaiseStatusEvent(address, finished: true, found: true);
                            Running = false;
                            tokenSource.Cancel();
                            break;
                        case CheckAddressResult.NotFound:
                            Debug.WriteLine("Checked Address: " + address.ToString());
                            RaiseStatusEvent(address, finished: false, found: false);
                            break;
                    }
                });
            } catch(Exception) {

            }
        }

        /// <summary>
        /// Revisar respuesta de dirección IP
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private async Task<CheckAddressResult> CheckAddress(IPAddress address, CancellationToken token)
        {
            string url = GenerateUrl(address);
            try {
                HttpResponseMessage response = await m_client.GetAsync(url, token);
                if (token.IsCancellationRequested)
                    return CheckAddressResult.Canceled;

                if (!response.IsSuccessStatusCode) {
                    return CheckAddressResult.NotFound;
                }
                string responseBody = await response.Content.ReadAsStringAsync();
                if (m_config.AcceptanceFunction(responseBody)) {
                    return CheckAddressResult.Found;
                }
            } catch (OperationCanceledException) {
                return CheckAddressResult.Canceled;
            } catch (Exception e) {
                Debug.WriteLine(e);
            }

            return CheckAddressResult.NotFound;
        }

        /// <summary>
        /// Generar URL a recurso en base a configuración
        /// </summary>
        /// <param name="address">Dirección IP sobre la cual generar la URL</param>
        /// <returns>URL al recurso de acuerdo a la IP provista</returns>
        private string GenerateUrl(IPAddress address)
        {
            Uri baseUri = new Uri($"{m_config.Protocol}://{address}:{m_config.Port}/");
            return new Uri(baseUri, m_config.ResourceUri).ToString();
        }

        /// <summary>
        /// Emitir evento de estado
        /// </summary>
        /// <param name="address">Dirección IP</param>
        /// <param name="finished">Determina si ha acabado el barrido</param>
        /// <param name="found">Determina si se ha encontrado la IP</param>
        private void RaiseStatusEvent(IPAddress address, bool finished, bool found)
        {
            OnStatus?.Invoke(this, new SweepStatus(address, finished, found));
        }
    }
}
