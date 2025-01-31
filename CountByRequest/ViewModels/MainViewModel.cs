using ReactiveUI;

namespace CountByRequest.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _message = "Please, run a browser and go to http://localhost:8080";

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    //
    private bool _isButtonEnabled = false;
    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isButtonEnabled, value);
    }
}
