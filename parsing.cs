using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;


namespace PolandTrackerMod
{
    class parsing
    {
        public class ApiPlayer
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("playFabId")]
            public string PlayFabId { get; set; }

            [JsonProperty("color")]
            public string Color { get; set; }

            [JsonProperty("platform")]
            public string Platform { get; set; }
        }

        public class ApiServer
        {
            [JsonProperty("players")]
            public List<ApiPlayer> Players { get; set; }

            [JsonProperty("gameMode")]
            public string GameMode { get; set; }

            [JsonProperty("lastUpdated")]
            public long LastUpdated { get; set; }

            public ApiServer()
            {
                Players = new List<ApiPlayer>();
                GameMode = string.Empty;
            }
        }

        public class Server
        {
            public List<Player> Players { get; set; }
            public string Gamemode { get; set; }
            public string Gmcolor { get; set; }

            public Server()
            {
                Players = new List<Player>();
                Gamemode = string.Empty;
                Gmcolor = string.Empty;
            }
        }

        public class Player
        {
            public string PlayFabId { get; set; }
            public string Name { get; set; }
            public string HexColor { get; set; }
            public string Platform { get; set; }

            public Player()
            {
                PlayFabId = string.Empty;
                Name = string.Empty;
                HexColor = string.Empty;
                Platform = string.Empty;
            }
        }

        private static string GetGameModeColor(string gameMode)
        {
            if (string.IsNullOrEmpty(gameMode)) return "#AAAAAA";
            switch (gameMode.ToUpper())
            {
                case "INFECTION": return "#FF4444";
                case "SUPERINFECT": return "#FF0000";
                case "CASUAL": return "#44FF44";
                case "SUPERCASUAL": return "#00FF00";
                case "HUNT": return "#FFAA00";
                case "PAINTBRAWL": return "#FF44FF";
                case "BATTLE": return "#4444FF";
                default: return "#AAAAAA";
            }
        }

        private static Server ConvertApiServer(ApiServer apiServer)
        {
            var server = new Server();
            if (apiServer == null) return server;

            server.Gamemode = apiServer.GameMode ?? string.Empty;
            server.Gmcolor = GetGameModeColor(apiServer.GameMode);

            if (apiServer.Players != null)
            {
                foreach (var ap in apiServer.Players)
                {
                    server.Players.Add(new Player
                    {
                        Name = ap.Name ?? string.Empty,
                        PlayFabId = ap.PlayFabId ?? string.Empty,
                        HexColor = ap.Color ?? string.Empty,
                        Platform = ap.Platform ?? string.Empty
                    });
                }
            }

            return server;
        }

        public static string addtext(string left, string midle, string right)
        {

            var leftlines = left.Split('\n');
            var midlelines = midle.Split('\n');
            var rightlines = right.Split('\n');
            var maxLines = Math.Max(leftlines.Length, rightlines.Length);

            var finalelines = "";
            for (int i = 0; i < maxLines; i++)
            {
                var lline = i < leftlines.Length ? leftlines[i] : "";
                var mline = i < midlelines.Length ? midlelines[i] : "";
                var rline = i < rightlines.Length ? rightlines[i] : "";
                finalelines += $"{lline}<indent=650>{mline}</indent><indent=1300>{rline}</indent>\n";
            }

            return finalelines;
        }

        private static HttpClient _client;

        private static HttpClient GetClient()
        {
            if (_client == null)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;

                var handler = new HttpClientHandler();
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                _client = new HttpClient(handler);
                _client.Timeout = TimeSpan.FromSeconds(15);
                _client.DefaultRequestHeaders.UserAgent.ParseAdd("HujCiDoMojegoUserAgentaCloudflareOdpierdolSie/1.0");
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            return _client;
        }

        public static async Task EventSource()
        {
            var client = GetClient();

            while (true)
            {
                try
                {
                    var response = await client.GetStringAsync("https://tracker.ovrflow.xyz/api/getplayers");

                    lock (_queue)
                    {
                        _queue.Enqueue(response);
                    }
                }
                catch (Exception ex)
                {
                    lock (_errorQueue)
                    {
                        _errorQueue.Enqueue($"API Error: {ex.GetType().Name}: {ex.Message}");
                    }
                }

                await Task.Delay(5000);
            }
        }

        internal static readonly Queue<string> _errorQueue = new Queue<string>();

        public static string JsonToBoards(string json)
        {
            var apiData = JsonConvert.DeserializeObject<Dictionary<string, ApiServer>>(json);

            ApiServer GetServer(string key)
            {
                if (apiData != null && apiData.TryGetValue(key, out var srv)) return srv;
                return null;
            }

            var polska  = ConvertApiServer(GetServer("POLSKA"));
            var polska1 = ConvertApiServer(GetServer("POLSKA1"));
            var polska2 = ConvertApiServer(GetServer("POLSKA2"));
            var polska3 = ConvertApiServer(GetServer("POLSKA3"));
            var polska4 = ConvertApiServer(GetServer("POLSKA4"));
            var polska5 = ConvertApiServer(GetServer("POLSKA5"));

            var leftcol = $"{ParseString(polska, "POLSKA")}{ParseString(polska3, "POLSKA3")}";
            var midcol = $"{ParseString(polska1, "POLSKA1")}{ParseString(polska4, "POLSKA4")}";
            var rightcol = $"{ParseString(polska2, "POLSKA2")}{ParseString(polska5, "POLSKA5")}";
            return addtext(leftcol, midcol, rightcol);
        }

        internal static readonly Queue<string> _queue = new Queue<string>();
        public static string ParseString(Server code, string nazwakodu)
        {
            try
            {

                var players = "";
                var gmclr = "";
                if (!string.IsNullOrEmpty(code.Gamemode)) gmclr = $"<color={code.Gmcolor}>{code.Gamemode}</color>";
                foreach (var i in code.Players)
                {
                    var colorstr = "";
                    var platformstr = i.Platform switch
                    {
                        "META" => "M",
                        "STEAM" => "S",
                        "PC" => "P",
                        _ => " "
                    };
                    if (!string.IsNullOrEmpty(i.HexColor)) colorstr = $"<color={i.HexColor}>██</color>";
                    else colorstr = "";
                    players += @$"<size=55%>{i.Name} {platformstr}  {colorstr}
<size=45%><color=#666666>{i.PlayFabId}</color> </size></size>
";
                }
                for (int i = 0; i < 10 - code.Players.Count; i++)
                {
                    players += "\n\n";
                }
                return @$"<size=80%><b>{nazwakodu}</b></size>
<size=70%> {gmclr} {code.Players.Count}/10</size> 
{players}
"; ;
            }
            catch (Exception ex)
            {
                return $"error+ {ex.Message} +and {ex.StackTrace}";
            }
        }

    }
}