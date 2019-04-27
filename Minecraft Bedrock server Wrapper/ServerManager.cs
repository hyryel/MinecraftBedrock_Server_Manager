using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraft_Bedrock_server_Wrapper
{
    public enum MessageType
    {
        Error,
        Info,

    }
    internal class ServerUpdatedEventArgs : EventArgs
    {
        public MessageType MessageType { get; set; }
        public string Message { get; set; }
    }
    internal class DataReceivedInfo
    {
        public string Data { get; set; }
        public MessageType MsgType { get; set; }
    }
    internal class ServerManager : IDisposable
    {
        public string ExecutablePath { get; set; }

        private StreamWriter _inputWriter;
        private Process _server;
        private SynchronizationContext _context;
        private bool _isStarted = false;

        public ServerManager(string executablePath)
        {
            _context = SynchronizationContext.Current ?? new SynchronizationContext();
            ExecutablePath = executablePath;
        }

        public void Start()
        {
            if (_server == null)
            {

                _server = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo(ExecutablePath)
                {
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                _server.StartInfo = si;
                _server.ErrorDataReceived += _server_ErrorDataReceived;
                _server.OutputDataReceived += _server_OutputDataReceived;

            }
            _isStarted = _server.Start();
            _inputWriter = _server.StandardInput;
            _server.BeginOutputReadLine();
            _server.BeginErrorReadLine();
        }
        public void Stop()
        {
            if (_isStarted)
            {
                SendCommand("stop");
                System.Threading.Thread.Sleep(500);
                _server.CancelErrorRead();
                _server.CancelOutputRead();
                _server.Close();
                _isStarted = false;
            }
        }
        public event EventHandler<ServerUpdatedEventArgs> StateUpdated;

        private void MySendOrPostCallback(object state)
        {
            DataReceivedInfo data = (DataReceivedInfo)state;
            StateUpdated?.Invoke(this, new ServerUpdatedEventArgs { Message = data.Data, MessageType = data.MsgType });
        }

        private void _server_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            _context.Post(MySendOrPostCallback, new DataReceivedInfo { Data = e.Data, MsgType = MessageType.Error });
        }
        private void _server_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            _context.Post(MySendOrPostCallback, new DataReceivedInfo { Data = e.Data, MsgType = MessageType.Info });
        }

        public void SendCommand(string command)
        {
            _inputWriter.WriteLine(command);
        }
        ~ServerManager()
        {
            Dispose();
        }
        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Stop();
            }
        }
    }
}
