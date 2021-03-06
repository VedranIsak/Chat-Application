﻿using System.Net;
using System.Windows.Input;

using TDDD49.Models;
using TDDD49.ViewModels.Commands;

namespace TDDD49.ViewModels
{
    public class ConnectViewModel : ViewModel
    {
        private ChatViewModel chatViewModel;
        private bool showSuccessfulListen = false;
        private bool isListening = false;
        private bool isConnecting = false;
        private int externalPort;
        private string externalIpAddress;
        private Communicator communicator;

        public ConnectViewModel(ChatViewModel chatViewModel, Communicator c)
        {
            this.chatViewModel = chatViewModel;
            AddUserCommand = new AddUserCommand(this, chatViewModel, c);
            ListenCommand = new ListenCommand(this, c, chatViewModel);
            communicator = c;
        }

        public bool ShowSuccessfulListen
        {
            get { return showSuccessfulListen; }
            set
            {
                showSuccessfulListen = value;
                OnPropertyChanged(nameof(ShowSuccessfulListen));
            }
        }

        public ICommand ListenCommand { get; set; }
        public ICommand AddUserCommand { get; set; }

        public string ExternalIpAddress
        {
            get { return externalIpAddress; }
            set
            {
                ValidIpAddress = CheckIpAddress(value);
                externalIpAddress = value;
                OnPropertyChanged(ExternalIpAddress);
            }
        }

        public int ExternalPort
        {
            get { return externalPort; }
            set
            {
                ValidPort = CheckPort(value);
                externalPort = value;
                OnPropertyChanged(nameof(ExternalPort));
            }
        }

        public bool IsListening
        {
            get { return isListening; }
            set
            {
                isListening = value;
                OnPropertyChanged(nameof(IsListening));
            }
        }

        public bool IsConnecting
        {
            get { return isConnecting; }
            set
            {
                isConnecting = value;
                OnPropertyChanged(nameof(IsConnecting));
            }
        }
    }
}
