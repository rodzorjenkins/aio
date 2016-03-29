using System.Windows;
using System.Windows.Input;
using BDCustomLauncher.Views;
using BDShared.GUI.ViewModels;
using Microsoft.Win32;

namespace BDCustomLauncher.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {

        public ICommand buttonBrowseGame { get; set; }
        public ICommand buttonSave { get; set; }

        public string textGamePath
        {
            get
            {
                return Properties.Settings.Default.gamePath;
            }
            set
            {
                Properties.Settings.Default.gamePath = value;
                NotifyPropertyChanged("textGamePath");
            }
        }

        public SettingsViewModel()
        {
            buttonBrowseGame = new RelayCommand(buttonBrowseGame_Click);
            buttonSave = new RelayCommand(buttonSave_Click);
        }

        private void buttonSave_Click(object obj)
        {
            Properties.Settings.Default.Save();
            foreach(var window in Application.Current.Windows)
                if(window.GetType() == typeof(SettingsWindow))
                    (window as Window).Close();
        }

        private void buttonBrowseGame_Click(object obj)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Black Desert Executable|BlackDesert*.exe";
            ofd.Multiselect = false;
            if((bool)ofd.ShowDialog())
            {
                textGamePath = ofd.FileName;
            }
        }
    }
}