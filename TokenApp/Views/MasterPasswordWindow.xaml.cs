using System.Windows;

namespace TokenApp.Views;

public partial class MasterPasswordWindow : Window
{
    public string Password { get; private set; }

    public MasterPasswordWindow()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Password = PasswordBox.Password;
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}