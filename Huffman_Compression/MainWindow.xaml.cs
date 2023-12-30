using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FileCompression
{
    public partial class MainWindow : Window
    {
        private List<string> selectedFiles = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Select Files"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file paths
                selectedFiles.AddRange(openFileDialog.FileNames);

                // Display selected files in the ListBox
                DisplaySelectedFiles(selectedFiles);
            }
        }

        private void DisplaySelectedFiles(List<string> files)
        {
            // Clear existing items
            fileListBox.Items.Clear();

            // Display each selected file in the ListBox
            foreach (string file in files)
            {
                fileListBox.Items.Add(file);
            }
        }

        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Please select at least one file to compress.", "No Files Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                foreach (string filePath in selectedFiles)
                {
                    CompressFile(filePath);
                }

                MessageBox.Show("Compression completed successfully.", "Compression Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Compression error: {ex.Message}", "Compression Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompressFile(string filePath)
        {
            string compressedFilePath = $"{System.IO.Path.GetDirectoryName(filePath)}\\{System.IO.Path.GetFileNameWithoutExtension(filePath)}_compressed.huff";

            byte[] input = File.ReadAllBytes(filePath);
            byte[] compressed = HuffmanCompression.Compress(input);

            File.WriteAllBytes(compressedFilePath, compressed);
        }
    }
}
