using System;
using System.IO;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Zordon.Services;
using Microsoft.VisualBasic;

namespace Zordon {
    class Program {
        private DiscordSocketClient _client;
        private readonly IConfiguration _config;
        public static string TempPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp"; //Path to Temp
        public static string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location; //Path to .exe directory

        static void Main(string[] args) {
            UpdateCheck();
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program() {
            //Create the configuration
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();
        }

        public static void UpdateCheck() {
            if (File.Exists(TempPath + @"\vsn.txt")) { File.Delete(TempPath + @"\vsn.txt"); }
            if (File.Exists(TempPath + @"\dt.txt")) { File.Delete(TempPath + @"\dt.txt"); }
#if DEBUG
            File.WriteAllText(apppath + @"..\..\..\..\..\version.txt", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString());
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                System.Net.WebClient Client = new System.Net.WebClient();
                Client.DownloadFile("https://raw.githubusercontent.com/PlasticJustice/Zordon/master/Zordon/version.txt", TempPath + @"\vsn.txt");
                string v;
                using (System.IO.StreamReader vReader = new System.IO.StreamReader(TempPath + @"\vsn.txt")) {v = vReader.ReadToEnd();}
                File.Delete(TempPath + @"\vsn.txt");
        }
#else
            string cv = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                try {
                    System.Net.WebClient Client = new System.Net.WebClient();
                    Client.DownloadFile("https://raw.githubusercontent.com/PlasticJustice/Zordon/master/Zordon/version.txt", TempPath + @"\vsn.txt");
                } catch {
                    File.WriteAllText(TempPath + @"\vsn.txt", " ");
                }
                string v;
                using (System.IO.StreamReader vReader = new System.IO.StreamReader(TempPath + @"\vsn.txt")) { v = vReader.ReadToEnd(); }
                File.Delete(TempPath + @"\dt.txt");
                if (cv != v) {
                    Console.WriteLine("New Version Available");
                    Console.ReadKey();
                    var ps = new System.Diagnostics.ProcessStartInfo("https://www.github.com/PlasticJustice/Zordon/releases/latest") {                
                        UseShellExecute = true,              
                        Verb = "open"
                    };
                    System.Diagnostics.Process.Start(ps);
                    Environment.Exit(0);
                }
        }
#endif
        }

        public async Task MainAsync() {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using (var services = ConfigureServices()) {
                // get the client and assign to client 
                // you get the services via GetRequiredService<T>
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                // setup logging and the ready event
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                //This is where we get the Token value from the configuration file
                await _client.LoginAsync(TokenType.Bot, _config["Token"]);
                await _client.StartAsync();

                // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                // Block the program until it is closed.
                await Task.Delay(-1);
            }
        }

    private Task LogAsync(LogMessage log) {
                Console.WriteLine(log.ToString());
                return Task.CompletedTask;
            }

            private Task ReadyAsync() {
                Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
                return Task.CompletedTask;
            }

            // this method handles the ServiceCollection creation/configuration, and builds out the service provider we can call on later
            private ServiceProvider ConfigureServices() {
                // this returns a ServiceProvider that is used later to call for those services
                // we can add types we have access to here, hence adding the new using statement:
                // using csharpi.Services;
                // the config we build is also added, which comes in handy for setting the command prefix!
                return new ServiceCollection()
                    .AddSingleton(_config)
                    .AddSingleton<DiscordSocketClient>()
                    .AddSingleton<CommandService>()
                    .AddSingleton<CommandHandler>()
                    .BuildServiceProvider();
            }
        }
    }