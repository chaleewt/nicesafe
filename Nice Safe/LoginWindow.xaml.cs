using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nice_Safe
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private string subVaultPath = @"c:\temp\nsf\pws\";
        private string userfileName = "pss";

        private MainWindow mainWindow;

        public LoginWindow()
        {
            InitializeComponent();

            //.. Preventing Program running in Background after closing
            this.Closed += (sender, e) => this.Dispatcher.InvokeShutdown();

            SetLoginPageVisibility   (Visibility.Visible);
            SetCreationPageVisibility(Visibility.Hidden );

            //.. Doesn't exist, Create the path 
            if (!File.Exists(subVaultPath))
            {
                Directory.CreateDirectory(subVaultPath);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(subVaultPath + userfileName))
            {
                string[] txt = File.ReadAllLines(subVaultPath + userfileName);

                if (UserBox.Text == txt[0] && PassBox.Password == txt[1])
                {
                    this.Hide();
                    mainWindow.Show();
                }
                else
                {
                    MessageBox.Show("    Incorrect username or password !");
                }    
            }
            else
            {
                MessageBox.Show("    You don't have an account !");
            }
        }


        private void UserBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (File.Exists(subVaultPath + userfileName))
                {
                    string[] txt = File.ReadAllLines(subVaultPath + userfileName);

                    if (UserBox.Text == txt[0] && PassBox.Password == txt[1])
                    {
                        this.Hide();
                        mainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("    Incorrect username or password !");
                    }
                }
                else
                {
                    MessageBox.Show("    You don't have an account !");
                }
            }
        }

        private void PassBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (File.Exists(subVaultPath + userfileName))
                {
                    string[] txt = File.ReadAllLines(subVaultPath + userfileName);

                    if (UserBox.Text == txt[0] && PassBox.Password == txt[1])
                    {
                        this.Hide();
                        mainWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("    Incorrect username or password !");
                    }
                }
                else
                {
                    MessageBox.Show("    You don't have an account !");
                }
            }
        }

        public void SetMainWindow(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
        }

        //.. Open Create account Form
        private void CreateAccountButton_HandleClick(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(subVaultPath + userfileName))
            {
                MessageBox.Show("      You already have an account\n\n      Only one account is allowed!");
            }
            else
            {
                UserBox.Text = "";
                PassBox.Password = "";

                SetLoginPageVisibility   (Visibility.Hidden );
                SetCreationPageVisibility(Visibility.Visible);
            }
        }

        private void ResetPassButton_HandleClick(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(subVaultPath + userfileName))
            {
                string ms = String.Format("Your account will be deleted, but all your password files still exist !\n\nContinue ?");
                MessageBoxResult result = MessageBox.Show(ms, "Delete Account", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("         Deleted !");
                    File.Delete(subVaultPath + userfileName);
                }
            }
        }

        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PassBox.Password == "")
            {
                PassBlock.Visibility = Visibility.Visible;
            }
            else
            {
                PassBlock.Visibility = Visibility.Hidden;
            }
        }

        private void SetLoginPageVisibility(Visibility visibility)
        {
            UsernameBorder.Visibility            = visibility;
            PasswordBorder.Visibility            = visibility;
            LoginButtonBorder.Visibility         = visibility;
            Create_Reset_ButtonBorder.Visibility = visibility;
        }


        private void LoginButtonBorder_HandleHover(object sender, MouseEventArgs e)
        {
            BrushConverter dbc = new BrushConverter();
            LoginButtonBorder.Background = (Brush)dbc.ConvertFrom("#BAE8E3");
        }

        private void LoginButtonBorder_HandleStopHover(object sender, MouseEventArgs e)
        {
            BrushConverter dbc = new BrushConverter();
            LoginButtonBorder.Background = (Brush)dbc.ConvertFrom("#FFFFFF");
        }

        private void CreateAccountButton_HandleHover(object sender, MouseEventArgs e)
        {
            CreateAccountButton.Foreground = new SolidColorBrush(Colors.White);
        }

        private void CreateAccountButton_HandleStopHover(object sender, MouseEventArgs e)
        {
            CreateAccountButton.Foreground = new SolidColorBrush(Colors.DarkCyan);
        }

        private void ResetPassButton_HandleHover(object sender, MouseEventArgs e)
        {
            ResetPassButton.Foreground = new SolidColorBrush(Colors.White);
        }

        private void ResetPassButton_HandleStopHover(object sender, MouseEventArgs e)
        {
            ResetPassButton.Foreground = new SolidColorBrush(Colors.DarkCyan);
        }



        /// <summary>
        /// Account Creatation Section
        /// </summary>
        /// 

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(subVaultPath + userfileName))
            {
                //.. Write to Text File
                string[] ms = { NameInputBox.Text, PassInputBox.Text };
                string pathFormat = subVaultPath + userfileName;
                using (StreamWriter w = new StreamWriter(pathFormat, true))
                {
                    foreach (var data in ms)
                    {
                        w.WriteLine(data);
                    }
                }

                MessageBox.Show("     Account successfully created !");

                NameInputBox.Text = "";
                PassInputBox.Text = "";

                SetCreationPageVisibility(Visibility. Hidden);
                SetLoginPageVisibility   (Visibility.Visible);
            }
        }


        private void HaveAccountButton_HandleClick(object sender, MouseButtonEventArgs e)
        {
            NameInputBox.Text = "";
            PassInputBox.Text = "";

            SetCreationPageVisibility(Visibility. Hidden);
            SetLoginPageVisibility   (Visibility.Visible);
        }


        private void SetCreationPageVisibility(Visibility visibility)
        {
            CreateUsernameBorder.Visibility    = visibility;
            CreatePassBorder.Visibility        = visibility;
            CreateButtonBorder.Visibility      = visibility;
            HaveAccountButtonBorder.Visibility = visibility;
        }



        private void CreateButtonBorder_HandleHover(object sender, MouseEventArgs e)
        {
            BrushConverter dbc = new BrushConverter();
            CreateButtonBorder.Background = (Brush)dbc.ConvertFrom("#BAE8E3");
        }

        private void CreateButtonBorder_HandleStopHover(object sender, MouseEventArgs e)
        {
            BrushConverter dbc = new BrushConverter();
            CreateButtonBorder.Background = (Brush)dbc.ConvertFrom("#FFFFFF");
        }

        private void HaveAccountButton_HandleHover(object sender, MouseEventArgs e)
        {
            HaveAccountButton.Foreground = new SolidColorBrush(Colors.White);
        }

        private void HaveAccountButton_HandleStopHover(object sender, MouseEventArgs e)
        {
            HaveAccountButton.Foreground = new SolidColorBrush(Colors.DarkCyan);
        }
    }
}
