using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System;
using System.Windows;
using System.Windows.Input;
using System.IO;

using System.Security.Cryptography;


namespace Nice_Safe
{
    struct NiceSafeData
    {
        public string tag      { get; set; }
        public string email    { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public partial class MainWindow : Window
    {
        private string vaultPath = @"c:\temp\nsf\";

        private string[] fileEntries;

        private NiceSafeData nsf = new NiceSafeData();
        private List<NiceSafeData> entriesList = new List<NiceSafeData>();

        private string tag_selected;
        private int entryId;

        LoginWindow loginWindow = new LoginWindow();

        public MainWindow()
        {
            InitializeComponent();

            //.. Preventing Program running in Background after closing
            this.Closed += (sender, e) => this.Dispatcher.InvokeShutdown();


            //.. Hide main & open login page
            this.Hide();
            loginWindow.SetMainWindow(this);
            loginWindow.Show();


            //.. Doesn't exist, Create the path 
            if (!File.Exists(vaultPath)) 
            {
                Directory.CreateDirectory(vaultPath);
            }

            //.. Already exist, Load all files from the path
            LoadEntryData();

            //.. Open Entries Form as Default Page
            OpenEntriesForm();
        }

        private void EntriesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenEntriesForm();

            //.. Clear Selected Data after Search 
            SelectedView.Items.Clear();

            UnselectEntry();
            Edit_Del_Buttons_Disable();
        }

        private void NewEntryButton_Click(object sender, RoutedEventArgs e)
        {
            OpenNewEntryForm();

            //.. Clear Selected Data after Search 
            SelectedView.Items.Clear();

            UnselectEntry();
            Edit_Del_Buttons_Disable();
        }

        private void NoteButton_Click(object sender, RoutedEventArgs e)
        {
            OpenNoteForm();

            //.. Clear Selected Data after Search 
            SelectedView.Items.Clear();

            UnselectEntry();
            Edit_Del_Buttons_Disable();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            //.. Write to Text File
            string[] ms = { TagBox.Text, UsernameBox.Text, PasswordBox.Text, EmailBox.Text };
            string pathFormat = vaultPath + TagBox.Text;
            using (StreamWriter w = new StreamWriter(pathFormat, true))
            {
                foreach (var data in ms)
                {
                    //.. SAVE & ENCODE
                    w.WriteLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(data)));
                }
            }

            MessageBox.Show(TagBox.Text + " successfully submitted !");

            UsernameBox.Text = "";
            TagBox.Text      = "";
            EmailBox.Text    = "";
            PasswordBox.Text = "";


            //.. Add to Entry List
            string[] txt = File.ReadAllLines(pathFormat);

            //.. UNCODE
            string _tag      = Encoding.UTF8.GetString(Convert.FromBase64String(txt[0]));
            string _username = Encoding.UTF8.GetString(Convert.FromBase64String(txt[1]));
            string _password = Encoding.UTF8.GetString(Convert.FromBase64String(txt[2]));
            string _email    = Encoding.UTF8.GetString(Convert.FromBase64String(txt[3]));

            nsf.tag      = _tag;
            nsf.username = _username;
            nsf.password = _password;
            nsf.email    = _email;
            entriesList. Add(nsf);
            ListViewRecord.Items.Add(entriesList.Last());
        }

        //.. Press Enter to fire Search Event
        private void SearchBox_EnterKeyDown(object sender, KeyEventArgs e)
        {
            string searchPath = vaultPath + SearchBox.Text;

            if (e.Key == Key.Enter)
            {
                //.. Unselect previous entry
                UnselectEntry();
                Edit_Del_Buttons_Disable  ();


                if (File.Exists(searchPath))
                {
                    //.. Clear Old Value, So it will not spam the same result again
                    SelectedView.Items.Clear();

                    foreach (var i in entriesList)
                    {
                        if ((vaultPath.ToUpper() + i.tag.ToUpper()) == searchPath.ToUpper())
                        {
                            for (int x = 0; x < entriesList.Count; ++x)
                            {
                                if (entriesList[x].tag.ToUpper() == i.tag.ToUpper())
                                {
                                    //.. Get Entry Row Number In ListView 
                                    entryId = x;

                                    SelectedView.Items.Add(entriesList[x]);

                                    EntriesButton.IsChecked  = false;
                                    NewEntryButton.IsChecked = false;
                                    NoteButton.IsChecked     = false;

                                    OpenSelectViewForm();
                                }
                            }
                        }
                    }
                }
                else
                {
                    string ms = String.Format(@"''{0}'' Not Found", SearchBox.Text);
                    MessageBox.Show(ms);
                }
            }
        }

        private void ProfileImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UnselectEntry();
            Edit_Del_Buttons_Disable  ();

            MessageBox.Show("Not Available !");
        }

        private void CloudImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UnselectEntry();
            Edit_Del_Buttons_Disable  ();

            MessageBox.Show("Not Available !");
        }

        private void SettingsImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UnselectEntry();
            Edit_Del_Buttons_Disable  ();

            MessageBox.Show("Not Available !");
        }

        //.. Get info of selected entry (In entries form)
        private void ListViewRecord_HandleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext;

            if (item != null)
            {
                int rowNumber = ListViewRecord.SelectedIndex;
                for (int i = 0; i < entriesList.Count; ++i)
                {
                    if (i == rowNumber)
                    {
                        Edit_Del_Buttons_Enable();

                        //.. Entry Selecting
                        tag_selected = entriesList[i].tag;
                    }
                }
            }
            else
            {
                ListViewRecord.UnselectAll();
                Edit_Del_Buttons_Disable  ();
            }
        }

        //.. Get info of selected entry (In search form)
        private void SelectedView_HandleClick(object sender, MouseButtonEventArgs e)
        {
            var item = ((FrameworkElement)e.OriginalSource).DataContext;

            if (item != null)
            {
                int rowNumber = SelectedView.SelectedIndex;
                for (int i = 0; i <= SelectedView.Items.Count; ++i)
                {
                    if (i == rowNumber)
                    {
                        foreach (var x in entriesList)
                        {
                            if (SearchBox.Text.ToUpper() == x.tag.ToUpper())
                            {
                                Edit_Del_Buttons_Enable();

                                //.. Entry Selecting
                                tag_selected = x.tag;
                            }
                        }
                    }
                }
            }
            else
            {
                SelectedView.UnselectAll();
                Edit_Del_Buttons_Disable();
            }
        }

        //.. Unselection all entry
        private void Canvas_HandleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewRecord.IsVisible)
            {
                if (!ListViewRecord.IsMouseOver)
                {
                    ListViewRecord.UnselectAll();
                    Edit_Del_Buttons_Disable  ();
                }
            }

            if (SelectedView.IsVisible)
            {
                if (!SelectedView.IsMouseOver)
                {
                    SelectedView.UnselectAll();
                    Edit_Del_Buttons_Disable();
                }
            }
        }

        private void SearchBox_HandleMouseEnter(object sender, MouseEventArgs e)
        {
            if (ListViewRecord.IsVisible)
            {
                if (!ListViewRecord.IsMouseOver)
                {
                    ListViewRecord.UnselectAll();
                    Edit_Del_Buttons_Disable  ();
                }
            }

            if (SelectedView.IsVisible)
            {
                if (!SelectedView.IsMouseOver)
                {
                    SelectedView.UnselectAll();
                    Edit_Del_Buttons_Disable();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var i in entriesList)
            {
                if (i.tag == tag_selected)
                {
                    for (int x = 0; x < entriesList.Count; ++x)
                    {
                        if (tag_selected == entriesList[x].tag)
                        {
                            if (ListViewRecord.IsVisible)
                            {
                                EditTagBox.Text      = entriesList[x].tag;
                                EditUsernameBox.Text = entriesList[x].username;
                                EditPasswordBox.Text = entriesList[x].password;
                                EditEmailBox.Text    = entriesList[x].email;

                                OpenEditForm();
                            }

                            if (SelectedView.IsVisible)
                            {
                                EditTagBox.Text      = entriesList[x].tag;
                                EditUsernameBox.Text = entriesList[x].username;
                                EditPasswordBox.Text = entriesList[x].password;
                                EditEmailBox.Text    = entriesList[x].email;

                                OpenEditForm();
                            }
                        }
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //.. Change Edit Image Filter Background Back to Default
            BrushConverter bgc = new BrushConverter();
            EditButton.Background = (Brush)bgc.ConvertFrom("#353340");

            string ms = String.Format("        Are you sure to delete: {0}?", tag_selected);
            MessageBoxResult result = MessageBox.Show(ms, "", MessageBoxButton.YesNo);


            if (result == MessageBoxResult.Yes)
            {
                foreach (var i in entriesList)
                {
                    if (i.tag == tag_selected)
                    {
                        for (int x = 0; x < entriesList.Count; ++x)
                        {
                            if (tag_selected == entriesList[x].tag)
                            {
                                //.. Remove entry from List View
                                if (ListViewRecord.IsVisible)
                                {
                                    ListViewRecord.Items.RemoveAt(x);

                                    ListViewRecord.UnselectAll();
                                    Edit_Del_Buttons_Disable  ();
                                }

                                if (SelectedView.IsVisible)
                                {
                                    ListViewRecord.Items.RemoveAt(entryId);

                                    OpenEntriesForm();

                                    ListViewRecord.UnselectAll();
                                    Edit_Del_Buttons_Disable  ();

                                }

                                //.. Delete file
                                File.Delete(vaultPath + tag_selected);
                            }
                        }
                    }
                }
            }

            if (result == MessageBoxResult.No)
            {
                UnselectEntry();
                Edit_Del_Buttons_Disable();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            //.. Open and get access to all text lines
            string[] txt = File.ReadAllLines((vaultPath + tag_selected));

            //.. Replace old values with new encoded data from text box
            txt[0] = Convert.ToBase64String(Encoding.UTF8.GetBytes(EditTagBox.Text));
            txt[1] = Convert.ToBase64String(Encoding.UTF8.GetBytes(EditUsernameBox.Text));
            txt[2] = Convert.ToBase64String(Encoding.UTF8.GetBytes(EditPasswordBox.Text));
            txt[3] = Convert.ToBase64String(Encoding.UTF8.GetBytes(EditEmailBox.Text));


            //.. Save changed back to the file
            File.WriteAllLines((vaultPath + tag_selected), txt);

            //.. If the tag changed also change the file name 
            File.Move((vaultPath + tag_selected), (vaultPath + EditTagBox.Text));

            UpdateData();

            OpenEntriesForm();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OpenEntriesForm();
            UnselectEntry  ();
            Edit_Del_Buttons_Disable();
        }


        private void LoadEntryData()
        {
            fileEntries = Directory.GetFiles(vaultPath);
            if (fileEntries != null)
            {
                foreach (string fileName in fileEntries)
                {
                    string[] text = File.ReadAllLines(fileName);

                    string _tag      = Encoding.UTF8.GetString(Convert.FromBase64String(text[0]));
                    string _username = Encoding.UTF8.GetString(Convert.FromBase64String(text[1]));
                    string _password = Encoding.UTF8.GetString(Convert.FromBase64String(text[2]));
                    string _email    = Encoding.UTF8.GetString(Convert.FromBase64String(text[3]));

                    nsf.tag      = _tag;
                    nsf.username = _username;
                    nsf.password = _password;
                    nsf.email    = _email;
                    entriesList.Add(nsf);
                }

                //.. Add To List Table
                foreach (var i in entriesList)
                {
                    ListViewRecord.Items.Add(i);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }


        private void OpenEntriesForm()
        {
            //.. Open Entries Form (ListView)

            ListViewForm.Visibility     = Visibility.Visible;

            NoteForm.Visibility         = Visibility. Hidden;
            NewEntryForm.Visibility     = Visibility. Hidden;
            SelectedViewForm.Visibility = Visibility. Hidden;
            EditForm.Visibility         = Visibility. Hidden;

            EntriesButton.IsChecked  =  true;
            NewEntryButton.IsChecked = false;
            NoteButton.IsChecked     = false;

            //.. Change Edit Image Filter Background Back to Default
            BrushConverter bgc = new BrushConverter();
            EditButtonFilter.Background = (Brush)bgc.ConvertFrom("#353340");
        }

        private void OpenSelectViewForm()
        {
            //.. Open Entries Form (ListView In Search)

            SelectedViewForm.Visibility = Visibility.Visible;

            NewEntryForm.Visibility     = Visibility. Hidden;
            NoteForm.Visibility         = Visibility. Hidden;
            ListViewForm.Visibility     = Visibility. Hidden;
            EditForm.Visibility         = Visibility. Hidden;

            EntriesButton.IsChecked  = false;
            NewEntryButton.IsChecked = false;
            NoteButton.IsChecked     = false;

            //.. Change Edit Image Filter Background Back to Default
            BrushConverter bgc = new BrushConverter();
            EditButtonFilter.Background = (Brush)bgc.ConvertFrom("#353340");
        }

        private void OpenNewEntryForm()
        {
            NewEntryForm.Visibility     = Visibility.Visible;

            ListViewForm.Visibility     = Visibility. Hidden;
            NoteForm.Visibility         = Visibility. Hidden;
            SelectedViewForm.Visibility = Visibility. Hidden;
            EditForm.Visibility         = Visibility. Hidden;

            NewEntryButton.IsChecked =  true;
            NoteButton.IsChecked     = false;
            EntriesButton.IsChecked  = false;

            //.. Change Edit Image Filter Background Back to Default
            BrushConverter bgc = new BrushConverter();
            EditButtonFilter.Background = (Brush)bgc.ConvertFrom("#353340");

        }

        private void OpenNoteForm()
        {
            NoteForm.Visibility         = Visibility.Visible;

            ListViewForm.Visibility     = Visibility. Hidden;
            NewEntryForm.Visibility     = Visibility. Hidden;
            SelectedViewForm.Visibility = Visibility. Hidden;
            EditForm.Visibility         = Visibility. Hidden;

            NoteButton.IsChecked     =  true;
            NewEntryButton.IsChecked = false;
            EntriesButton.IsChecked  = false;

            //.. Change Edit Image Filter Background Back to Default
            BrushConverter bgc = new BrushConverter();
            EditButtonFilter.Background = (Brush)bgc.ConvertFrom("#353340");
        }

        private void OpenEditForm()
        {
            EditForm.Visibility = Visibility.Visible;

            NoteForm.Visibility         = Visibility.Hidden;
            ListViewForm.Visibility     = Visibility.Hidden;
            NewEntryForm.Visibility     = Visibility.Hidden;
            SelectedViewForm.Visibility = Visibility.Hidden;

            NoteButton.IsChecked     = false;
            NewEntryButton.IsChecked = false;
            EntriesButton.IsChecked  = false;


            Edit_Del_Buttons_Disable();

            //.. Highlight Edit Image Filter
            BrushConverter bgc = new BrushConverter();
            EditButtonFilter.Background = (Brush)bgc.ConvertFrom("#85BDBF");
        }

        private void Edit_Del_Buttons_Enable()
        {
            BrushConverter ebc = new BrushConverter();
            EditButton.Background = (Brush)ebc.ConvertFrom("#85BDBF");
            EditImage.Opacity = 1;

            BrushConverter dbc = new BrushConverter();
            DeleteButton.Background = (Brush)dbc.ConvertFrom("#85BDBF");
            DeleteImage.Opacity = 1;

            EditButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;

            EditButtonFilter.Visibility = Visibility.Hidden;
            DeleteButtonFilter.Visibility = Visibility.Hidden;
        }

        private void Edit_Del_Buttons_Disable()
        {
            //.. Disable buttons and Turn on blocking filter
            EditButton.Visibility = Visibility.Hidden;
            DeleteButton.Visibility = Visibility.Hidden;

            EditButtonFilter.Visibility = Visibility.Visible;
            DeleteButtonFilter.Visibility = Visibility.Visible;
        }

        private void UnselectEntry()
        {
            ListViewRecord.UnselectAll();
            SelectedView  .UnselectAll();
        }

        private void UpdateData()
        {
            //.. Clear previous items in listview and in entry list 
            ListViewRecord.Items.Clear();
            entriesList.Clear();

            //.. Load all file, to update all information
            LoadEntryData();

            UnselectEntry();
            Edit_Del_Buttons_Disable();
        }
    }
}
