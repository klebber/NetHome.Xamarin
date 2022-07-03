﻿using System;
using System.Collections.Generic;
using System.Text;
using NetHome.Common;
using NetHome.Services;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections;
using System.Threading.Tasks;
using NetHome.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using NetHome.Views;
using NetHome.Views.DevicePages;

namespace NetHome.ViewModels
{
    public class DeviceSettingsViewModel : BaseViewModel
    {
        private readonly IDeviceManager _deviceManager;
        private readonly IDeviceService _deviceService;
        private Command onRefreshed;
        private ObservableCollection<DeviceModel> devices;
        private Command<int> goToDeviceInfo;
        private Command addCommand;
        private Command addRoomCommand;
        private Command deleteRoomCommand;
        private Command addTypeCommand;
        private Command deleteTypeCommand;

        public ObservableCollection<DeviceModel> Devices { get => devices; set => SetProperty(ref devices, value); }
        public ICommand OnRefreshed => onRefreshed ??= new Command(PerformOnRefreshed);
        public ICommand GoToDeviceInfo => goToDeviceInfo ??= new Command<int>(async (param) => await PerformGoToDeviceInfo(param));
        public ICommand AddCommand => addCommand ??= new Command(async () => await Add());
        public ICommand AddRoomCommand => addRoomCommand ??= new Command(async () => await AddRoom());
        public ICommand DeleteRoomCommand => deleteRoomCommand ??= new Command(async () => await DeleteRoom());
        public ICommand AddTypeCommand => addTypeCommand ??= new Command(async () => await AddType());
        public ICommand DeleteTypeCommand => deleteTypeCommand ??= new Command(async () => await DeleteType());

        public DeviceSettingsViewModel()
        {
            Devices = new ObservableCollection<DeviceModel>();
            BindingBase.EnableCollectionSynchronization(Devices, null, ObservableCollectionCallback);
            _deviceManager = DependencyService.Get<IDeviceManager>();
            _deviceService = DependencyService.Get<IDeviceService>();
        }

        private void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        internal void OnAppearing()
        {
            SetDevices();
        }


        private void SetDevices()
        {
            Devices = new ObservableCollection<DeviceModel>(_deviceManager.GetAllDevices());
            IsWaiting = false;
        }

        private void PerformOnRefreshed()
        {
            SetDevices();
        }

        private async Task PerformGoToDeviceInfo(int param)
        {
            var route = $"{nameof(DeviceInfoPage)}?{nameof(DeviceInfoPage.DeviceId)}={param}";
            await Shell.Current.GoToAsync(route);
        }

        private async Task Add()
        {
            var route = $"{nameof(DeviceInfoPage)}";
            await Shell.Current.GoToAsync(route);
        }


        private async Task AddRoom()
        {
            string result = await Shell.Current.ShowPopupAsync(new Propmpt(
                "Add New Room", "Enter room name here:",
                "Room Name", "", "Add", true, true, keyboard: Keyboard.Text));
            if (string.IsNullOrWhiteSpace(result))
                return;
            var response = await _deviceService.AddRoom(result);
            if (response.IsSuccessful)
            {
                await Shell.Current.ShowPopupAsync(new Alert("Success!", "New room has been added.", "Ok", true));
            }
            else
            {
                await Shell.Current.ShowPopupAsync(new Alert(response.ErrorType, response.ErrorMessage, "Ok", true));
            }
        }


        private async Task DeleteRoom()
        {
            var itemsSource = await _deviceService.GetAllRooms();
            string result = await Shell.Current.ShowPopupAsync(new PickerPopup(
                "Delete Room", "Pick a room to delete:",
                "Room Name", itemsSource.Paylaod, "Delete", true, true, keyboard: Keyboard.Text));
            if (string.IsNullOrWhiteSpace(result))
                return;
            var response = await _deviceService.DeleteRoom(result);
            if (response.IsSuccessful)
            {
                await Shell.Current.ShowPopupAsync(new Alert("Success!", $"Room {result} has been deleted.", "Ok", true));
            }
            else
            {
                await Shell.Current.ShowPopupAsync(new Alert(response.ErrorType, response.ErrorMessage, "Ok", true));
            }
        }


        private async Task AddType()
        {
            string result = await Shell.Current.ShowPopupAsync(new Propmpt(
                "Add New Device Type", "Enter type name here:",
                "Type Name", "", "Add", true, true, keyboard: Keyboard.Text));
            if (string.IsNullOrWhiteSpace(result))
                return;
            var response = await _deviceService.AddType(result);
            if (response.IsSuccessful)
            {
                await Shell.Current.ShowPopupAsync(new Alert("Success!", "New type has been added.", "Ok", true));
            }
            else
            {
                await Shell.Current.ShowPopupAsync(new Alert(response.ErrorType, response.ErrorMessage, "Ok", true));
            }
        }


        private async Task DeleteType()
        {
            var itemsSource = await _deviceService.GetAllDeviceTypes();
            string result = await Shell.Current.ShowPopupAsync(new PickerPopup(
                "Delete Device Type", "Pick a type to delete:",
                "Type Name", itemsSource.Paylaod, "Delete", true, true, keyboard: Keyboard.Text));
            if (string.IsNullOrWhiteSpace(result))
                return;
            var response = await _deviceService.DeleteType(result);
            if (response.IsSuccessful)
            {
                await Shell.Current.ShowPopupAsync(new Alert("Success!", $"Device type {result} has been deleted.", "Ok", true));
            }
            else
            {
                await Shell.Current.ShowPopupAsync(new Alert(response.ErrorType, response.ErrorMessage, "Ok", true));
            }
        }
    }
}