using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SystemZarzadzania_PM;

public partial class CollectionDetailPage : ContentPage
{
    public CollectionDetailPage(Collection collection)
    {
        InitializeComponent();
        Title = "Collection Detail";
        BindingContext = collection;
        ItemsListView.ItemsSource = collection.Items;
    }


}