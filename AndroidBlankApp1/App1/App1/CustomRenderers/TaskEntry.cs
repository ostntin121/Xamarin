using Xamarin.Forms;

namespace App1.CustomRenderers
{
    public class TaskEntry: Entry
    {
        public static readonly BindableProperty NeedToFocusProperty = BindableProperty.Create(nameof(NeedToFocus), 
            typeof(bool), typeof(TaskEntry), 
            default(bool), propertyChanged: OnNeedToFocusChanged);
        public bool NeedToFocus
        {
            get { return (bool) GetValue(NeedToFocusProperty); }
            set { SetValue(NeedToFocusProperty, value); }
        }
        static void OnNeedToFocusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = (TaskEntry) bindable;
            item.Focus();
        }
    }
}