using Resocentro_Desktop.Entitys;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Resocentro_Desktop.Interfaz.Chat
{
    /// <summary>
    /// Lógica de interacción para frmListChat.xaml
    /// </summary>
    public partial class frmListChat : UserControl
    {
        public MySession session { get; set; }
        public List<frmChat> Listachat { get; set; }
        public frmMenu menu { get; set; }

        public List<ClientChat> lstusuarios { get; set; }

        public frmListChat()
        {
            InitializeComponent();
            Listachat = new List<frmChat>();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void showChat(string userid, string nickname, string msj, bool pushmessage)
        {
            var user = lstusuarios.SingleOrDefault(x => x.id == userid);
            if (user != null)
            {
                var frm = Listachat.SingleOrDefault(x => x.Name == "frm" + userid);
                if (frm == null)
                {
                    frm = new frmChat();
                    frm.Name = "frm" + userid;
                    Listachat.Add(frm);
                }
                frm.cargarGUI(this, nickname);
                frm.Show();
                Application.Current.MainWindow = frm;
                this.session.helper.FlashApplicationWindow();
                if (pushmessage)
                    frm.pushMessge(nickname, msj);

            }
        }
        public void RefreshUser(List<ClientChat> lstusuarios)
        {
            this.lstusuarios = null;
            this.lstusuarios = lstusuarios;
            listChat.ItemsSource = null;
            listChat.ItemsSource = this.lstusuarios;

        }

        public void cargarGUI()
        {
            lblusuario.Content = session.shortuser;
            if (session.ispathBuddy)
            {
                System.IO.File.Copy(session.PathBuddy, (System.IO.Path.GetTempPath() + session.codigousuario + ".png"), true);
                setImagen((System.IO.Path.GetTempPath() + session.codigousuario + ".png"));
                imgbuddychat.Source = new BitmapImage(new Uri(System.IO.Path.GetTempPath() + session.codigousuario + ".png"));
            }
            else
                imgbuddychat.Source = new BitmapImage(new Uri("pack://application:,,,/Resocentro_Desktop;component" + session.PathBuddy));


        }
        private void setImagen(string file)
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(file))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream.SetLength(fileStream.Length);
                    fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = memoryStream;
                    imageSource.EndInit();
                    // Assign the Source property of your image
                    imgbuddychat.Source = imageSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void listChat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ClientChat item in e.AddedItems)
            {
                showChat(item.id, item.username, "",false);
            }
            //showChat()
        }
    }
}
