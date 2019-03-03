using System.Net;

namespace Inheritech.NetSweeper
{
    /// <summary>
    /// Estructura de datos sobre el estado de un barrido
    /// </summary>
    public class SweepStatus
    {
        /// <summary>
        /// Dirección IP actual
        /// </summary>
        public IPAddress NetAddress { get; }

        /// <summary>
        /// Determina si la IP fue encontrada
        /// </summary>
        public bool Found { get; }

        /// <summary>
        /// Determina si ya ha terminado el barrido
        /// </summary>
        public bool Finished { get; }

        /// <summary>
        /// Construir estructura
        /// </summary>
        /// <param name="current">Dirección IP actual</param>
        /// <param name="finished">Ha terminado ya el barrido</param>
        /// <param name="found">La IP ha sido encontrada ( Y es la actual )</param>
        public SweepStatus(IPAddress current, bool finished, bool found)
        {
            NetAddress = current;
            Finished = finished;
            Found = found;
        }
    }
}
