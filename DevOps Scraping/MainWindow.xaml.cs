using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace DevOps_Scraping
{
    public partial class MainWindow : Window
    {
        private string search;
        private string checkedRadioButton;
        private string checkedCheckbox;
        private string browserPath;

        private bool csv;
        private bool json;
        private bool folder = false;

        public MainWindow()
        {
            InitializeComponent();

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folder = true;
                browserPath = dialog.SelectedPath;
                labelName.Content = browserPath;
            }
        }
        private async void ScrapeButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (System.Windows.Controls.CheckBox checkBox in checkBoxStackPanel.Children)
            {
                checkedCheckbox = checkBox.Content.ToString();
                switch (checkedCheckbox)
                {
                    case "csv":
                        csv = (bool)checkBox.IsChecked;
                        break;
                    case "json":
                        json = (bool)checkBox.IsChecked;
                        break;
                }
            }

            search = SearchInput.Text;
            if ( search.Length == 0 | (!csv && !json) | !folder)
            {

                var white = searchText.Foreground;

                if (search.Length == 0) searchText.Foreground = System.Windows.Media.Brushes.Red;
                if (!csv) csvBox.Foreground = System.Windows.Media.Brushes.Red;
                if (!json) jsonBox.Foreground = System.Windows.Media.Brushes.Red;
                if (!folder) labelName.Foreground = System.Windows.Media.Brushes.Red;

                await Task.Delay(1000);
                searchText.Foreground = white;
                csvBox.Foreground = white;
                jsonBox.Foreground = white;
                labelName.Foreground = white;
                return;
            }

            foreach (System.Windows.Controls.RadioButton radioButton in radioButtonStackPanel.Children)
            {
                if (radioButton.IsChecked == true) checkedRadioButton = radioButton.Content.ToString();

            }

            switch (checkedRadioButton)
            {
                case "YouTube":
                    Scraping.Youtube(browserPath, search, csv,json);
                    break;
                case "ICT Job":
                    Scraping.JobSite(browserPath, search, csv, json);
                    break;
                case "Ebay":
                    Scraping.Ebay(browserPath, search, csv, json);
                    break;
            }
        }
    }
}