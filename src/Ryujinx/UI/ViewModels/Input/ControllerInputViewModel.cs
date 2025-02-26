using Avalonia.Svg.Skia;
using CommunityToolkit.Mvvm.ComponentModel;
using Ryujinx.Ava.UI.Models.Input;
using Ryujinx.Ava.UI.Views.Input;
using Ryujinx.Common.Utilities;
using Ryujinx.UI.Views.Input;
using System.Drawing;

namespace Ryujinx.Ava.UI.ViewModels.Input
{
    public partial class ControllerInputViewModel : BaseModel
    {
        [ObservableProperty] private GamepadInputConfig _config;

        private bool _isLeft;
        public bool IsLeft
        {
            get => _isLeft;
            set
            {
                _isLeft = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSides));
            }
        }

        private bool _isRight;
        public bool IsRight
        {
            get => _isRight;
            set
            {
                _isRight = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSides));
            }
        }

        public bool HasSides => IsLeft ^ IsRight;

        [ObservableProperty] private SvgImage _image;

        public InputViewModel ParentModel { get; }

        public ControllerInputViewModel(InputViewModel model, GamepadInputConfig config)
        {
            ParentModel = model;
            model.NotifyChangesEvent += OnParentModelChanged;
            OnParentModelChanged();
            config.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName is nameof(Config.UseRainbowLed))
                {
                    if (Config is { UseRainbowLed: true, TurnOffLed: false, EnableLedChanging: true })
                        Rainbow.Updated += (ref Color color) => ParentModel.SelectedGamepad.SetLed((uint)color.ToArgb());
                    else
                    {
                        Rainbow.Reset();
                        
                        if (Config.TurnOffLed)
                            ParentModel.SelectedGamepad.ClearLed();
                        else
                            ParentModel.SelectedGamepad.SetLed(Config.LedColor.ToUInt32());
                    }
                }
            };
            Config = config;
        }

        public async void ShowMotionConfig()
        {
            await MotionInputView.Show(this);
        }

        public async void ShowRumbleConfig()
        {
            await RumbleInputView.Show(this);
        }
        
        public async void ShowLedConfig()
        {
            await LedInputView.Show(this);
        }

        public void OnParentModelChanged()
        {
            IsLeft = ParentModel.IsLeft;
            IsRight = ParentModel.IsRight;
            Image = ParentModel.Image;
        }
    }
}
