﻿using Common;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace SnakeWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        public ViewModelUserSettings ViewModelUserSettings = new ViewModelUserSettings();
        public ViewModelGames ViewModelGames = null;
        public static IPAddress remoteIPAddress = IPAddress.Parse("127.0.0.1");
        public static int remotePort = 5001;
        public Thread tRec;
        public UdpClient receivingUdpClient;
        public Pages.Home Home = new Pages.Home();
        public Pages.Game Game = new Pages.Game();
        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            OpenPage(Home);
        }
        public void StartReceiver()
        {
            tRec = new Thread(new ThreadStart(Receiver));
            tRec.Start();
        }
        public void OpenPage(Page PageOpen)
        {
            frame.Navigate(PageOpen);
        }
        public void Receiver()
        {
            receivingUdpClient = new UdpClient(int.Parse(ViewModelUserSettings.Port));
            IPEndPoint RemoteIpEndPoint = null;
            try
            {
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    if (ViewModelGames == null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            OpenPage(Game);
                        });
                    }
                    var data = JsonConvert.DeserializeObject<GameData>(returnData);
                    ViewModelGames = new ViewModelGames
                    {
                        AllSnakes = data.AllSnakes,
                        Points = data.ApplePoint
                    };
                    if (ViewModelGames.AllSnakes.Any(x => x.SnakesPlayers.GameOver))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            OpenPage(new Pages.EndGame());
                        });
                    }
                    else
                    {
                        Game.CreateUI();
                    }
                }
            } catch (Exception ex)
            {
                Debug.WriteLine("Возникло исключение: " + ex.ToString() + "\n " + ex.Message);
            }
        }
        public static void Send(string datagram)
        {
            UdpClient sender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(remoteIPAddress, remotePort);
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(datagram);
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Возникло исключение: " + ex.ToString() + "\n " + ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }
        private void EventKeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModelUserSettings.IPAddress) &&
                !string.IsNullOrEmpty(ViewModelUserSettings.Port) &&
                (ViewModelGames != null && !ViewModelGames.SnakesPlayers.GameOver))
            {
                if (e.Key == Key.Up) 
                {
                    Send($"Up|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                }
                else if (e.Key == Key.Down)
                {
                    Send($"Down|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                }
                else if (e.Key == Key.Left)
                {
                    Send($"Left|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                }
                else if (e.Key == Key.Right)
                {
                    Send($"Right|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                }
            }
        }
        public void QuitApplication(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (receivingUdpClient != null) receivingUdpClient.Close();
            else return;
            tRec.Abort();
        }
    }
}
