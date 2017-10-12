using Devri.Common;
using static Devri.Common.BindablePage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;
using System.ComponentModel;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace Devri
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = null;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        private float m_temperature;        
        public float Temperature
        {
            get { return m_temperature; }
            set
            {
                SetProperty(ref m_temperature,value);
                OnPropertyChanged(nameof(DisplayTemperature));
            }
        }

        public string DisplayTemperature
        {
            get
            {
                return string.Format("{0} C", Temperature);
            }
        }
        private float m_humidity;
        public float Humidity
        {
            get { return m_humidity; }
            set
            {
                SetProperty(ref m_temperature, value);
                OnPropertyChanged(nameof(DisplayHumidity));
            }
        }

        public string DisplayHumidity
        {
            get
            {
                return string.Format("{0} C", Humidity);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}
