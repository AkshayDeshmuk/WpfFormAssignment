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
using System.Windows.Shapes;
using WpfApp1.model;
using WpfApp1.Service;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Form.xaml
    /// </summary>
    public partial class Form : Window
    {

        UserDbService _userDbService;
        

        public Form()
        {
            InitializeComponent();
        }

        public void Start()
        {
            _userDbService = new UserDbService();
            DataContext = _userDbService;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void ButtonInsert_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.Insert();
            TextBoxUsername.Focus();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.Update();
        }

        private void ButtonFirst_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.First();
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.Previous();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.Next();
        }

        private void ButtonLast_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.Last();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.Save();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            _userDbService.StopEditing();
        }
    }
}
