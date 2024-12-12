using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using Image = System.Windows.Controls.Image;

namespace NMSE;

  class EnlargeAbleImage
  {

  public string ImageTitle { get; set; }
  public int ImageWidth { get; set; }

  public int ImageMaxWidth { get; set; }

  public int ImageHeight { get; set; }

  public string ImageMaxHeight { get; set; }

  public string ImageUri { get; set; }


  public string ImageTooltip = "Click to enlarge";



  public EnlargeAbleImage() { 
    
    }


  public void ReturnImage()
    {

    }


  public Window ReturnEnlargedWindow ()
    {

    // Create a new window to display the enlarged image
    var enlargedImageWindow = new Window
      {
      Title = "Enlarged Image",
      Width = 800,
      Height = 600,
      Content = new Image
        {
        Source = new BitmapImage(new Uri("pack://application:,,,/NMSE;component/Images/transitions-base-model.png")),
        Stretch = System.Windows.Media.Stretch.Uniform
        }
      };

    return enlargedImageWindow;


    }



  }
