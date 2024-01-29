using BepInEx;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace bepinpython
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private Thread telnetThread;
        private TcpListener listener;

        public void StartTelnetServer()
        {
            telnetThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    // Create a TCP listener
                    listener = new TcpListener(IPAddress.Loopback, 10123);
                    listener.Start();
                    Logger.LogInfo("Listener started.");

                    // Accept incoming connections
                    while (true)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Logger.LogInfo("Client connected.");
                        NetworkStream stream = client.GetStream();
                        Logger.LogInfo("stream created");
                        StreamReader reader = new StreamReader(stream);
                        Logger.LogInfo("reader created");
                        StreamWriter writer = new StreamWriter(stream);
                        Logger.LogInfo("writer created");

                        // Start a new thread for each client

                        Logger.LogInfo("Starting new thread for client...");
                        HandleClient(client, reader, writer);
                        Logger.LogInfo("New thread for client started.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error in StartTelnetServer: " + ex.ToString());
                }
            }));

            telnetThread.Start();
        }

        private void HandleClient(TcpClient client, StreamReader reader, StreamWriter writer)
        {
            Logger.LogInfo("HandleClient started.");
            try
            {
                // Create a Python engine
                ScriptEngine engine = Python.CreateEngine();
                Logger.LogInfo("Python engine created.");

                // Create a Python scope
                ScriptScope scope = engine.CreateScope();

                // Set AutoFlush to true
                writer.AutoFlush = true;

                // Start the interactive session
                writer.WriteLine("Python " + engine.LanguageVersion);
                writer.Flush();
                while (true)
                {
                    writer.Write(">>> ");
                    writer.Flush();
                    string code = reader.ReadLine();
                    Logger.LogInfo("Received code: " + code);
                    if (string.IsNullOrEmpty(code) || !client.Connected)
                        break;

                    try
                    {
                        ScriptSource source = engine.CreateScriptSourceFromString(code, SourceCodeKind.AutoDetect);
                        var result = source.Execute(scope);
                        if (result != null)
                        {
                            writer.WriteLine(result.ToString());
                            writer.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine(ex.Message);
                        writer.Flush();
                        Logger.LogError("Error executing code: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in HandleClient: " + ex.ToString());
            }
            finally
            {
                client.Close();
                Logger.LogInfo("Client disconnected.");
            }
        }

        private void OnDestroy()
        {
            // Stop the TCP listener when the plugin is destroyed
            listener?.Stop();
            telnetThread?.Join();
            Logger.LogInfo("Listener stopped.");
        }
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            StartTelnetServer();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} Telnet server should be running?!");
        }
    }
}