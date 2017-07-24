using Resocentro_Desktop.DAO;
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
using System.Windows.Shapes;

namespace Resocentro_Desktop.Interfaz.Chat
{
    /// <summary>
    /// Lógica de interacción para frmChat.xaml
    /// </summary>
    public partial class frmChat : Window
    {
        public frmListChat main { get; set; }
        public frmChat()
        {
            InitializeComponent();
        }

        

        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            sendmessage();
        }

        private async void sendmessage()
        {
            try
            {
                pushMessge(main.menu.userChat.username, txtnewmensaje.Text);
                await main.menu.Proxy.Invoke("send", this.Name.Replace("frm", ""), txtnewmensaje.Text, main.menu.userChat.id);
                txtnewmensaje.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        public void pushMessge(string nickname, string msj)
        {

            txtmensajes.Inlines.Add(new Run(nickname + " ") { FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.Aquamarine) });
            txtmensajes.Inlines.Add(new Run("(" + DateTime.Now.ToString("HH:mm") + ") \n") { FontStyle = FontStyles.Italic, Foreground = new SolidColorBrush(Colors.Gray) });
            txtmensajes.Inlines.Add(msj + "\n\n");
            

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           main.Listachat.Remove(this);
        }

        public void cargarGUI(frmListChat frmListChat,string nickname)
        {
            this.main = frmListChat;
            lbltitulochat.Content = nickname;
            this.Title = "Chat - " + nickname;
            try
            {
                System.IO.File.Copy(Tool.PathImgBuddy + this.Name.Replace("frm", "") + ".png", (System.IO.Path.GetTempPath() +  this.Name.Replace("frm", "") + ".png"), true);
                setImagen((System.IO.Path.GetTempPath() + this.Name.Replace("frm", "") + ".png"));
            }
            catch (Exception)
            {

            }
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
                    imgbuddy.Source = imageSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void txtnewmensaje_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendmessage();
            }
        }

        
    }
}
