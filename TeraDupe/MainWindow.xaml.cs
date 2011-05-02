using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Recls;

namespace TeraDupe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<PathToSearch> paths = new ObservableCollection<PathToSearch>();

        public MainWindow()
        {
            InitializeComponent();

            lbPathsToSearch.ItemsSource = paths;
        }

        string _folderName = "";

        public ObservableCollection<PathToSearch> Paths
        {
            get { return paths; }
            set { paths = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _folderName = (System.IO.Directory.Exists(_folderName)) ? _folderName : "";
            var dlg1 = new Ionic.Utils.FolderBrowserDialogEx
            {
                Description = "Select a folder for the extracted files:",
                ShowNewFolderButton = true,
                ShowEditBox = true,
                SelectedPath = _folderName,
                ShowFullPathInEditBox = false,
            };
            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            var result = dlg1.ShowDialog();
            paths.Add(new PathToSearch(dlg1.SelectedPath, true));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var entries = new List<IEntry>();

            foreach (PathToSearch item in lbPathsToSearch.Items)
            {
                entries.AddRange(FileSearcher.Search(item.SelectedPath, "*.avi").Select(file => file));
            }

            var fileGroups =
                from file in entries
                group file by new { file.Size, hash = GetHash(file) } into groupedFiles
                where groupedFiles.Count() > 1
                select new { ID = groupedFiles.Key, Values = groupedFiles };

            var duplicateFiles = (from fileGroup in fileGroups
                                  from file in fileGroup.Values
                                  select new DuplicateFile {hash = fileGroup.ID.hash, Size = file.Size, Path = file.Path}).ToList();

            var collection = new ListCollectionView(duplicateFiles);
            collection.GroupDescriptions.Add(new PropertyGroupDescription("hash"));

            dataGrid1.DataContext = collection;
        }

        private static ulong GetHash(IEntry file)
        {
            ulong hashFromBytes = 0ul;

            try
            {
                using (BinaryReader b = new BinaryReader(File.Open(file.Path, FileMode.Open)))
                {
                    long length = b.BaseStream.Length;
                    long pos = length / 2;
                    //TODO: If length is less than 2000 bytes read in entire file
                    b.BaseStream.Seek(pos, SeekOrigin.Begin);
                    hashFromBytes = GetHashFromBytes(b.ReadBytes(2000));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return hashFromBytes;
        }

        private static ulong GetHashFromBytes(byte[] b)
        {
            ulong result = 0;
            if ((b != null) || (b.Length != 0))
            {
                unchecked
                {
                    result = 0xcbf29ce484222325u; //init prime
                    for (int i = 0; i < b.Length; i++)
                    {
                        result = ((result ^ b[i]) * 0x100000001b3); //xor with new and mul with prime
                    }
                }
            }
            return result;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            foreach (var dataGridRow in GetDataGridRows(dataGrid1))
            {
                
            }
        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }

    public class PathToSearch
    {
        public string SelectedPath { get; set; }
        public bool SearchRecursively { get; set; }

        public PathToSearch(string selectedPath, bool searchRecursively)
        {
            SelectedPath = selectedPath;
            SearchRecursively = searchRecursively;
        }
    }

    public class DuplicateFile : DependencyObject
    {
        public ulong hash { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }

        public bool HasBeenMarkedForDeletion
        {
            get { return (bool)GetValue(HasBeenMarkedForDeletionProperty); }
            set { SetValue(HasBeenMarkedForDeletionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasBeenMarkedForDeletion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasBeenMarkedForDeletionProperty =
            DependencyProperty.Register("HasBeenMarkedForDeletion", typeof(bool), typeof(DuplicateFile));

        private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
