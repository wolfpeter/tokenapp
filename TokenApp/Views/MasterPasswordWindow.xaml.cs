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
        Password = PasswordBox.Password;  // Jelszó lekérése a PasswordBox-ból
        DialogResult = true;  // Zárd be az ablakot, jelezve, hogy az OK gombra kattintottak
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;  // Zárd be az ablakot, jelezve, hogy a Mégse gombra kattintottak
    }
}