using Common.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishWords.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        private int showInterval { get; set; } = 60;
        private int hideInterval { get; set; } = 5;
        private bool autoHide { get; set; } = true;
        private int order { get; set; } = 0;
        public ShowOrder ShowOrder { get; set; }

        public event EventHandler OnIntervalChanged;
        public event EventHandler OnOrderChanged;
        public int Order
        {
            get => order;
            set
            {
                order = value;
                ShowOrder = (ShowOrder)value;
                OnOrderChanged.Invoke(this, null);
                OnPropertyChanged("Order");
            }
        }
        public int HideInterval
        {
            get => hideInterval;
            set
            {
                hideInterval = value;
                OnPropertyChanged("HideInterval");

            }
        }
        public bool AutoHide
        {
            get => autoHide;
            set
            {
                autoHide = value;
                OnPropertyChanged("AutoHide");
            }
        }
        public int ShowInterval
        {
            get => showInterval;
            set
            {
                showInterval = value;
                OnPropertyChanged("ShowInterval");
                OnIntervalChanged.Invoke(this, null);
            }
        }

    }


    public enum ShowOrder
    {
        AddDateAsc,
        AddDateDesc,
        Random
    }

}
