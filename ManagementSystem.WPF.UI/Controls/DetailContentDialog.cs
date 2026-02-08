
namespace ManagementSystem.WPF.UI.Controls
{
    public class DetailContentDialog : ContentDialog
    {
        /// <summary>Identifies the <see cref="IsPrimaryButtonVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsPrimaryButtonVisibleProperty = DependencyProperty.Register(
            nameof(IsPrimaryButtonVisible),
            typeof(bool),
            typeof(ContentDialog),
            new PropertyMetadata(true)
        );

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ContentDialog"/> primary button is Visible.
        /// </summary>
        public bool IsPrimaryButtonVisible
        {
            get => (bool)GetValue(IsPrimaryButtonVisibleProperty);
            set => SetValue(IsPrimaryButtonVisibleProperty, value);
        }


        IDetailViewModel _detailViewModel;
        public DetailContentDialog(IDetailViewModel vm) : base()
        {
            _detailViewModel = vm;
        }

        public Frame Frame1 { get; private set; }

        protected override void OnButtonClick(ContentDialogButton button)
        {
            var buttonClickEventArgs = new ContentDialogButtonClickEventArgs(ButtonClickedEvent, this)
            {
                Button = button,
            };

            RaiseEvent(buttonClickEventArgs);

            if (Frame1 == null && Content is Frame frame)
            {
                Frame1 = frame;
            }

            switch (button)
            {
                case ContentDialogButton.Primary:
                    _detailViewModel.TrySavePrimary();
                    break;

                case ContentDialogButton.Secondary:
                    _detailViewModel.TrySaveSecondary();
                    break;

                case ContentDialogButton.Close:
                    _detailViewModel.TryClose();
                    break;

                default:
                    break;
            }
        }

        public void Close(ContentDialogResult result)
        {
            Hide(result);
        }

        #region Mouse Drag Support

        /// Drag support fields
        private bool _isDragging;
        private Point _dragStartPoint;
        private double _startOffsetX;
        private double _startOffsetY;
        private TranslateTransform? _translate;
        private ContentPresenter? _titlePresenter;
        private Border? _mainBorder;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Unsubscribe previous handlers (if re-applied)
            if (_titlePresenter != null)
            {
                _titlePresenter.PreviewMouseLeftButtonDown -= Title_PreviewMouseLeftButtonDown;
                _titlePresenter.MouseMove -= Title_MouseMove;
                _titlePresenter.PreviewMouseLeftButtonUp -= Title_PreviewMouseLeftButtonUp;
                _titlePresenter.MouseLeave -= Title_MouseLeave;
            }

            _titlePresenter = GetTemplateChild("TitleContentPresenter") as ContentPresenter;
            _mainBorder = GetTemplateChild("MainBorder") as Border;

            if (_mainBorder != null)
            {
                // Ensure we have a TranslateTransform to move the border.
                if (_mainBorder.RenderTransform is TranslateTransform tt)
                {
                    _translate = tt;
                }
                else if (_mainBorder.RenderTransform is TransformGroup tg)
                {
                    // try to find existing TranslateTransform
                    foreach (var t in tg.Children)
                    {
                        if (t is TranslateTransform found)
                        {
                            _translate = found;
                            break;
                        }
                    }

                    if (_translate == null)
                    {
                        _translate = new TranslateTransform();
                        tg.Children.Add(_translate);
                    }
                }
                else if (_mainBorder.RenderTransform == Transform.Identity || _mainBorder.RenderTransform == null)
                {
                    _translate = new TranslateTransform();
                    _mainBorder.RenderTransform = _translate;
                }
                else
                {
                    // Other transform: wrap into TransformGroup so we don't lose existing transform.
                    var group = new TransformGroup();
                    group.Children.Add(_mainBorder.RenderTransform);
                    _translate = new TranslateTransform();
                    group.Children.Add(_translate);
                    _mainBorder.RenderTransform = group;
                }

                // improve rendering quality while dragging
                RenderOptions.SetBitmapScalingMode(_mainBorder, BitmapScalingMode.HighQuality);
            }

            if (_titlePresenter != null)
            {
                _titlePresenter.PreviewMouseLeftButtonDown += Title_PreviewMouseLeftButtonDown;
                _titlePresenter.MouseMove += Title_MouseMove;
                _titlePresenter.PreviewMouseLeftButtonUp += Title_PreviewMouseLeftButtonUp;
                _titlePresenter.MouseLeave += Title_MouseLeave;
            }
        }

        private void Title_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_mainBorder == null || _translate == null) return;

            _isDragging = true;

            // starting mouse position relative to the dialog control
            _dragStartPoint = e.GetPosition(this);

            // capture mouse so we keep receiving move/up events even if pointer leaves the title
            _titlePresenter?.CaptureMouse();

            _startOffsetX = _translate.X;
            _startOffsetY = _translate.Y;

            e.Handled = true;
        }

        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || _mainBorder == null || _translate == null) return;

            var current = e.GetPosition(this);
            var dx = current.X - _dragStartPoint.X;
            var dy = current.Y - _dragStartPoint.Y;

            _translate.X = _startOffsetX + dx;
            _translate.Y = _startOffsetY + dy;

            e.Handled = true;
        }

        private void Title_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndDrag();
            e.Handled = true;
        }

        private void Title_MouseLeave(object sender, MouseEventArgs e)
        {
            // If mouse left while dragging but still captured, continue; otherwise end drag.
            if (!_isDragging) return;
            // If the presenter still has capture, keep dragging; otherwise end.
            if (_titlePresenter != null && !_titlePresenter.IsMouseCaptured)
            {
                EndDrag();
            }
        }

        private void EndDrag()
        {
            if (!_isDragging) return;
            _isDragging = false;
            if (_titlePresenter != null && _titlePresenter.IsMouseCaptured)
            {
                _startOffsetX = _translate.X;
                _startOffsetY = _translate.Y;
                _titlePresenter.ReleaseMouseCapture();
            }
        }

        #endregion
    }
}
