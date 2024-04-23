// MainPage.xaml.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace SystemZarzadzania_PM
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Collection> collections;
        private string dataFilePath;

        public MainPage()
        {
            InitializeComponent();
            dataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "collections.txt");
            collections = new ObservableCollection<Collection>();
            LoadCollections();
            CollectionListView.ItemsSource = collections;
        }

        private void LoadCollections()
        {
            if (File.Exists(dataFilePath))
            {
                using (StreamReader reader = new StreamReader(dataFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 2)
                        {
                            string name = parts[0];
                            string type = parts[1];
                            Collection collection = new Collection(name, type);
                            collections.Add(collection);
                            LoadItems(collection); // Wczytaj przedmioty dla każdej kolekcji
                        }
                        else
                        {
                            Console.WriteLine($"Invalid data format: {line}");
                        }
                    }
                }
            }
        }

        private void LoadItems(Collection collection)
        {
            string itemFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{collection.Name}_items.txt");
            if (File.Exists(itemFilePath))
            {
                using (StreamReader reader = new StreamReader(itemFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        collection.Items.Add(line);
                    }
                }
            }
        }

        private async Task SaveCollectionsAsync()
        {
            using (StreamWriter writer = new StreamWriter(dataFilePath))
            {
                foreach (Collection collection in collections)
                {
                    await writer.WriteLineAsync($"{collection.Name},{collection.Type}");
                    await SaveItemsAsync(collection); // Zapisz przedmioty dla każdej kolekcji
                }
            }
        }

        private async Task SaveItemsAsync(Collection collection)
        {
            string itemFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{collection.Name}_items.txt");
            using (StreamWriter writer = new StreamWriter(itemFilePath))
            {
                foreach (string item in collection.Items)
                {
                    await writer.WriteLineAsync(item);
                }
            }
        }

        private async void AddCollectionButton_Clicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("New Collection", "Enter collection name:");
            if (!string.IsNullOrWhiteSpace(name))
            {
                string type = await DisplayActionSheet("Collection Type", "Cancel", null, "Books", "Video Games", "Board Games", "LEGO Sets", "TCG Cards", "Music CDs");
                if (type != null && type != "Cancel")
                {
                    Collection collection = new Collection(name, type);
                    collections.Add(collection);
                    await SaveCollectionsAsync();
                }
            }
            await SaveCollectionsAsync();
        }

        private async void CollectionListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                Collection selectedCollection = e.Item as Collection;
                string action = await DisplayActionSheet("Options", "Cancel", null, "Edit", "Delete", "Add Item", "View Items");
                if (action == "Edit")
                {
                    string newName = await DisplayPromptAsync("Edit Collection", "Enter new name:", "OK", "Cancel", selectedCollection.Name);
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        selectedCollection.Name = newName;
                        await SaveCollectionsAsync();
                    }
                }
                else if (action == "Delete")
                {
                    bool delete = await DisplayAlert("Delete Collection", $"Are you sure you want to delete {selectedCollection.Name}?", "Yes", "No");
                    if (delete)
                    {
                        collections.Remove(selectedCollection);
                        await SaveCollectionsAsync();
                    }
                }
                else if (action == "Add Item")
                {
                    await Navigation.PushModalAsync(new AddItemPage(selectedCollection));
                    await SaveCollectionsAsync();
                }
                else if (action == "View Items")
                {
                    await Navigation.PushAsync(new CollectionDetailPage(selectedCollection));
                    await SaveCollectionsAsync();
                }
            }
        }
    }

    public class Collection
    {
        public string Name
        {
            get; set;
        }
        public string Type
        {
            get; set;
        }
        public List<string> Items
        {
            get; set;
        }

        public Collection(string name, string type)
        {
            Name = name;
            Type = type;
            Items = new List<string>();
        }
    }
}
