using System.ComponentModel;
using BDShared.Util.Attributes;

namespace BDShared.GUI.ViewModels
{
    [Developer("Johannes Jacobs")]
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}