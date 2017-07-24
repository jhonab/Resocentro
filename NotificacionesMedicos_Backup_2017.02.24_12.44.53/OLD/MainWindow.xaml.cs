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


namespace NotificacionesMedicos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        public bool isEncuesta;
        public bool isSupervisor;
        public MainWindow()
        {
            InitializeComponent();
            
            
        }
       
        public bool Supervisor()
        {
            //this.isSupervisor = this.chkSupervicion.IsChecked.Value;
            return this.chkSupervicion.IsChecked.Value;
        }

    }
}
