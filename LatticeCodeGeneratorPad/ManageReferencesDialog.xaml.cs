using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LatticeCodeGeneratorPad
{
    public partial class ManageReferencesDialog : Window
    {
        private string lastDirectory;
        private readonly FileInfoEqualityComparer fileInfoEqualityComparer = new FileInfoEqualityComparer();

        public ManageReferencesDialog() : this(null, null) { }
        public ManageReferencesDialog(string initialDirectory, IEnumerable<FileInfo> initialAssemblyFiles)
        {
            this.DataContext = this;
            this.lastDirectory = initialDirectory;
            this.customAssemblyFiles = new BindingList<FileInfo>((initialAssemblyFiles ?? Enumerable.Empty<FileInfo>()).ToList());
            InitializeComponent();
        }

        public string LastDirectory { get { return lastDirectory; } }

        public BindingList<FileInfo> AssemblyFiles
        {
            get { return customAssemblyFiles; }
        }
        private readonly BindingList<FileInfo> customAssemblyFiles;

        private void AddAssemblyFileButtonClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".dll";
            openFileDialog.Filter = "Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            if (lastDirectory != null)
            {
                openFileDialog.InitialDirectory = lastDirectory;
            }

            var showDialogResult = openFileDialog.ShowDialog();
            if (showDialogResult.HasValue && showDialogResult.Value && !string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                var fileNames = openFileDialog.FileNames;
                foreach (var fileName in fileNames)
                {
                    var file = new FileInfo(fileName);
                    lastDirectory = System.IO.Path.GetDirectoryName(file.FullName);
                    if (!customAssemblyFiles.Contains(file, fileInfoEqualityComparer))
                    {
                        customAssemblyFiles.Add(file);
                    }
                }
            }
        }

        private void RemoveSelectedButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedAssemblyFiles = AssemblyListView.SelectedItems.Cast<FileInfo>().ToList();
            foreach (var selectedAssemblyFile in selectedAssemblyFiles)
            {
                customAssemblyFiles.Remove(selectedAssemblyFile);
            }
        }

        private void OkButtonClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private class FileInfoEqualityComparer : IEqualityComparer<FileInfo>
        {
            public bool Equals(FileInfo x, FileInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;

                return x.FullName.Equals(y.FullName);
            }

            public int GetHashCode(FileInfo obj)
            {
                if (obj == null) return 0;
                return obj.FullName.GetHashCode();
            }
        }
    }
}
