// AddItemPage.xaml.cs
using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SystemZarzadzania_PM
{
    public partial class AddItemPage : ContentPage
    {
        private Collection selectedCollection;

        public AddItemPage(Collection collection)
        {
            InitializeComponent();
            selectedCollection = collection;
        }

        private async Task SaveItemAsync(string itemName)
        {
            if (!string.IsNullOrWhiteSpace(itemName))
            {
                selectedCollection.Items.Add(itemName);
                await Navigation.PopModalAsync(); // Zapisuje tylko po zakoñczeniu dodawania
            }
        }

        private async void AddItemButton_Clicked(object sender, EventArgs e)
        {
            string itemName = ItemNameEntry.Text;
            await SaveItemAsync(itemName);
        }
    }
}
