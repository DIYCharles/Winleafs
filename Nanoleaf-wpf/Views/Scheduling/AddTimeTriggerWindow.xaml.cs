﻿using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models;
using Nanoleaf_Models.Models.Effects;
using Nanoleaf_Models.Models.Scheduling.Triggers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for AddTriggerWindow.xaml
    /// </summary>
    public partial class AddTimeTriggerWindow : Window
    {
        private DayUserControl _parent;

        private TriggerType _triggerType { get; set; }
        
        private int _brightness { get; set; }

        public string SelectedEffect { get; set; }

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }

        public TriggerType TriggerType
        {
            get { return _triggerType; }
            set
            {
                _triggerType = value;

                TriggerTypeChanged();
            }
        }

        public IEnumerable<TriggerType> TriggerTypes
        {
            get
            {
                return Enum.GetValues(typeof(TriggerType)).Cast<TriggerType>();
            }
        }

        public List<Effect> Effects { get; set; }

        public AddTimeTriggerWindow(DayUserControl parent)
        {
            _parent = parent;
            Effects = UserSettings.Settings.ActviceDevice.Effects;
            Effects.Insert(0, new Effect { Name = Nanoleaf_Models.Models.Effects.Effect.NONEEFFECTNAME });

            DataContext = this;

            InitializeComponent();

            TriggerType = TriggerType.Time;
        }

        private void TriggerTypeChanged()
        {
            if (_triggerType == TriggerType.Time)
            {
                BeforeRadioButton.Visibility = Visibility.Hidden;
                AfterRadioButton.Visibility = Visibility.Hidden;
                TimeLabel.Content = "Time:";
            }
            else
            {
                BeforeRadioButton.Visibility = Visibility.Visible;
                AfterRadioButton.Visibility = Visibility.Visible;
                TimeLabel.Content = "Extra time:";
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int hours = 0;
            int minutes = 0;
            try
            {
                hours = Convert.ToInt32(Hours.Text, CultureInfo.InvariantCulture);

                if (hours < 0 || hours > 23)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for hours, between 0 and 23");
                return;
            }

            try
            {
                minutes = Convert.ToInt32(Minutes.Text, CultureInfo.InvariantCulture);

                if (minutes < 0 || minutes > 59)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for minutes, between 0 and 59");
                return;
            }

            if (TriggerType == TriggerType.Sunrise || TriggerType == TriggerType.Sunset)
            {
                var beforeAfter = BeforeRadioButton.IsChecked.Value ? BeforeAfter.Before : (AfterRadioButton.IsChecked.Value ? BeforeAfter.After : BeforeAfter.None);

                _parent.TriggerAdded(new TimeTrigger
                {
                    TriggerType = TriggerType,
                    BeforeAfter = beforeAfter,
                    Brightness = _brightness,
                    Effect = SelectedEffect,
                    ExtraHours = hours,
                    ExtraMinutes = minutes,
                    Hours = TriggerType == TriggerType.Sunrise ? UserSettings.Settings.SunriseHour.Value : UserSettings.Settings.SunsetHour.Value,
                    Minutes = TriggerType == TriggerType.Sunrise ? UserSettings.Settings.SunriseMinute.Value : UserSettings.Settings.SunsetMinute.Value
                });
            }
            else
            {
                _parent.TriggerAdded(new TimeTrigger
                {
                    TriggerType = TriggerType,
                    BeforeAfter = BeforeAfter.None,
                    Brightness = _brightness,
                    Effect = SelectedEffect,
                    ExtraHours = 0,
                    ExtraMinutes = 0,
                    Hours = hours,
                    Minutes = minutes
                });                
            }

            
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[0-9][0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
