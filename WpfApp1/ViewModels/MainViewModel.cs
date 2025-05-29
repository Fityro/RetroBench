using Aida64Clone.Models;
using Aida64Clone.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Aida64Clone.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TreeViewItemViewModel> MenuTree { get; set; }
        public CollectionViewSource TableView { get; set; }
        public ChartViewModel ChartViewModel { get; set; }
        public ObservableCollection<HelpItem> HelpItems { get; set; }
        public ICommand OpenHelpCommand { get; set; }
        public ICommand ExportDataCommand { get; set; }

        private bool _isMonitoring;
        public bool IsMonitoring
        {
            get => _isMonitoring;
            set { _isMonitoring = value; OnPropertyChanged(); }
        }

        private TreeViewItemViewModel _selectedNode;
        public TreeViewItemViewModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;
                UpdateTableView();
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            // Инициализация дерева
            MenuTree = new ObservableCollection<TreeViewItemViewModel>
            {
                new TreeViewItemViewModel("Hardware", new[]
                {
                    new TreeViewItemViewModel("CPU", new TreeViewItemViewModel[0], new List<DataPoint>
                    {
                        new DataPoint { Name = "CPU Usage", Status = "OK" },
                        new DataPoint { Name = "Temperature", Status = "OK" }
                    }),
                    new TreeViewItemViewModel("GPU", new TreeViewItemViewModel[0], new List<DataPoint>
                    {
                        new DataPoint { Name = "GPU Usage", Status = "OK" },
                        new DataPoint { Name = "Memory", Status = "OK" }
                    })
                }, new List<DataPoint>())
            };

            // Инициализация таблицы
            TableView = NewMethod();

            // Инициализация графика
            ChartViewModel = new ChartViewModel();

            // Инициализация команд
            OpenHelpCommand = new RelayCommand(OpenHelpWindow);
            ExportDataCommand = new RelayCommand(ExportData);
            StartMonitoring();

            // Справочная информация
            HelpItems = new ObservableCollection<HelpItem>
            {
                new HelpItem { Title = "Реальный мониторинг", Description = "Обновление данных каждую секунду" },
                new HelpItem { Title = "Экспорт данных", Description = "Сохранение в CSV" }
            };
        }

        private static CollectionViewSource NewMethod()
        {
            return new CollectionViewSource
            {
                Source = new ObservableCollection<DataPoint>()
            };
        }

        private async void StartMonitoring()
        {
            IsMonitoring = true;
            while (IsMonitoring)
            {
                await Task.Delay(1000); // Интервал 1 секунда

                foreach (var node in MenuTree)
                {
                    UpdateHardwareData(node);
                }
            }
        }

        private void UpdateHardwareData(TreeViewItemViewModel node)
        {
            if (node.Name == "CPU")
            {
                // Обновляем CPU Usage
                var cpuUsageItem = node.Data.FirstOrDefault(d => d.Name == "CPU Usage");
                if (cpuUsageItem != null)
                {
                    string value = HardwareMonitor.GetCpuUsage() + "%";
                    cpuUsageItem.Value = value;
                    cpuUsageItem.Status = int.TryParse(value.Replace("%", ""), out int usage) && usage > 90 ? "Error" : "OK";

                    // Обновляем график
                    if (int.TryParse(value.Replace("%", ""), out int cpuUsageValue))
                    {
                        ChartViewModel.UpdateChartData(cpuUsageValue);
                    }
                }

                // Обновляем температуру CPU
                var tempItem = node.Data.FirstOrDefault(d => d.Name == "Temperature");
                if (tempItem != null)
                {
                    string value = HardwareMonitor.GetCpuTemperature() + "°C";
                    tempItem.Value = value;
                    tempItem.Status = double.TryParse(value.Replace("°C", ""), out double temp) && temp > 80 ? "Warning" : "OK";
                }
            }

            // Рекурсивно обновляем дочерние узлы
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    UpdateHardwareData(child);
                }
            }
        }

        private void UpdateTableView()
        {
            if (SelectedNode != null)
            {
                var data = new ObservableCollection<DataPoint>(SelectedNode.Data);
                TableView.Source = data;
            }
            else
            {
                TableView.Source = new ObservableCollection<DataPoint>();
            }
        }

        private void ExportData(object obj)
        {
            var data = (ObservableCollection<DataPoint>)TableView.Source;
            var csv = "Название,Значение,Статус\n";
            foreach (var item in data)
                csv += $"{item.Name},{item.Value},{item.Status}\n";
            File.WriteAllText("report.csv", csv);
            MessageBox.Show("Экспортировано в report.csv");
        }

        private void OpenHelpWindow(object obj)
        {
            var helpWindow = new HelpWindow { DataContext = this };
            helpWindow.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StopMonitoring()
        {
            IsMonitoring = false;
        }
    }
}