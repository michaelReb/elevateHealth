﻿using System.Windows.Controls;
using System.Windows.Navigation;
using NMSE.Contracts.Services;
using NMSE.Contracts.ViewModels;

namespace NMSE.Services;

public class NavigationService : INavigationService {
  readonly IPageService _pageService;
  Frame _frame;
  object _lastParameterUsed;

  public event EventHandler<string> Navigated;

  public bool CanGoBack => _frame.CanGoBack;

  public NavigationService(IPageService pageService) { _pageService = pageService; }

  public void Initialize(Frame shellFrame) {
    if (_frame == null) {
      _frame = shellFrame;
      _frame.Navigated += OnNavigated;
    }
  }

  public void UnsubscribeNavigation() {
    _frame.Navigated -= OnNavigated;
    _frame = null;
  }

  public void GoBack() {
    if (_frame.CanGoBack) {
      var vmBeforeNavigation = _frame.GetDataContext();
      _frame.GoBack();
      if (vmBeforeNavigation is INavigationAware navigationAware) navigationAware.OnNavigatedFrom();
    }
  }

  public bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false) {
    var pageType = _pageService.GetPageType(pageKey);

    if (_frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed))) {
      _frame.Tag = clearNavigation;
      var page = _pageService.GetPage(pageKey);
      var navigated = _frame.Navigate(page, parameter);
      if (navigated) {
        _lastParameterUsed = parameter;
        var dataContext = _frame.GetDataContext();
        if (dataContext is INavigationAware navigationAware) navigationAware.OnNavigatedFrom();
      }

      return navigated;
    }

    return false;
  }

  public void CleanNavigation()
    => _frame.CleanNavigation();

  void OnNavigated(object sender, NavigationEventArgs e) {
    if (sender is Frame frame) {
      var clearNavigation = (bool)frame.Tag;
      if (clearNavigation) frame.CleanNavigation();

      var dataContext = frame.GetDataContext();
      if (dataContext is INavigationAware navigationAware) navigationAware.OnNavigatedTo(e.ExtraData);

      Navigated?.Invoke(sender, dataContext.GetType().FullName);
    }
  }
}
