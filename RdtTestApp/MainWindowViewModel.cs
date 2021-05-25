using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RandomDataToolsInterop;
using RandomDataToolsInterop.Models;
using RdtTestApp.Annotations;

namespace RdtTestApp
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _progressValue;
        public ObservableCollection<Person> People { get; } = new();
        
        public MainWindowViewModel()
        {
            LoadData();
        }

        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        private async void LoadData()
        {
            var syncContext = SynchronizationContext.Current;

            ProgressValue = 0;
            for (var i = 0; i < 10; i++)
            {
                var people = await Api.GetPeopleAsync(100);
                await Task.Run(() =>
                {
                    people.ForEach(person =>
                    {
                        syncContext.Send(_ =>
                        {
                            People.Add(person);
                            ProgressValue++;
                        }, null);
                    });
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}