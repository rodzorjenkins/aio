using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BDCustomLauncher.Network.Frames;
using BDCustomLauncher.Views;
using BDShared.GUI.ViewModels;
using BDShared.Util.Attributes;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BDCustomLauncher.ViewModels
{
    [Developer("Johannes Jacobs")]
    internal class MainViewModel : ViewModelBase
    {
        
        public ICommand windowClosing { get; set; }
        public ICommand buttonStartGame { get; set; }
        public ICommand buttonSettings { get; set; }
        public ICommand buttonExit { get; set; }

        private string _textEmail { get; set; }
        public string textEmail
        {
            get { return _textEmail; }
            set
            {
                _textEmail = value;
                NotifyPropertyChanged("textEmail");
            }
        }
    
        private bool _buttonStartGameEnabled { get; set; }
        public bool buttonStartGameEnabled
        {
            get { return _buttonStartGameEnabled; }
            set
            {
                _buttonStartGameEnabled = value;
                NotifyPropertyChanged("buttonStartGameEnabled");
            }
        }

        private bool _checkBoxEmailChecked { get; set; }
        public bool checkBoxEmailChecked
        {
            get { return _checkBoxEmailChecked; }
            set
            {
                _checkBoxEmailChecked = value;
                NotifyPropertyChanged("checkBoxEmailChecked");
            }
        }

        private bool _checkBoxPasswordChecked { get; set; }
        public bool checkBoxPasswordChecked
        {
            get { return _checkBoxPasswordChecked; }
            set
            {
                _checkBoxPasswordChecked = value;
                NotifyPropertyChanged("checkBoxPasswordChecked");
            }
        }

        private string gamePath { get; set; }

        public MainViewModel()
        {
            windowClosing = new RelayCommand(window_Closing);
            buttonStartGame = new RelayCommand(buttonStartGame_Click);
            buttonSettings = new RelayCommand(buttonSettings_Click);
            buttonExit = new RelayCommand(buttonExit_Click);
            buttonStartGameEnabled = true;

            checkBoxEmailChecked = !string.IsNullOrWhiteSpace(Properties.Settings.Default.email) ? true : false;
            if(checkBoxEmailChecked)
                textEmail = Properties.Settings.Default.email;
            gamePath = Properties.Settings.Default.gamePath;
        }

        private void buttonExit_Click(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        private void buttonSettings_Click(object obj)
        {
            new SettingsWindow().ShowDialog();
        }

        private void window_Closing(object obj)
        {
            Properties.Settings.Default.email = checkBoxEmailChecked ? textEmail : null;
            Properties.Settings.Default.gamePath = gamePath;
            Properties.Settings.Default.Save();
        }

        private async void buttonStartGame_Click(object o)
        {
            PasswordBox passwordBox = o as PasswordBox;

            buttonStartGameEnabled = false;

            if(!string.IsNullOrWhiteSpace(textEmail) && !string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                var loginFrame = await loginUser(textEmail, passwordBox.Password);
                if(loginFrame != null)
                {
                    if(loginFrame.result.token != null)
                    {
                        Process p = new Process();
                        if(File.Exists("BlackDesert64.exe"))
                            p.StartInfo.FileName = "BlackDesert64.exe";
                        else if(File.Exists("BlackDesert32.exe"))
                            p.StartInfo.FileName = "BlackDesert32.exe";
                        else
                        {
                            if(string.IsNullOrEmpty(gamePath))
                            {
                                var ofd = new OpenFileDialog();
                                ofd.Filter = "Black Desert 32-Bit|BlackDesert32.exe|BlackDesert 64-Bit|BlackDesert64.exe";
                                ofd.Multiselect = false;
                                if((bool)ofd.ShowDialog())
                                {
                                    gamePath = ofd.FileName;
                                    p.StartInfo.FileName = ofd.FileName;
                                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);
                                }
                            }
                            else
                            {
                                p.StartInfo.FileName = gamePath;
                                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);
                            }
                        }
                        p.StartInfo.Arguments = loginFrame.result.token;

                        if(!string.IsNullOrWhiteSpace(p.StartInfo.FileName))
                        {
                            p.Start();
                            Application.Current.MainWindow.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Unable to login to Black Desert Online account, please try again later.", "Login error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

            buttonStartGameEnabled = true;
        }

        private async Task<LoginFrame> loginUser(string email, string password)
        {
            try
            {
                var request = HttpWebRequest.Create("https://www.blackdesertonline.com/launcher/l/api/Login.json");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] data = Encoding.UTF8.GetBytes(string.Format("email={0}&password={1}", email, password));
                using(var reqStream = await request.GetRequestStreamAsync())
                {
                    await reqStream.WriteAsync(data, 0, data.Length);
                }
                using(var res = await request.GetResponseAsync())
                {
                    using(var resStream = res.GetResponseStream())
                    {
                        using(var reader = new StreamReader(resStream))
                        {
                            return await await Task.Factory.StartNew(async() => JsonConvert.DeserializeObject<LoginFrame>(await reader.ReadToEndAsync()));
                        }
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Unable to login to Black Desert Online account, please try again later.", "Login error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return null;
        }
    }
}