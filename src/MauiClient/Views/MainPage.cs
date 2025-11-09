using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestAppMaui.Converters;
using TestAppMaui.MauiClient.ViewModels;

namespace TestAppMaui.Views
{
    public sealed partial class MainPage : BasePage
    {

        public MainPage(MainViewModel mainViewModel)
        {
            BindingContext = mainViewModel;
           
        }

        public override void Build()
        {
            Title = "Strona główna";

            Resources = new ResourceDictionary
        {
            { "NullToBoolConverter", new NullToBoolConverter() }
        };

            // --- Nagłówek ---
            var headerLabel = new Label
            {
                Text = "Zadania 123",
                FontAttributes = FontAttributes.Bold,
                FontSize = 24
            };

            // --- Pole tekstowe + przycisk ---
            var nameEntry = new Entry
            {
                Placeholder = "Nazwa zadania123"
            };
            nameEntry.SetBinding(Entry.TextProperty, "NewTaskName");

            var sendButton = new Button
            {
                Text = "Wyślij"
            };
            sendButton.SetBinding(Button.CommandProperty, "SendCommand");

            var descEntry = new Entry
            {
                Placeholder = "Opis (max 250 znaków)",
                MaxLength = 250
            };
            descEntry.SetBinding(Entry.TextProperty, "NewTaskDescription");

            var grid = new Grid
            {
                ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            },
                RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
                ColumnSpacing = 12,
                RowSpacing = 12
            };

            // Pozycjonowanie w siatce
            Grid.SetColumn(nameEntry, 0);
            Grid.SetRow(nameEntry, 0);
            grid.Children.Add(nameEntry);

            Grid.SetColumn(sendButton, 1);
            Grid.SetRow(sendButton, 0);
            grid.Children.Add(sendButton);

            Grid.SetColumn(descEntry, 0);
            Grid.SetRow(descEntry, 1);
            Grid.SetColumnSpan(descEntry, 2);
            grid.Children.Add(descEntry);

            // --- Komunikat o błędzie ---
            var errorLabel = new Label
            {
                TextColor = Colors.Red
            };
            errorLabel.SetBinding(Label.TextProperty, "ErrorMessage");
            errorLabel.SetBinding(Label.IsVisibleProperty, new Binding("ErrorMessage")
            {
                Converter = (IValueConverter)Resources["NullToBoolConverter"]
            });

            // --- Lista zadań ---
            var collectionView = new CollectionView();
            collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Tasks");

            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var nameLabel = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18
                };
                nameLabel.SetBinding(Label.TextProperty, "Name");

                var descriptionLabel = new Label
                {
                    FontSize = 14,
                    TextColor = Colors.Gray,
                    LineBreakMode = LineBreakMode.WordWrap
                };
                descriptionLabel.SetBinding(Label.TextProperty, "Description");
                descriptionLabel.SetBinding(Label.IsVisibleProperty, new Binding("Description")
                {
                    Converter = (IValueConverter)Resources["NullToBoolConverter"]
                });

                var stack = new VerticalStackLayout
                {
                    Spacing = 4,
                    Children = { nameLabel, descriptionLabel }
                };

                return new Border
                {
                    Margin = new Thickness(0, 4),
                    Padding = 8,
                    Content = stack
                };
            });

            // --- Główny layout ---
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Spacing = 16,
                Children =
            {
                headerLabel,
                grid,
                errorLabel,
                collectionView
            }
            };
        }
    }
}