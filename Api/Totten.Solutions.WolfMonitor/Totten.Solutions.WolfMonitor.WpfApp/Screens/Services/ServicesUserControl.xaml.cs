﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Totten.Solutions.WolfMonitor.WpfApp.Applications.Monitorings;
using Totten.Solutions.WolfMonitor.WpfApp.Screens.Items;
using Totten.Solutions.WolfMonitor.WpfApp.ValueObjects;
using Totten.Solutions.WolfMonitor.WpfApp.ValueObjects.SystemServices;

namespace Totten.Solutions.WolfMonitor.WpfApp.Screens.Services
{
    public partial class ServicesUserControl : UserControl, IUserControl
    {
        private ItensMonitoringService _itemsMonitoringService;
        public Guid _agentId;

        public ServicesUserControl(Guid agentId, ItensMonitoringService itemsMonitoringService)
        {
            InitializeComponent();
            _agentId = agentId;
            _itemsMonitoringService = itemsMonitoringService;
            Populate();
        }

        public void Populate()
        {
            this.wrapPanel.Children.Clear();

            _itemsMonitoringService.GetSystemServices(_agentId).ContinueWith(task =>
            {
                if (task.Result.IsSuccess)
                {
                    this.wrapPanel.Dispatcher.Invoke(() =>
                    {
                        foreach (SystemServiceViewModel service in task.Result.Success.Items)
                        {
                            this.wrapPanel.Children.Add(new ServiceUC(OnRemove, service));
                        }
                        OnApplyTemplate();
                    });
                }
            });
        }

        private async void OnRemove(object sender, EventArgs e)
        {
            SystemServiceViewModel serviceViewModel = sender as SystemServiceViewModel;
            if (MessageBox.Show($"Deseja realmente remover o serviço: {serviceViewModel.DisplayName} do monitoramento?", "Atênção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                var removeCallback = await _itemsMonitoringService.Delete(_agentId, serviceViewModel.Id);

                if (removeCallback.IsSuccess)
                {
                    MessageBox.Show($"Removido com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Falha na tentativa de remoção.", "Falha", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void btnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FrmItemsAdd frmItemsAdd = new FrmItemsAdd(new ServicesCreateUC(_agentId));

            var dialogResult = frmItemsAdd.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                await _itemsMonitoringService.Post(frmItemsAdd.Item);
            }
        }
    }
}
