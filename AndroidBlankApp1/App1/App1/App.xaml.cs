using System;
using System.IO;
using App1.Data;
using App1.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace App1
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "xamarin.db";
        public static DbContext database;
        public static DbContext Database
        {
            get
            {
                if (database == null)
                {
                    database = new DbContext(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME));
                }
                return database;
            }
        }
        public App()
        {
            Device.SetFlags(new string[]{ "MediaElement_Experimental" });
            
            InitializeComponent();

            MainPage = new MainPage(Database);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}