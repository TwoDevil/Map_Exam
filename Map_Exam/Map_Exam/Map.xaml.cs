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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;


namespace Map_Exam
{
    /// <summary>
    /// Логика взаимодействия для Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        public class Doting 
        {
            public bool changeDot { get; set; }
            public int index { get; set; }
            public List<PointLatLng> Lpll { get; set; }
            PointLatLng pointFirst { get; set; }
            public GMapRoute gMapRoute { get; set; }

            GMapMarker _gMapMarker;
            public GMapMarker gMapMarker
            {
                get { return _gMapMarker; }
                set { _gMapMarker = value;
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
                Lpll[index]= pointFirst;
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
            
        }

        Doting dot = new Doting();
        OracleConnection oracleConn { get; set; }
        bool createMap { get; set; }
        bool createWarpPoint { get; set; }
        public Map()
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

        private void gmap_Loaded(object sender, RoutedEventArgs e)
        {
            gmap.MapProvider = BingMapProvider.Instance;
            gmap.ShowCenter = false;
            gmap.SetPositionByKeywords("Paris, France");

        }

        private void gmap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (createWarpPoint)
            {
                Point p = e.GetPosition(gmap);
                if (dot.changeDot)
                    gmap.Markers.Remove(dot.gMapMarker);

                dot.SetDot(gmap.FromLocalToLatLng((int)p.X, (int)p.Y));        
                gmap.Markers.Add(dot.gMapMarker);

                DotSave.IsEnabled = true;
                Create_Map.IsEnabled = false;

                DotCoord.Text = e.GetPosition(gmap).ToString();

                ListDot.Items.Clear();
                foreach (var item in dot.Lpll)
                    ListDot.Items.Add(item.Lng.ToString()+" "+item.Lat.ToString());
                ListDot.Items.Add("ok");
            }
            createWarpPoint = false;
        }

        private void Create_Map_Click(object sender, RoutedEventArgs e)
        {
            if (createWarpPoint)
            {
                //gmap.Markers.Remove(dot.gMapRoute);
                gmap.Markers.Add(dot.RoutAdd(gmap.MapProvider as RoutingProvider));

                Create_Map.Content = "Создать машрут";
                DotSave.IsEnabled = false;
                createWarpPoint = false;
            }
            else
            {
                dot = new Doting();
                Create_Map.Content = "Закончить машрут";
                //DotSave.IsEnabled = true;
                createWarpPoint = true;
                ListDot.Items.Clear();
                ListDot.Items.Add("ok");
            }
            createMap = !createMap;
            
        }

        bool b = false;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (b)
            {
                MenuLeft.Width = new GridLength(150);
                MenuRight.Width = new GridLength(150);
            }
            else
            {
                MenuLeft.Width = new GridLength(0);
                MenuRight.Width = new GridLength(0);
            }
            b = !b;
        }

        private void ListRoat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListRoat.SelectedIndex != -1)
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
                    ListDot.Items.Add("ok");
                    ListDot.SelectedIndex = ListDot.Items.Count - 1;

                    com = new OracleCommand("SELECT name,Country,City,Duration_Day,type,Description,Full_Name FROM roat INNER JOIN USERS ON users.ID=ID_User"
                        , oracleConn);

                    var res2 = com.ExecuteReader();
                    while (res.Read())
                    {
                        RoatName.Text = res[0].ToString();
                        RoatContry.Text = res[1].ToString();
                        RoatCity.Text = res[2].ToString();
                        RoatDuration.Text = res[3].ToString();
                        RoatType.Text = res[4].ToString();
                        RoatDescript.Text = res[5].ToString();
                        UserName.Text = res[6].ToString();
                    }

                }
                catch { }
        }

        private void ListDot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             if (ListDot.SelectedIndex != -1)
                 if (ListDot.SelectedItem.ToString() == "ok")
                 {
                     DotName.Text = "";
                     DotDescript.Text = "";
                     DotCoord.Text = "";
                     DotImage.Text = "";
                     dot.index = ListDot.SelectedIndex;
                 }
                 else
                     try
                     {
                         OracleCommand com = new OracleCommand("SELECT name,Coordinates,Description,Image FROM Waypoint INNER JOIN Roat ON Roat.ID=id_Roat WHERE Waypoint.Name='"
                             + ListDot.SelectedItem.ToString() + "'", oracleConn);

                         var res = com.ExecuteReader();
                         while (res.Read())
                         {
                             DotName.Text = res[0].ToString();
                             DotDescript.Text = res[2].ToString();
                             DotCoord.Text = res[1].ToString();
                             DotImage.Text = res[3].ToString();
                         }
                         dot.index = ListDot.SelectedIndex;
                     }
                     catch { }

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
                    + Route_Descript.Text+ "',1,2)", oracleConn);
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

        private void Save_Route_Click(object sender, RoutedEventArgs e)
        {
            if (RoatCity.Text != null && RoatContry.Text != null && RoatName.Text != null && RoatDuration.Text != null && RoatType.Text != null && RoatDescript.Text != null && UserName.Text != null)
                try
                {
                    string idUser = null;
                    OracleCommand com = new OracleCommand("SELECT ID_User FROM roat INNER JOIN USERS ON users.ID=ID_User WHERE Full_Name='"
                        + UserName.Text + "'", oracleConn);
                    var resUser = com.ExecuteReader();
                    while (resUser.Read())
                        idUser = resUser[0].ToString();

                    com = new OracleCommand("INSERT INTO Roat(name,Country,City,Duration_Day,type,Description,id_user) VALUES('"
                    + RoatName.Text + "','" + RoatContry.Text +
                     RoatCity.Text + "','" + RoatDuration.Text +
                     RoatType.Text + "','" + RoatDescript.Text +
                     idUser + "')", oracleConn);
                    var res = com.ExecuteReader();
                }
                catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //OracleCommand com = new OracleCommand("SELECT Full_Name FROM USERS WHERE Status='1'", oracleConn);
            //var res = com.ExecuteReader();
            //ComboModer.Items.Clear();
            //while (res.Read())
            //    ComboModer.Items.Add(res[0].ToString());
        }

        private void DotSave_Click(object sender, RoutedEventArgs e)
        {
            if (createWarpPoint==false) 
            {
                //OracleCommand com = new OracleCommand("SELECT Full_Name FROM USERS WHERE Status='1'", oracleConn);
                //var res = com.ExecuteReader();
                dot.AddDot();
                ListDot.Items.Clear();
                foreach (var item in dot.Lpll)
                    ListDot.Items.Add(item.ToString());
                ListDot.Items.Add("ok");
                ListDot.SelectedIndex = ListDot.Items.Count - 1;

                createWarpPoint = true;
                DotSave.IsEnabled = false;
                Create_Map.IsEnabled = true;
            }
            
        }

    }
}
