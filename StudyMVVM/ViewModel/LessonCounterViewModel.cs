using System.ComponentModel;
using System.Windows.Input;

namespace StudyMVVM.ViewModel
{
    // A real view model for the CustomControl/MVVM lesson: no view manipulation here.
    public sealed class LessonCounterViewModel : INotifyPropertyChanged
    {
        private int value;
        public LessonCounterViewModel()
        {
            IncrementCommand = new LabCommand(_ => Value++);
            ResetCommand = new LabCommand(_ => Value = 0);
        }
        public int Value
        {
            get => value;
            set { if (this.value == value) return; this.value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value")); }
        }
        public ICommand IncrementCommand { get; }
        public ICommand ResetCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
