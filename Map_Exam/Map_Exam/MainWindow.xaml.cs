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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Map_Exam
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Doting
        {
            public int index { get; set; }
            public List<PointLatLng> Lpll { get; set; }
            PointLatLng pointFirst { get; set; }
            public GMapRoute gMapRoute { get; set; }

            GMapMarker _gMapMarker;
            public GMapMarker gMapMarker
            {
                get { return _gMapMarker; }
                set
                {
                    _gMapMarker = value;
                    _gMapMarker.Shape = new Ellipse() { Width = 10, Height = 10, Fill = Brushes.Red };
                }
            }
            public Doting()
            {
                gMapRoute = null;
                Lpll = new List<PointLatLng>();
                pointFirst = new PointLatLng();
            }
            public void SetDot(PointLatLng pointFirst)
            {
                this.pointFirst = pointFirst;
                gMapMarker = new GMapMarker(pointFirst);
            }
            public void AddDot()
            {
                if (index == Lpll.Count)
                    Lpll.Add(pointFirst);
                else
                    Lpll[index] = pointFirst;
            }
            public GMapRoute RoutAdd(RoutingProvider rp)
            {
                List<PointLatLng> ps = new List<PointLatLng>();
                for (int i = 0; i < Lpll.Count - 1; i++)
                {
                    MapRoute r = rp.GetRoute(Lpll[i], Lpll[i + 1], false, true, 13);
                    ps.AddRange(r.Points);
                }
                gMapRoute = new GMapRoute(ps);
                return gMapRoute;
            }
            public List<GMapMarker> GetAllMarker(bool b = true)
            {
                List<GMapMarker> Lgmm = new List<GMapMarker>();

                for (int i = 0; i < Lpll.Count; i++)
                {
                    gMapMarker = new GMapMarker(Lpll[i]);
                    if (b && i == index)
                        gMapMarker = new GMapMarker(pointFirst);
                    Lgmm.Add(gMapMarker);
                }
                return Lgmm;
            }
        }

        Doting dot = new Doting();
        public bool moderator { get; set; }
        int id_User { get; set; }
        int id_Roat { get; set; }
        OracleConnection oracleConn { get; set; }
        bool fullNameRegistr { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Height = 140;
            Width = 400;
            try
            {
                //тест 
                //oracleConn = new OracleConnection("Data Source=192.168.0.100;User Id=Sheun;Password=1111;");
                oracleConn = new OracleConnection("Data Source=192.168.1.108;User Id=malyh;Password=1111;");
                oracleConn.Open();
            }
            catch (Exception e)
            {
                oracleConn.Close();
                Close();
            }
        }
        public void LoginToMap(bool reg = false)
        {
            if (Login.Text != null && Password.Text != null)
                try
                {
                    OracleCommand com = new OracleCommand("SELECT get_user('" + Login.Text + "','" + Password.Text + "') FROM DUAL", oracleConn);
                    var res = com.ExecuteReader();
                    if (res.Read())
                    {
                        if (res[0].ToString() != "0")
                        {
                            id_User = int.Parse(res[0].ToString());
                            Height = 500;
                            Width = 600;
                            RegistrRow.Height = new GridLength(0, GridUnitType.Star);
                            MapRow.Height = new GridLength(1, GridUnitType.Star);

                            com = new OracleCommand("SELECT STATUS FROM USERS WHERE ID='" + id_User.ToString() + "' ", oracleConn);

                            var resid = com.ExecuteReader();
                            if (resid.Read())
                                if (res[0].ToString() == "admin")
                                    moderator = true;

                            if (!moderator)
                                ReportsRoat.Height = new GridLength(0, GridUnitType.Star);
                            else
                            {
                                com = new OracleCommand("SELECT ID FROM Reports WHERE Id_User_Moderator='" + id_User.ToString() + "'", oracleConn);
                                var res2 = com.ExecuteReader();
                                while (res2.Read())
                                    ComboModer.Items.Add(res2[0].ToString());
                            }
                            But_Filtr_Click(null, null);
                            DotRoat.Height = new GridLength(0);
                            RoatCommentOrReports.Height = new GridLength(0);
                            MenuRight.Width = new GridLength(0);
                        }

                        else
                        {
                            if (reg)
                                MessageBox.Show("Такой логин уже существует");
                            else
                                MessageBox.Show("Неправильный логин или пароль");
                        }
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
                FullNameRegistr.Height = new GridLength(1, GridUnitType.Star);
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
        private void gmap_Loaded(object sender, RoutedEventArgs e)
        {
            gmap.MapProvider = BingMapProvider.Instance;
            gmap.ShowCenter = false;
            gmap.SetPositionByKeywords("Paris, France");

        }
        private void gmap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListDot.SelectedIndex != -1)
            {
                Point p = e.GetPosition(gmap);
                //if (dot.changeDot)
                //    gmap.Markers.Remove(dot.gMapMarker);

                dot.SetDot(gmap.FromLocalToLatLng((int)p.X, (int)p.Y));

                gmap.Markers.Clear();

                foreach (var item in dot.GetAllMarker())
                    gmap.Markers.Add(item);

                GMapMarker gMapMarker = new GMapMarker(gmap.FromLocalToLatLng((int)p.X, (int)p.Y));
                gMapMarker.Shape = new Ellipse() { Width = 10, Height = 10, Fill = Brushes.Yellow };
                gmap.Markers.Add(gMapMarker);

                gmap.Markers.Add(dot.RoutAdd(gmap.MapProvider as RoutingProvider));

                DotSave.IsEnabled = true;
                Create_Map.IsEnabled = false;

                DotCoord.Text = e.GetPosition(gmap).ToString();

                /*int index = ListDot.SelectedIndex;
                ListDot.Items.Clear();
                foreach (var item in dot.Lpll)
                    ListDot.Items.Add(item.Lng.ToString()+" "+item.Lat.ToString());
                ListDot.Items.Add("new");
                ListDot.SelectedIndex = index;*/
            }
        }

        private void Create_Map_Click(object sender, RoutedEventArgs e)
        {
            dot = new Doting();
            ListDot.Items.Clear();
            ListDot.SelectedIndex = -1;
            MenuRight.Width = new GridLength(150);
            RoatName.Text = "";
            RoatDuration.Text = "";
            RoatDescript.Text = "";
            CheckDost.IsEnabled = false;

            OracleCommand com = new OracleCommand("SELECT FULL_NAME FROM USERS WHERE ID='" + id_User.ToString() + "'", oracleConn);

            var res = com.ExecuteReader();
            while (res.Read())
                UserName.Text = res[0].ToString();

        }

        bool b = false;
        bool b2 = false;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (b)
            {
                MenuLeft.Width = new GridLength(150);
                if(!b2)
                    MenuRight.Width = new GridLength(150);
                b2 = false;
            }
            else
            {
                MenuLeft.Width = new GridLength(0);
                if (MenuRight.Width.Value == 0)
                    b2 = true;
                MenuRight.Width = new GridLength(0);
            }
            b = !b;
        }
        private void ListRoat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListRoat.SelectedIndex != -1)
            {
                DotRoat.Height = new GridLength(140);
                RoatCommentOrReports.Height = new GridLength(120);
                MenuRight.Width = new GridLength(150);
                try
                {
                    OracleCommand com = null;
                    if (CheckDuration.IsChecked == true)
                        com = new OracleCommand("SELECT Waypoint.name,Coordinates,Waypoint.Description,Image FROM Waypoint INNER JOIN Roat ON Roat.ID=id_Roat WHERE Duration_Day='"
                            + ListRoat.SelectedItem.ToString() + "'", oracleConn);
                    else if (CheckCity.IsChecked == true)
                        com = new OracleCommand("SELECT Waypoint.name,Coordinates,Waypoint.Description,Image FROM Waypoint INNER JOIN Roat ON Roat.ID=id_Roat WHERE City='"
                            + ListRoat.SelectedItem.ToString() + "'", oracleConn);
                    else if (CheckCountry.IsChecked == true)
                        com = new OracleCommand("SELECT Waypoint.name,Coordinates,Waypoint.Description,Image FROM Waypoint INNER JOIN Roat ON Roat.ID=id_Roat WHERE Country='"
                            + ListRoat.SelectedItem.ToString() + "'", oracleConn);
                    else if (CheckType.IsChecked == true)
                        com = new OracleCommand("SELECT Waypoint.name,Coordinates,Waypoint.Description,Image FROM Waypoint INNER JOIN Roat ON Roat.ID=id_Roat WHERE Type='"
                            + ListRoat.SelectedItem.ToString() + "'", oracleConn);


                    var res = com.ExecuteReader();
                    ListDot.Items.Clear();
                    while (res.Read())
                        ListDot.Items.Add(res[0].ToString());
                    ListDot.Items.Add("new");
                    ListDot.SelectedIndex = ListDot.Items.Count - 1;

                    com = new OracleCommand("SELECT name,Country,City,Duration_Day,type,Description,Full_Name,ID FROM roat INNER JOIN USERS ON users.ID=ID_User"
                        , oracleConn);

                    var res2 = com.ExecuteReader();
                    while (res.Read())
                    {
                        RoatName.Text = res[0].ToString();
                        RoatContry.Text = res[1].ToString();
                        RoatCity.Text = res[2].ToString();
                        RoatDuration.Text = res[3].ToString();
                        RoatDescript.Text = res[5].ToString();
                        UserName.Text = res[6].ToString();
                        id_Roat = int.Parse(res[7].ToString());
                    }

                    com = new OracleCommand("SELECT ID FROM Comments WHERE Id_Roat='" + id_Roat.ToString() + "'", oracleConn);
                    var res3 = com.ExecuteReader();
                    while (res3.Read())
                        ComboModer.Items.Add(res3[0].ToString());

                }
                catch { }
            }
            else
            {
                DotRoat.Height = new GridLength(0);
                RoatCommentOrReports.Height = new GridLength(0);
                MenuRight.Width = new GridLength(0);
            }
        }
        private void ListDot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListDot.SelectedIndex != -1)
            {
                if (ListDot.SelectedItem.ToString() == "new")
                {
                    DotName.Text = "";
                    DotDescript.Text = "";
                    DotCoord.Text = "";
                    DotImage.Text = "";
                }
                else
                    try
                    {
                        OracleCommand com = new OracleCommand("SELECT name,Coordinates,Description,Image FROM Waypoint INNER JOIN Roat ON Roat.ID=id_Roat WHERE Waypoint.Name='"
                            + ListDot.SelectedItem.ToString() + "'", oracleConn);

                        dot.SetDot(dot.Lpll[ListDot.SelectedIndex]);//

                        var res = com.ExecuteReader();
                        while (res.Read())
                        {
                            DotName.Text = res[0].ToString();
                            DotDescript.Text = res[2].ToString();
                            DotCoord.Text = res[1].ToString();
                            DotImage.Text = res[3].ToString();
                        }

                    }
                    catch { }
                dot.index = ListDot.SelectedIndex;
            }

        }

        private void But_Like_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand com = new OracleCommand("BEGIN LIKE() END", oracleConn);
            bool res = (bool)com.ExecuteScalar();

            if (res)
                But_Like.Content = "Дизлайк";
            else
                But_Like.Content = "Лайк";
        }

        private void But_Comment_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand com = new OracleCommand("INSERT INTO Comments(Description,Id_User,Id_Roat) VALUES('"
                    + Route_Descript.Text + "',1,2)", oracleConn);
            var res = com.ExecuteReader();
        }

        private void But_Complaite_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand com = new OracleCommand("INSERT INTO Reports(Description,Id_User,Id_Roat) VALUES('"
                    + Route_Descript.Text + "',1,2)", oracleConn);
            var res = com.ExecuteReader();
        }

        private void But_Filtr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand com = null;
                if (CheckDuration.IsChecked == true)
                    com = new OracleCommand("SELECT Name FROM roat INNER JOIN USERS ON users.ID=ID_User ORDER BY Duration_Day", oracleConn);
                else if (CheckCity.IsChecked == true)
                    com = new OracleCommand("SELECT City FROM roat INNER JOIN USERS ON users.ID=ID_User ORDER BY City", oracleConn);
                else if (CheckCountry.IsChecked == true)
                    com = new OracleCommand("SELECT Country FROM roat INNER JOIN USERS ON users.ID=ID_User ORDER BY Country", oracleConn);
                else if (CheckType.IsChecked == true)
                    com = new OracleCommand("SELECT Type FROM roat INNER JOIN USERS ON users.ID=ID_User ORDER BY Type", oracleConn);

                var res = com.ExecuteReader();

                ListRoat.Items.Clear();
                while (res.Read())
                    ListRoat.Items.Add(res[0].ToString());
            }
            catch { }
        }
        private void DotSave_Click(object sender, RoutedEventArgs e)
        {

            //OracleCommand com = new OracleCommand("SELECT Full_Name FROM USERS WHERE Status='1'", oracleConn);
            //var res = com.ExecuteReader();
            dot.AddDot();
            ListDot.Items.Clear();
            foreach (var item in dot.Lpll)
                ListDot.Items.Add(item.ToString());
            ListDot.Items.Add("new");
            ListDot.SelectedIndex = -1;

            DotSave.IsEnabled = false;
            Create_Map.IsEnabled = true;

            gmap.Markers.Clear();

            foreach (var item in dot.GetAllMarker())
                gmap.Markers.Add(item);

            gmap.Markers.Add(dot.RoutAdd(gmap.MapProvider as RoutingProvider));

        }

        private void Change_Route_Click(object sender, RoutedEventArgs e)
        {

            if (RoatCity.Text != null && RoatContry.Text != null && RoatName.Text != null && RoatDuration.Text != null && RoatDescript.Text != null && UserName.Text != null)
                try
                {
                    string idUser = null;
                    OracleCommand com = new OracleCommand("SELECT ID_User FROM roat INNER JOIN USERS ON users.ID=ID_User WHERE Full_Name='"
                        + UserName.Text + "'", oracleConn);
                    var resUser = com.ExecuteReader();
                    while (resUser.Read())
                        idUser = resUser[0].ToString();

                    com = new OracleCommand("INSERT INTO Roat(name,Country,City,Duration_Day,type,Description,id_user) VALUES('"
                    + RoatName.Text + "','" + RoatContry.Text + "','"+
                     RoatCity.Text + "','" + RoatDuration.Text + "','public','" + RoatDescript.Text + "','"+
                     idUser + "')", oracleConn);
                    var res = com.ExecuteReader();

                    ListDot.Items.Add("new");
                }
                catch { }
        }

        private void DotClear_Click(object sender, RoutedEventArgs e)
        {
            ListDot.Items.RemoveAt(ListDot.SelectedIndex);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand com = new OracleCommand("SELECT FULL_NAME FROM USERS WHERE STATUS='admin'", oracleConn);
                var res2 = com.ExecuteReader();
                while (res2.Read())
                    ReportsMessage.Items.Add(res2[0].ToString());
            }
            catch { }
            

        }
    }
}
