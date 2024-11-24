﻿using System;
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

namespace SnakeWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для EndGame.xaml
    /// </summary>
    public partial class EndGame : Page
    {
        public EndGame()
        {
            InitializeComponent();
            name.Content = MainWindow.mainWindow.ViewModelUserSettings.UserName;
            top.Content = MainWindow.mainWindow.ViewModelGames.Top;
            //Console.WriteLine($"Очки пользователя: {remoteIPAddress.Find(x => x.IdSnake == User.IdSnake).IPAddress}:{remoteIPAddress.Find(x => x.IdSnake == User.IdSnake).Port} == {dataToSend.AllSnakes.Find(x => x.IdSnake == User.IdSnake).SnakesPlayers.Points.Count - 3}");
            score.Content = $"{MainWindow.mainWindow.ViewModelGames.SnakesPlayers.Points.Count - 3} score";
            MainWindow.mainWindow.receivingUdpClient.Close();
            MainWindow.mainWindow.tRec.Abort();
            MainWindow.mainWindow.ViewModelGames = null;
        }
        public void OpenHome(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(MainWindow.mainWindow.Home);
        }
    }
}
