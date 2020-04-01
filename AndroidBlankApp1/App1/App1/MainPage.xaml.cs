using App1.Data;
using Xamarin.Forms;

namespace App1
{
    public partial class MainPage : TabbedPage
    {
        public MainPage(DbContext dbContext)
        {
            InitializeComponent();

            var cwp = new CurrentWeekPage(dbContext);
            var sp = new ScratchesPage(dbContext, cwp.BindingContext);
            var hp = new HistoryPage(dbContext);

            Children.Add(new NavigationPage(cwp) {Title = "Текущие планы"});
            Children.Add(new NavigationPage(sp) {Title = "Черновики"});
            Children.Add(new NavigationPage(hp) {Title = "История"});
        }
    }
}