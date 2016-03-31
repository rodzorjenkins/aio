using BDProxy.Network;

namespace BDProxy.Core
{
    public class MainContext
    {

        public static TcpProxy loginProxy, gameProxy;
        public static bool IsPlayerIngame { get; set; }
        
    }
}