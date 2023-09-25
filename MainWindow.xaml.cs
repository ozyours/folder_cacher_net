using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace folder_cacher_net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AsyncInitialize();
        }

        private async void AsyncInitialize()
        {
            await LoadConfig();

            _FolderEntry.List.Sort(new FolderEntryComparer());

            if (_FolderEntry.List.Count > 0)
                foreach (var _entry in _FolderEntry.List)
                {
                    var _folder_entry = new folder_entry(this);
                    _folder_entry.SetFolderEntry(_entry);
                    lsb_Folder_List.Items.Add(_folder_entry);
                }
        }

        public static readonly string CONFIG_FILE = "Config";

        private static FConfig _FolderEntry;

        public async Task LoadConfig()
        {
            if (File.Exists(CONFIG_FILE))
            {
                await Task.Run(() =>
                {
                    _FolderEntry = JsonConvert.DeserializeObject<FConfig>(File.ReadAllText(CONFIG_FILE));
                    if (_FolderEntry == null)
                        _FolderEntry = new FConfig();
                });
            }
            else
            {
                _FolderEntry = new FConfig();
            }
        }

        public async Task SaveConfig()
        {
            var _fconfig = new FConfig();

            foreach (var _entry in lsb_Folder_List.Items)
                _fconfig.List.Add(((folder_entry)_entry)._FolderEntry);

            _FolderEntry = _fconfig;

            var _write_string = JsonConvert.SerializeObject(_FolderEntry);

            if (!File.Exists(CONFIG_FILE))
                File.Create(CONFIG_FILE);

            StreamWriter _writer = new StreamWriter(CONFIG_FILE);
            await _writer.WriteAsync(_write_string);
            _writer.Close();
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            lsb_Folder_List.Items.Add(new folder_entry(this));
        }
    }
}