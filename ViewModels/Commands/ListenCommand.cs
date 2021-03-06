﻿using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using TDDD49.Models;

namespace TDDD49.ViewModels.Commands
{
    public class ListenCommand : ICommand
    {
        private ConnectViewModel connectViewModel;
        private Communicator communicator;
        private Thread listenThread;
        private ChatViewModel chatViewModel;

        public ListenCommand(ConnectViewModel connectViewModel, Communicator c, ChatViewModel chatViewModel)
        {
            this.connectViewModel = connectViewModel;
            this.communicator = c;
            this.chatViewModel = chatViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (chatViewModel.InternalUser != null && !connectViewModel.IsListening) { return true; }
            return false;
        }

        public void Execute(object parameter)
        {
            if (listenThread != null)
            {
                if (listenThread.IsAlive)
                {

                    communicator.stopChatting(chatViewModel.InternalUser);
                    listenThread.Abort();
                    listenThread = null;
                }
                else
                {
                    communicator.stopChatting(chatViewModel.InternalUser);
                }
            }

            Thread showThread = new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    connectViewModel.ShowSuccessfulListen = true;
                });

                Thread.Sleep(3000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    connectViewModel.ShowSuccessfulListen = false;
                });
            });

            showThread.IsBackground = false;
            showThread.Start();

            listenThread = new Thread(() =>
            {
                chatViewModel.IsListening = true;
                connectViewModel.IsListening = true;
                try
                {
                    communicator.ListenToPort(internalUser: this.chatViewModel.InternalUser, port: this.chatViewModel.InternalUser.Port, cvm: chatViewModel, ip: chatViewModel.InternalUser.IpAddress);

                    if (communicator.externalUser != null)
                    {
                        User newUser = new User()
                        {
                            ID = communicator.externalUser.ID,
                            Name = communicator.externalUser.Name,
                            IpAddress = communicator.externalUser.IpAddress,
                            Port = communicator.externalUser.Port,
                            Messages = new System.Collections.ObjectModel.ObservableCollection<Message>()
                        };

                        if (!chatViewModel.Users.Any(item => item.ID == newUser.ID))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                chatViewModel.AddUser(newUser);
                            });
                            chatViewModel.ChattingUser = newUser;
                        }
                        else
                        {
                            chatViewModel.ChattingUser = chatViewModel.Users.Single(item => item.ID == newUser.ID);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to connect");
                    }

                }
                catch (SocketException e1)
                {
                    MessageBox.Show("Anslutningen bröts, försök igen");
                    Console.WriteLine("SocketException: {0}", e1);
                }
                catch (ThreadAbortException e2)
                {
                    MessageBox.Show("Anslutningen bröts, försök igen");
                    Console.WriteLine(e2);
                }
                catch (NullReferenceException e3)
                {
                    Console.WriteLine(e3);
                }
                catch (ObjectDisposedException e4)
                {
                    Console.WriteLine(e4);
                }
                catch (Communicator.BadServerException e3)
                {
                    MessageBox.Show("Bad IP address, try again", "Bad IP address");
                    Console.WriteLine(e3);
                }
                finally
                {
                    chatViewModel.IsListening = false;
                    connectViewModel.IsListening = false;
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }
    }
}
