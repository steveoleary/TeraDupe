using System;
using System.Collections.Generic;
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

namespace TeraDupe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }

        string _folderName = "c:\\dinoch";
        private void button1_Click(object sender, EventArgs e)
        {
            _folderName = (System.IO.Directory.Exists(_folderName)) ? _folderName : "";
            var dlg1 = new Ionic.Utils.FolderBrowserDialogEx
            {
                Description = "Select a folder for the extracted files:",
                ShowNewFolderButton = true,
                ShowEditBox = true,
                //NewStyle = false,
                SelectedPath = _folderName,
                ShowFullPathInEditBox = false,
            };
            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            var result = dlg1.ShowDialog();

            //if (result == DialogResult.)
            //{
                _folderName = dlg1.SelectedPath;
                //this.label1.Text = "The folder selected was: ";
                //this.label2.Text = _folderName;
           // }
        }
        

    }
}
