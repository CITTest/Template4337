﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Template4337
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void gruppa4337_Gatina(object sender, RoutedEventArgs e)
        {
           Gatina_4337 g= new Gatina_4337();
            g.Show();
        }

        private void gruppa4337_Tishchenko(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
           Tishchenko_4337 g= new Tishchenko_4337();
            g.Show();
=======
            Tishchenko_4337 a = new Tishchenko_4337();
            a.Show();
>>>>>>> ec99bfa (AAAAAAAAAAAA)
        }
    }
}
