namespace BDCustomLauncher.Network.Frames
{
    public class Cbt2
    {
        public string predownload { get; set; }
    }

    public class Common
    {
        public string tou_form { get; set; }
        public string error { get; set; }
        public string login { get; set; }
        public string menu { get; set; }
        public string home { get; set; }
    }

    public class L
    {
        public Cbt2 cbt2 { get; set; }
        public Common common { get; set; }
        public string paramsJson { get; set; }
        public string ipCountry { get; set; }
        public string locale { get; set; }
        public string support { get; set; }
    }

    public class Result
    {
        public L l { get; set; }
        public string token { get; set; }
    }

    public class AdditionalInfo
    {
        public string msg { get; set; }
        public string code { get; set; }
    }

    public class Api
    {
        public int code { get; set; }
        public string codeMsg { get; set; }
        public AdditionalInfo additionalInfo { get; set; }
    }

    public class LoginFrame
    {
        public Result result { get; set; }
        public Api api { get; set; }
    }
}