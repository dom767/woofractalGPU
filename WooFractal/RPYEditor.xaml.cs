﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WooFractal
{
    /// <summary>   
    /// Interaction logic for RPYEditor.xaml
    /// </summary>
    public partial class RPYEditor : UserControl
    {
        public RPYEditor()
        {
            InitializeComponent();
            this.Focus();
        }

        private void Grid_LostFocus(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).MainCanvas.Children.Remove(this);
        }
    }
}
