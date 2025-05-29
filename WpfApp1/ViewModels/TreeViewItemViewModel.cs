using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Aida64Clone.ViewModels
{
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        public TreeViewItemViewModel(string name, TreeViewItemViewModel[] children, List<Aida64Clone.Models.DataPoint> data)
        {
            Name = name;
            Children = new ObservableCollection<TreeViewItemViewModel>(children);
            Data = new ObservableCollection<Aida64Clone.Models.DataPoint>(data);
        }

        public string Name { get; set; }
        public ObservableCollection<TreeViewItemViewModel> Children { get; set; }
        public ObservableCollection<Aida64Clone.Models.DataPoint> Data { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}