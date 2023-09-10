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

            //StreamFile();
            for (int i = 0; i < 8; i++)
            {
                var _folder_entry = new folder_entry();
                lsb_Folder_List.Items.Add(_folder_entry);
            }
        }

        public async void StreamFile()
        {
            var PATH = "D:\\Documents\\Gears\\SteamLibrary\\steamapps\\common\\Little Nightmares\\Atlas\\Content\\Paks\\Atlas-WindowsNoEditor.pak";

            var _file1 = new FileStream(PATH, FileMode.Open);

            var _info = new FileInfo(PATH);
            var _stream_end = _info.Length;

            await Task.Run(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    _file1.ReadByte();
                }
            });

            System.Environment.Exit(0);
        }
    }
}