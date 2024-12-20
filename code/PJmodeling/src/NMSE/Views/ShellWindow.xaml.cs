﻿using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.Controls;

using NMSE.Contracts.Views;
using NMSE.ViewModels;

namespace NMSE.Views;

public partial class ShellWindow : MetroWindow, IShellWindow
{
    public ShellWindow(ShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    public Frame GetNavigationFrame()
        => shellFrame;

    public void ShowWindow()
        => Show();

    public void CloseWindow()
        => Close();






  }
