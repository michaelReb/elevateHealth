﻿using NMSE.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace NMSE.Views;

public partial class Data2Page : Page
{
    public Data2Page(Data2ViewModel viewModel)
    {
        InitializeComponent();
        DataContext = SimProb.Instance;
    }



  private void BaseModelExample_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    // Create a new window to display the enlarged image
    var enlargedImageWindow = new Window
      {
      Title = "Enlarged Image",
      Width = 800,
      Height = 600,
      Content = new Image
        {
        Source = new BitmapImage(new Uri("pack://application:,,,/NMSE;component/Images/time-relations.png")),
        //Source = new BitmapImage(new Uri("/Images/transitions-base-model.png", UriKind.Relative)),
        Stretch = System.Windows.Media.Stretch.Uniform
        }
      };

    // Show the window
    enlargedImageWindow.ShowDialog();
    }

  private void ResetTimeRelationsTable_Click(object sender, RoutedEventArgs e)
    {
    SimProb.Instance.SetDefaultForWeeks();
    }
  }
