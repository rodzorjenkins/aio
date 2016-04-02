using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BDShared.GUI.ViewModels;
using BDShared.Network.Cryptography;
using BDShared.Network.Model;
using BDShared.Util;
using BDShared.Util.Attributes;
using Microsoft.Win32;

namespace BDPacketViewer.ViewModels
{
    [Author("Johannes Jacobs")]
    internal class MainViewModel : ViewModelBase
    {

        public ICommand buttonLoadCryptoPacket { get; set; }
        public ICommand buttonLoadPacket { get; set; }
        public ICommand buttonExit { get; set; }

        private string _txtPacketView { get; set; }
        public string txtPacketView
        {
            get { return _txtPacketView; }
            set
            {
                _txtPacketView = value;
                NotifyPropertyChanged("txtPacketView");
            }
        }

        private BdoTransformer packetTransformer;
                
        public MainViewModel()
        {
            buttonLoadCryptoPacket = new RelayCommand(buttonLoadCryptoPacket_Click);
            buttonLoadPacket = new RelayCommand(buttonLoadPacket_Click);
            buttonExit = new RelayCommand(buttonExit_Click);
        }
        
        private void buttonLoadCryptoPacket_Click(object o)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All Files|*.*",
                Multiselect = false
            };
            var showDialog = openFileDialog.ShowDialog();
            if (showDialog == null || !(bool) showDialog)
                return;

            var packet = new BDPacket(File.ReadAllBytes(openFileDialog.FileName));
            packetTransformer = new BdoTransformer(packet.Buffer.Extract(5));
        }

        private void buttonLoadPacket_Click(object o)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All Files|*.*",
                Multiselect = false
            };
            var showDialog = openFileDialog.ShowDialog();
            if (showDialog == null || !(bool) showDialog)
                return;

            var packet = new BDPacket(File.ReadAllBytes(openFileDialog.FileName));
            var packetBody = packet.Buffer.Extract(2);

            if(packet.IsEncrypted)
            {
                if(packetTransformer != null)
                    packetTransformer.Transform(ref packetBody, true);
                else
                    MessageBox.Show("You need to specify the encryption packet first in order to decrypt encrypted packets.", "Encryption packet required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            FormatPacket(new BDPacket(packet.Buffer.Extract(0, 2).Concat(packetBody).ToArray()));
        }

        private void FormatPacket(BDPacket packet)
        {
            var b = new StringBuilder();

            b.AppendLine($"Size: {packet.Length}");
            b.AppendLine($"OpCode: 0x{packet.OpCode:X4}");
            b.AppendLine($"Encrypted: {packet.IsEncrypted}");
            if(packet.IsEncrypted) b.AppendLine($"SequenceId: {packet.SequenceId}");
            b.AppendLine();
            b.Append(packet.Buffer.FormatHex());
            txtPacketView = b.ToString();
        }

        private void buttonExit_Click(object o)
        {
            Application.Current.MainWindow.Close();
        }
    }
}