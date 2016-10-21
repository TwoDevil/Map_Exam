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
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Map_Exam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OracleConnection oracleConn { get; set; }
        bool fullNameRegistr { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            oracleConn = new OracleConnection("Data Source=10.3.0.227;User Id=Sheun;Password=1111;");
            try
            {
                oracleConn.Open();
            }
            catch (Exception e)
            {
                oracleConn.Close();
                Close();
            }
        }
        public void LoginToMap()
        {
            try
            {
                OracleCommand com = new OracleCommand("BEGIN get_user('" + Login.Text + "','" + Password.Text + "') END", oracleConn);
                var res = com.ExecuteScalar();

                if ((int)res != 0)
                {
                    Map m = new Map();
                    m.Show();
                    Close();
                }
                else
                {
                }
            }
            catch { }
        }

        private void Button_Click_Login(object sender, RoutedEventArgs e)
        {
            LoginToMap();

        }

        private void Button_Click_Registr(object sender, RoutedEventArgs e)
        {
            if (!fullNameRegistr)
                FullNameRegistr.Height = new GridLength(1,GridUnitType.Star);
            else
                if (Login.Text != null && Password.Text != null && FullName.Text != null)
                    try
                    {
                        OracleCommand com = new OracleCommand("INSERT INTO Users(Full_Name,Login,Password,Status) VALUES('"
                            + FullName.Text + "','" + Login.Text + "','" + Password.Text + "','user')", oracleConn);
                        var res = com.ExecuteReader();
                        LoginToMap();
                    }
                    catch { }
            fullNameRegistr = true;
        }
    }
}
