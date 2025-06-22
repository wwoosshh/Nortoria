using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Connection.Models;

namespace Connection.ViewModels
{
    public class InventoryViewModel : ObservableObject
    {
        private readonly UserData _userData;
        private readonly Dictionary<string, ItemInfo> _itemDatabase;
        private InventoryItemViewModel _selectedItem;

        public InventoryViewModel(UserData userData)
        {
            _userData = userData;
            _itemDatabase = LoadItemDatabase();
            InventoryItems = new ObservableCollection<InventoryItemViewModel>();

            RefreshInventory();
        }

        public ObservableCollection<InventoryItemViewModel> InventoryItems { get; }

        public long Currency => _userData.Inventory.Currency;

        public InventoryItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnPropertyChanged(nameof(SelectedItemVisibility));
                OnPropertyChanged(nameof(NoSelectionVisibility));
            }
        }

        public Visibility SelectedItemVisibility => SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
        public Visibility NoSelectionVisibility => SelectedItem == null ? Visibility.Visible : Visibility.Collapsed;

        private Dictionary<string, ItemInfo> LoadItemDatabase()
        {
            return new Dictionary<string, ItemInfo>
            {
                ["mysterious_stone"] = new ItemInfo
                {
                    Id = "mysterious_stone",
                    Name = "신비한 돌",
                    Description = "이상한 힘이 느껴지는 돌입니다.",
                    Type = "재료",
                    Icon = "🪨",
                    Value = 50,
                    Usable = false
                },
                ["health_potion"] = new ItemInfo
                {
                    Id = "health_potion",
                    Name = "체력 포션",
                    Description = "체력을 회복시켜주는 빨간 포션입니다.",
                    Type = "소모품",
                    Icon = "🧪",
                    Value = 100,
                    Usable = true
                }
            };
        }

        public void RefreshInventory()
        {
            InventoryItems.Clear();

            foreach (var item in _userData.Inventory.Items)
            {
                if (_itemDatabase.TryGetValue(item.Key, out var itemInfo))
                {
                    InventoryItems.Add(new InventoryItemViewModel
                    {
                        Id = item.Key,
                        Name = itemInfo.Name,
                        Description = itemInfo.Description,
                        TypeText = itemInfo.Type,
                        DisplayIcon = itemInfo.Icon,
                        Quantity = item.Value,
                        Value = itemInfo.Value,
                        Usable = itemInfo.Usable,
                        QuantityVisibility = item.Value > 1 ? Visibility.Visible : Visibility.Collapsed,
                        ValueVisibility = itemInfo.Value > 0 ? Visibility.Visible : Visibility.Collapsed,
                        UsableVisibility = itemInfo.Usable ? Visibility.Visible : Visibility.Collapsed
                    });
                }
            }

            OnPropertyChanged(nameof(Currency));
        }

        public void UseItem(string itemId)
        {
            if (_itemDatabase.TryGetValue(itemId, out var itemInfo) && itemInfo.Usable)
            {
                MessageBox.Show($"{itemInfo.Name}을(를) 사용했습니다!", "아이템 사용");
                if (itemInfo.Type == "소모품")
                {
                    _userData.Inventory.Items[itemId]--;
                    if (_userData.Inventory.Items[itemId] <= 0)
                    {
                        _userData.Inventory.Items.Remove(itemId);
                    }
                }
                RefreshInventory();
                SelectedItem = null;
            }
        }

        public void SortItems()
        {
            var sortedItems = InventoryItems.OrderBy(i => i.TypeText).ThenBy(i => i.Name).ToList();
            InventoryItems.Clear();
            foreach (var item in sortedItems)
            {
                InventoryItems.Add(item);
            }
        }
    }

    public class InventoryItemViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TypeText { get; set; }
        public string DisplayIcon { get; set; }
        public int Quantity { get; set; }
        public int Value { get; set; }
        public bool Usable { get; set; }
        public Visibility QuantityVisibility { get; set; }
        public Visibility ValueVisibility { get; set; }
        public Visibility UsableVisibility { get; set; }
    }

    public class ItemInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public int Value { get; set; }
        public bool Usable { get; set; }
    }
}